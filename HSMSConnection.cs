using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace JSecs
{
    public class HSMSConnection : IDisposable
    {
        public event EventHandler<SECSMessage> OnPriSECSMsgRcvd;

        TcpClient _tcpClient;
        TcpListener _tcpServer;
        ReceiveQueue _receiveQueue;
        SendQueue _sendQueue;
        ControlMessageGenerator _ctrlMsg;
        DataMessageGenerator _dataMsg;
        IPAddress _ip;
        SystemByteGenerator _systemByteGenerator;
        ConnectState _connState;
        Timer _t7Timer;
        Timer _linktestTimer;
        Func<Task> _connFunc;
        Action _disconnAction;
        CancellationTokenSource _cancelTokenSource;
        
        private static readonly DefaultSecsGemLogger _defaultLogger = new DefaultSecsGemLogger();
        
        private bool _isStarted;

        public bool IsStarted
        {
            get { return _isStarted; }
            set { _isStarted = value; }
        }


        int _port;
        public ushort DeviceId { get; set; } = 0;
        public int T3 { get; set; } = 45000;
        public int T5 { get; set; } = 25000;//10000
        public int T6 { get; set; } = 50000;//5000;
        public int T7 { get; set; } = 100000;//10000;
        public int T8 { get; set; } = 500000;//5000;
        public int LinktestInterval { get; set; } = 60000;
        
        bool _linktestEnable = false;
        public bool LinktestEnable
        {
            get => _linktestEnable;
            set
            {
                if (_linktestEnable = value) return;
                _linktestEnable = value;
                if (value)
                {
                    if (_linktestTimer == null)
                    {
                        _linktestTimer = new Timer(async delegate
                        {
                            if (_connState != ConnectState.SELECTED) return;

                            var LinktestRsp = await Task.Run(() => _ctrlMsg.LiketestReq());
                            if (LinktestRsp.IsError)
                            {
                                //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                                //For Active 5-3. T6 timeout waiting for Linktest.rsp;
                                //For Active 5-8. Other unrecoverable TCP/IP error
                                //1. Close TCP/IP connection
                                //2. Start T5 timeout
                                //For Passive 5-3. T6 timeout waiting for Linktest.rsp
                                //For Passive 5-8. Other unrecoverable TCP/IP error
                                //1. Close TCP/IP connection
                                _logger.Error(LinktestRsp.ErrorMsg);
                                _connState = ConnectState.NOT_CONNECTED;
                                await TryDisconnectAndReconnect();
                                return;
                            }
                            else  //normal Linktest.Rsp received
                                _logger.Info($"[{ConnName}] Linktest Success");

                        }, null, LinktestInterval, Timeout.Infinite);
                    }
                }
                else
                    _linktestTimer?.Change(Timeout.Infinite, Timeout.Infinite);


            }
        }
        bool _isActive;
        bool _isEquipment;
        bool _isDisposed;

        public string ConnName { get; set; }
        public string HandlerName { get; set; }
        //bool _isShowBinary = false;
        //public bool IsShowBinary
        //{
        //    get { return _isShowBinary; }
        //    set 
        //    {
        //        _isShowBinary = value;

        //        if (value)
        //            _logger.Info("Enable Show Binary");
        //        else
        //            _logger.Info("Disable Show Binary");
        //    }
        //}

        private ISecsGemLogger _logger = _defaultLogger;
        public ISecsGemLogger Logger
        {
            get => _logger;
            set => _logger = value ?? _defaultLogger;
        }

        private SECSLogFormat _logConfig;
        public SECSLogFormat SECSLogShowFormat
        {
            get { return _logConfig ?? new SECSLogFormat(); }
            set { _logConfig = value; }
        }

        //public static HSMSConnection CreateByConfig(HSMSConnectionConfiguration config)
        //{
        //    return new HSMSConnection(config.IsActive, config.IsEquipment, IPAddress.Parse(config.IP), config.Port);
        //}

        public HSMSConnection(bool isActive, bool isEquipment, IPAddress ip, int port)
        {
            if (port < 0 || port > 65535) throw new Exception("Port number invalid");
            _isActive = isActive;
            _isEquipment = isEquipment;
            _ip = ip;
            _port = port;
            _systemByteGenerator = new SystemByteGenerator();

            _t7Timer = new Timer(delegate
            {
                if (!_isActive && _connState == ConnectState.NOT_SELECTED)
                {
                    //[HSMS-SS] NOT_CONNECTED -> NOT_SELECTED 
                    //For Passive 4-1. T7 Timeout waiting for Select.req;
                    //1. Close TCP/IP connection
                    _connState = ConnectState.NOT_CONNECTED;
                    _logger.Error($"[{ConnName}] Occured T7 Timeout");
                    Task.Run(() => TryDisconnectAndReconnect());
                }
            }, null, Timeout.Infinite, Timeout.Infinite);

            _connState = ConnectState.NOT_CONNECTED;

            if (_isActive)
            {
                //start func do:
                //1 new tcpclient
                //2 add 2 queues and start their loop task
                //3 regist receive queue event
                //4 init ctrlMsg
                //5 send select req
                _connFunc = async () =>
                {
                    _isStarted = true;
                    //[HSMS-SS] -- -> NOT_CONNECTED 
                    //For Active 1. initialization;
                    _connState = ConnectState.NOT_CONNECTED;
                    //var connected = false;
                    while (_connState == ConnectState.NOT_CONNECTED)
                    {
                        if (_isDisposed) return;
                        _logger.Info($"[{ConnName}] Attempting to connect to {_ip} port {_port}");

                        try
                        {
                            //[HSMS-SS] NOT_CONNECTED -> NOT_SELECTED 
                            //For Active 1-1. Decide to connect;
                            //1. TCP/IP Connect
                            _tcpClient = new TcpClient();
                            await _tcpClient.ConnectAsync(_ip, _port);
                        }
                        catch (Exception ex)
                        {
                            //before initializing queue objects, just do t5 retry
                            if (_isDisposed) return;
                            _logger.Error(ex.Message);
                            _logger.Info($"[{ConnName}] Fail to connect");
                            _logger.Info($"[{ConnName}] Waiting for T5 before attempting next connect ({T5 / 1000}s)");
                            await Task.Delay(T5);
                            continue;
                        }

                        //connected = true;
                        _logger.Info($"[{ConnName}] TCP port connected");
                        CreateQueuesAndInit();

                        //_t7Timer.Change(T7, Timeout.Infinite);        //HSMS-GS use T7
                        //For Active 1-1. Decide to connect.
                        //2. Send Select.Req
                        //3. Start T6 Timer (in SECSTransaction.cs)
                        _connState = ConnectState.NOT_SELECTED;
                        var SelectRsp = await Task.Run(() => _ctrlMsg.SelectReq());
                        if (SelectRsp.IsError)
                        {
                            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                            //For Active 4-1. T6 timeout waiting for Select.rsp;
                            //For Active 4-7. Other unrecoverable TCP/IP error
                            //1. Close TCP/IP connection
                            //2. Start T5 timeout   
                            _logger.Error(SelectRsp.ErrorMsg);
                            _connState = ConnectState.NOT_CONNECTED;
                            await TryDisconnectAndReconnect();
                            return;
                        }
                        if (SelectRsp.Header.SType != 2)
                        {
                            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                            //For Active 4-3. Receive any HSMS message other than Select.rsp;
                            //1. Close TCP/IP connection
                            //2. Start T5 timeout
                            _logger.Error($"[{ConnName}] Invalid SType for Select.Rsp");
                            _connState = ConnectState.NOT_CONNECTED;
                            await TryDisconnectAndReconnect();
                            return;
                        }
                        if (SelectRsp.Header.Bytes[3] != 0)
                        {
                            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                            //For Active 4-2. Receive Select.rsp with non-zero Select Status;
                            //1. Close TCP/IP connection
                            //2. Start T5 timeout
                            _logger.Error($"[{ConnName}] The responding entity rejected the Select.Req");
                            _connState = ConnectState.NOT_CONNECTED;
                            await TryDisconnectAndReconnect();
                            return;
                        }
                        if (SelectRsp.Bytes.Length != 10)
                        {
                            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                            //For Active 4-4. Receive HSMS message length not equal to 10;
                            //1. Close TCP/IP connection
                            //2. Start T5 timeout
                            _logger.Error($"[{ConnName}] The responding entity rejected the Select.Req");
                            _connState = ConnectState.NOT_CONNECTED;
                            await TryDisconnectAndReconnect();
                            return;
                        }
                        else  //normal SelectRsp received
                        {
                            //[HSMS-SS] NOT_SELECTED -> SELECTED 
                            //For Active 3-1. Receive Select.rsp with zero SelectStatus;
                            //1. Cancel T6 timeout (in SECSTransaction.cs)
                            _connState = ConnectState.SELECTED;
                            //_t7Timer.Change(Timeout.Infinite, Timeout.Infinite);  //HSMS-GS use T7
                            _logger.Info($"[{ConnName}] HSMS Selected");
                        }

                    }
                };
            }
            else //Passive Mode
            {
                _tcpServer = new TcpListener(IPAddress.Any, _port);
                try
                {
                    _tcpServer.Start();
                }
                catch (SocketException ex)
                {
                    throw ex;
                }

                _logger.Info($"[{ConnName}] Starting passive mode on TCP port {_port}");

                //start func do:
                //1 assign tcpclient
                //2 start t7
                //3 add 2 queues and start their loop task
                //4 regist receive queue event
                //5 init ctrlCmd
                _connFunc = async () =>
                {
                    _isStarted = true;
                    //[HSMS-SS] -- -> NOT_CONNECTED 
                    //For Passive 1. initialization;
                    _connState = ConnectState.NOT_CONNECTED;
                    _logger.Info($"[{ConnName}] Waiting for a connection...");
                    //var connected = false;
                    while (_connState == ConnectState.NOT_CONNECTED)
                    {
                        try
                        {
                            if (_isDisposed) return;

                            // Perform a blocking call to accept requests.
                            // Could also use server.AcceptSocket() here.
                            _tcpClient = await _tcpServer.AcceptTcpClientAsync().ConfigureAwait(false);
                            //connected = true;
                            //[HSMS-SS] NOT_CONNECTED -> NOT_SELECTED
                            //For Passive 2-1. TCP/IP “accept” succeeds;
                            //1. Start T7 timeout
                            _logger.Info("TCP port connected");
                            _connState = ConnectState.NOT_SELECTED;
                            _t7Timer.Change(T7, Timeout.Infinite);
                            CreateQueuesAndInit();
                        }
                        catch (Exception ex)
                        {
                            if (_isDisposed) return;

                            _logger.Error(ex.Message);
                            await Task.Delay(2000).ConfigureAwait(false);
                        }
                    }
                };
            }

            void CreateQueuesAndInit()
            {
                
                _cancelTokenSource = new CancellationTokenSource();
                _sendQueue = new SendQueue(_tcpClient.GetStream(), _logger, _cancelTokenSource.Token);
                _receiveQueue = new ReceiveQueue(_tcpClient.GetStream(), _logger, _cancelTokenSource.Token, T8);
                _sendQueue.LogConfiguration = SECSLogShowFormat;
                _receiveQueue.LogConfiguration = SECSLogShowFormat;
                _receiveQueue.OnCtrlMsgReqRcvd += _receiveQueue_OnCtrlMsgReqRcvd;
                _receiveQueue.OnPriSECSMsgRcvd += _receiveQueue_OnPriSECSMsgRcvd;
                _receiveQueue.OnInvalidMessageLength += _receiveQueue_OnInvalidMessageLength;
                _receiveQueue.OnInvalidMessageHeader += _receiveQueue_OnInvalidMessageHeader;
                _receiveQueue.OnT8TimeoutOccured += _receiveQueue_OnT8TimeoutOccured;
                _receiveQueue.OnToSendRejectReq += _receiveQueue_OnToSendRejectReq;
                _ctrlMsg = new ControlMessageGenerator(_receiveQueue, _sendQueue, T6, _systemByteGenerator, _logger);
                _dataMsg = new DataMessageGenerator(_receiveQueue, _sendQueue, T3, _systemByteGenerator, _logger);
            }
        }

        private void _receiveQueue_OnToSendRejectReq(object sender, RejectEventArgs e)
        {
            Task.Run(() => _ctrlMsg?.RejectReq(e.RequestHeader.Bytes, e.ReasonCode));
        }

        private void _receiveQueue_OnT8TimeoutOccured(object sender, EventArgs e)
        {
            _logger.Error($"[{ConnName}] Occured T8 Timeout");
            if (_isDisposed) return;
            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
            //For Active 4-6 & 5-7. Receive bad HSMS message header;
            //1. Close TCP/IP connection
            //2. Start T5 timeout
            //For Passive 4-6 & 5-7. Receive bad HSMS meesage header;
            //1. Close TCP/IP connection
            if (_connState == ConnectState.NOT_SELECTED || _connState == ConnectState.SELECTED)
            {
                _connState = ConnectState.NOT_CONNECTED;
                Task.Run(() => TryDisconnectAndReconnect());
            }
        }

        private void _receiveQueue_OnInvalidMessageHeader(object sender, InvalidMessageHeaderEventArgs e)
        {
            //bad header check is in ReceiveQueue class when decoding byte[] to secsmessage
            //raise event when check fail
            _logger.Error($"[{ConnName}] Invalid message header {e.Header} received");
            if (_isDisposed) return;
            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
            //For Active 4-5 & 5-6. Receive bad HSMS message header;
            //1. Close TCP/IP connection
            //2. Start T5 timeout
            //For Passive 4-5 & 5-6. Receive bad HSMS message header;
            //1. Close TCP/IP connection
            if (_connState == ConnectState.NOT_SELECTED || _connState == ConnectState.SELECTED)
            {
                _connState = ConnectState.NOT_CONNECTED;
                Task.Run(() => TryDisconnectAndReconnect());
            }
        }

        private void _receiveQueue_OnInvalidMessageLength(object sender, InvalidMessageLengthEventArgs e)
        {
            _logger.Error($"[{ConnName}] Invalid message length {e.MessageLength} (10~2G) received");
            if (_isDisposed) return;
            //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
            //For Active 5-4. Receive HSMS message length < 10;
            //For Active 5-5. Receive HSMS message length > max supported by entity (2G) //actually should be 4g, but some .net function do not have uint parameter overloads
            //1. Close TCP/IP connection
            //2. Start T5 timeout
            //For Passive 5-4. Receive HSMS message length < 10
            //For Passive 5-5. Receive HSMS message length > max supported by entity (2G) //actually should be 4g, but some .net function do not have uint parameter overloads
            //1. Close TCP/IP connection
            _connState = ConnectState.NOT_CONNECTED;
            Task.Run(() => TryDisconnectAndReconnect());
        }

        private void _receiveQueue_OnPriSECSMsgRcvd(object sender, SECSMessage e)
        {
            if (_connState == ConnectState.NOT_SELECTED)
            {
                //For 8.2.8 ReasonCode 4 Entity Not Selected. A data message was received when not in the SELECTED state.
                Task.Run(() => _ctrlMsg?.RejectReq(e.Header.Bytes, 4));
                //some data messages are header only e.g. S1F1 W
                if (e.Bytes.Length != 10)
                {
                    //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                    //For Active 4-4. Receive HSMS message length not equal to 10;
                    //1. Close TCP/IP connection
                    //2. Start T5 timeout
                    //For Passive 4-4. Receive HSMS message length not equal to 10;
                    //1. Close TCP/IP connection
                    _connState = ConnectState.NOT_CONNECTED;
                    Task.Run(() => TryDisconnectAndReconnect());
                    return;
               
                    //bad header check is in ReceiveQueue class when decoding byte[] to secsmessage
                    //if (e.IsError && e.IsBadHeader)   //bad header check is in ReceiveQueue
                    //{
                    //    //[HSMS-SS-Active] NOT_SELECTED -> NOT_CONNECTED 
                    //    //For4-5 & 5-6. Receive bad HSMS message header;
                    //    //1. Close TCP/IP connection
                    //    //2. Start T5 timeout
                    //    _connState = ConnectState.NOT_CONNECTED;
                    //    Task.Run(() => TryDisconnectAndReconnect());
                    //    return;
                    //}
                }
                return;
            }

            e.FromConnection = this;
            OnPriSECSMsgRcvd?.Invoke(this, e);
        }

        private void _receiveQueue_OnCtrlMsgReqRcvd(object sender, SECSMessage e)
        {
            e.FromConnection = this;
            switch (e.Header.SType)
            {
                case 1:
                    _logger.Info($"[{ConnName}] Received Select.Req");
                    break;
                case 3:
                    _logger.Info($"[{ConnName}] Received Deselect.Req");
                    break;
                case 5:
                    _logger.Info($"[{ConnName}] Received Linktest.Req");
                    break;
                case 7:
                    _logger.Info($"[{ConnName}] Received Reject.Req");
                    break;
                case 9:
                    _logger.Info($"[{ConnName}] Received Separate.Req");
                    break;
            }

            //SType = 9 : Separate.Req
            if (e.Header.SType == 9 && _connState == ConnectState.SELECTED)
            {
                //[HSMS-SS] SELECTED -> NOT_CONNECTED 
                //For Active 5-2. Receive Separate.req;
                //1. Close TCP/IP connection
                //2. Start T5 timeout
                //For Passive 5-2. Receive Separate.req;
                //1. Close TCP/IP connection
                _connState = ConnectState.NOT_CONNECTED;
                Task.Run(() => TryDisconnectAndReconnect());
                return;
            }

            //select.rsp should not receive by event, but for transaction return;
            //so, reject any message when NOT_SELECT
            if (_isActive && _connState == ConnectState.NOT_SELECTED)
            {
                //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                //For Active 4-3. Receive any HSMS message other than Select.rsp;
                //1. Close TCP/IP connection
                //2. Start T5 timeout
                _connState = ConnectState.NOT_CONNECTED;
                Task.Run(() => TryDisconnectAndReconnect());
                return;
            }

            //SType = 1 : Select.Req
            if (e.Header.SType != 1 && !_isActive && _connState == ConnectState.NOT_SELECTED)
            {
                //[HSMS-SS] NOT_SELECTED -> NOT_CONNECTED 
                //For Passive 4-3. Receive any HSMS message other than Select.req;
                //1. Close TCP/IP connection
                _connState = ConnectState.NOT_CONNECTED;
                Task.Run(() => TryDisconnectAndReconnect());
                return;
            }

            if ((!_isActive && !_tcpServer.Server.IsBound) || _isDisposed) return;

            switch (e.Header.SType)
            {
                case 1: //Select.req in passive mode
                    if (_connState == ConnectState.NOT_SELECTED ||  //for normal not select
                        _connState == ConnectState.SELECTED)        //for simultaneous select   //actually HSMS-SS do not need support simultaneous case. should just disconnect
                    {
                        //[HSMS-SS] NOT_SELECTED -> SELECTED 
                        //For Passive 3-1. Receive Select.req and decide to allow it;
                        //1. Cancel T7 timeout
                        //2. Send Select.rsp with zero SelectStatus
                        _connState = ConnectState.SELECTED;
                        _t7Timer.Change(Timeout.Infinite, Timeout.Infinite); 
                        Task.Run(() => _ctrlMsg.SelectRsp(e.Bytes, true));
                        _logger.Info($"[{ConnName}] HSMS Selected");

                        //if wanna reject
                        //For Passive 4-2. Receive Select.req and decide to reject it;
                        //Task.Run(async () => 
                        //{
                        //    await _ctrlMsg.SelectRsp(e.Bytes, false);
                        //    await TryDisconnectAndReconnect();
                        //});
                    }
                    break;
                //HSMS-SS do not use Deselect (7.3)
                //case 3: //Deselect.req in passive mode
                //    if (_connState == ConnectState.SELECTED ||      //for normal select
                //        _connState == ConnectState.NOT_SELECTED)    //for simultaneous select
                //    {
                //        Task.Run(() => _ctrlMsg.DeselectRsp(e.Bytes, true));
                //        _connState = ConnectState.NOT_SELECTED;
                //        _logger.Info("HSMS Deselected");
                //    }
                //    break;
                case 5: //Linktest.req in both mode
                    if (_connState == ConnectState.SELECTED)
                        Task.Run(() => _ctrlMsg.LiketestRsp(e.Bytes, true));

                    break;
                case 7: //Reject.req
                    _logger.Error($"[{ConnName}] Received Reject.req ReasonCode:{e.Header.Bytes[3]} SystemByte:{e.SystemByte}");
                    if (_isDisposed) return;
                    //[HSMS-SS] SELECTED & NOT_SELECTED -> NOT_CONNECTED 
                    //For 7.5 Reject Procedure — The Reject Procedure is optional in HSMS communications. 
                    //Note, however, that any situation which would require the use of the Reject as described
                    //in HSMS Generic Services shall be treated as a communications failure in implementations 
                    //not supporting reject. Specifically, the TCP/IP connection is immediately closed.
                    if (_connState == ConnectState.SELECTED || _connState == ConnectState.NOT_SELECTED)
                    {
                        _connState = ConnectState.NOT_CONNECTED;
                        Task.Run(() => TryDisconnectAndReconnect());
                    }
                    break;
                //case 9: is on the top of the func
            }
        }

        public void Separate()
        {
            //if (_connState != ConnectState.SELECTED) return;

            //[HSMS-SS] SELECTED -> NOT_CONNECTED 
            //For Active 5-1. Decide to terminate and send Separate.req;
            //1. Close TCP/IP connection
            //2. Start T5 timeout
            //For Passive 5-1. Decide to terminate and send Separate.req;
            //1. Close TCP/IP connect
            Task.Run(async () => 
            {
                await _ctrlMsg.SeparateReq();
                _connState = ConnectState.NOT_CONNECTED;
                await TryDisconnectAndReconnect();
            });
        }

        //config file
        public void Config() { }
        public void Start() => new TaskFactory(TaskScheduler.Default).StartNew(_connFunc);
        public void Stop() 
        {
            //stop func do:
            //1 dispose ctrlMsg, dataMsg
            //2 unregist receive queue event and other error event
            //3 dispose 2 queues and stop their loop task
            //4 stop t7 and linktest
            //5 dispose tcpclient
            //6 dispose network stream
            _dataMsg = null;
            _ctrlMsg = null;
            _receiveQueue.OnCtrlMsgReqRcvd -= _receiveQueue_OnCtrlMsgReqRcvd;
            _receiveQueue.OnPriSECSMsgRcvd -= _receiveQueue_OnPriSECSMsgRcvd;
            _receiveQueue.OnInvalidMessageLength -= _receiveQueue_OnInvalidMessageLength;
            _receiveQueue.OnInvalidMessageHeader -= _receiveQueue_OnInvalidMessageHeader;
            _receiveQueue.OnT8TimeoutOccured -= _receiveQueue_OnT8TimeoutOccured;
            _receiveQueue.OnToSendRejectReq -= _receiveQueue_OnToSendRejectReq;
            _cancelTokenSource.Cancel();
            _receiveQueue = null;
            _sendQueue = null;
            _t7Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _linktestTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _tcpClient?.GetStream()?.Close();
            _tcpClient?.Close();
            _tcpClient.Dispose();
            _logger.Info($"[{ConnName}] TCP disconnected");
            _isStarted = false;

        }
        private async Task TryDisconnectAndReconnect()
        {
            if (_isDisposed) return;
            Stop();
            if (_isActive)
            {
                _logger.Info($"[{ConnName}] Waiting for T5 before attempting next connect ({T5 / 1000}s)");
                await Task.Delay(T5);
            } 
            Start();
        }
        public async Task<SECSMessage> SendRequest(SECSMessage msg)
        {
            return await _dataMsg.SendPrimaryMessage(msg);
        }

        public async Task Reply(SECSMessage receivedMsg, SECSMessage sendMsg)
        {
            await _dataMsg.Reply(receivedMsg, sendMsg);
        }

        public void Dispose()
        {
            if (_connState == ConnectState.SELECTED)
            {
                _ctrlMsg.SeparateReq().Wait();
                _connState = ConnectState.NOT_CONNECTED;
            }
            

            Stop();

            _t7Timer.Dispose();
            _linktestTimer.Dispose();

            _isDisposed = true;
        }

#if DEBUG
        public void Select()
        {
            Task.Run(() => _ctrlMsg.SelectReq());
        }

        public void Deselect()
        {
            Task.Run(() => _ctrlMsg.DeselectReq());
        }

        public void LinkTest()
        {
            Task.Run(() => _ctrlMsg.LiketestReq());
        }

        public void AreYouThere()
        {
            Task.Run(async () =>
            {
                var reply = await _dataMsg.AreYouThere();
            } );
        }

#endif
    }
}
