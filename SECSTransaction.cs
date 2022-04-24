using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSecs
{
    class SECSTransaction
    {
        //public event EventHandler<int> OnT3TimeoutOccured;
        //public event EventHandler<int> OnT6TimeoutOccured;

        ReceiveQueue _receiver;
        SendQueue _sender;
        SECSMessage _replyMsg;
        ISecsGemLogger _logger;
        public int T3 { get; set; }
        public int T6 { get; set; }
        Timer _timerT3;
        Timer _timerT6;

        bool _T3TimeoutOccured;
        bool _T6TimeoutOccured;
        public SECSTransaction(ReceiveQueue receiver, SendQueue sender, ISecsGemLogger logger, int T3 = 45000, int T6 = 5000)
        {
            _receiver = receiver;
            _sender = sender;
            _logger = logger;
            this.T3 = T3;
            this.T6 = T6;
            _timerT3 = new Timer((o) =>
            {
                //OnT3TimeoutOccured?.Invoke(this, msg.SystemByte);
                _T3TimeoutOccured = true;
            }, null, Timeout.Infinite, Timeout.Infinite);
            _timerT6 = new Timer((o) =>
            {
                //OnT6TimeoutOccured?.Invoke(this, msg.SystemByte);
                _T6TimeoutOccured = true;
            }, null, Timeout.Infinite, Timeout.Infinite);

        }

        internal async Task<SECSMessage> ControlMessageRequest(byte[] bytes)
        {
            int systemByte = (bytes[10] << 24) + (bytes[11] << 16) + (bytes[12] << 8) + bytes[13];
            //Only support Select.req, Deselect.req, Linktest.req
            if (bytes[9] != 1 && bytes[9] != 3 && bytes[9] != 5)
                return SECSMessage.CreateErrorMsg($"Invalid SType {bytes[9]} for Control Message when ControlMessageRequest", systemByte);

            _replyMsg = null;
            _T6TimeoutOccured = false;

            //[HSMS-SS-Active] NOT_CONNECTED -> NOT_SELECTED 
            //For Active 1-1. Decide to connect.
            //3. Start T6 timeout
            _timerT6.Change(T6, Timeout.Infinite);
            
            //regist msg in receiver
            if (!_receiver.RegistReplyForSystemByte(systemByte, GetReplyMessage)) 
                return SECSMessage.CreateErrorMsg("Fail to Regist Control Message", systemByte);

            //enqueue msg into sender
            await Task.Run(() => _sender.Enqueue(bytes));

            return await Task.Run(async () =>
            {
                while (true)
                {
                    if (_T6TimeoutOccured)
                    {
                        _receiver.UnregistReplyForSystemByte(systemByte);
                        //T6Type type = T6Type.UNKNOWN;
                        switch (bytes[9])   //SType
                        {
                            case 1:
                                //type = T6Type.SELECT;
                                return SECSMessage.CreateErrorMsg($"Occured T6 Timeout for Select.Req SystemByte:{ systemByte }", systemByte);
                            case 3:
                                //type = T6Type.DESELECT;
                                return SECSMessage.CreateErrorMsg($"Occured T6 Timeout for Deselect.Req SystemByte:{ systemByte }", systemByte);
                            case 5:
                                //type = T6Type.LINKTEST;
                                return SECSMessage.CreateErrorMsg($"Occured T6 Timeout for Linktest.Req SystemByte:{ systemByte }", systemByte);
                        }

                        //string msg = $"Occured T6 Timeout for SystemByte:{systemByte}";
                        //var ex = new T6TimeoutException(type, systemByte, msg);
                        //throw ex;
                    }
                    if (_replyMsg != null)
                    {
                        //[HSMS-SS] NOT_SELECTED -> SELECTED 
                        //For Active 3-1. Receive Select.rsp with zero SelectStatus
                        //1. Cancel T6 timeout
                        _timerT6.Change(Timeout.Infinite, Timeout.Infinite);
                        return _replyMsg;
                    }

                    await Task.Delay(50);
                }
            });

            //return null;
        }

        internal async Task ControlMessageSendOnly(byte[] bytes)
        {
            if (bytes[9] != 7 && bytes[9] != 9)
            {
                _logger.Error($"Invalid SType {bytes[9]} for Control Message when ControlMessageSendOnly");
                return;
            }
            
            await Task.Run(() => _sender.Enqueue(bytes));
        }

        //use for tester
        internal async Task<SECSMessage> SendRequest(byte[] bytes)
        {
            _replyMsg = null;
            _T3TimeoutOccured = false;
            _timerT3.Change(T3, Timeout.Infinite);

            int systemByte = (bytes[10] << 24) + (bytes[11] << 16) + (bytes[12] << 8) + bytes[13];

            //regist msg in receiver
            if (!_receiver.RegistReplyForSystemByte(systemByte, GetReplyMessage))
                return SECSMessage.CreateErrorMsg("Fail to Regist Primary Message", systemByte);

            //enqueue msg into sender
            await Task.Run(() => _sender.Enqueue(bytes));
            

            return await Task.Run(async () =>
            {
                while (true)
                {
                    if (_T3TimeoutOccured)
                    {
                        _receiver.UnregistReplyForSystemByte(systemByte);
                        return SECSMessage.CreateErrorMsg($"Occured T3 Timeout SystemByte:{ systemByte }", systemByte);
                        //throw new Exception($"Occured T3 Timeout for SystemByte:{msg.SystemByte}");
                        //return null;
                    }
                    if (_replyMsg != null)
                    {
                        _timerT3.Change(Timeout.Infinite, Timeout.Infinite);
                        return _replyMsg;
                    }

                    await Task.Delay(2);
                }
            });

            //return null;
        }

        internal async Task<SECSMessage> SendRequest(SECSMessage msg)
        {
            //if (!msg.IsPrimary && msg.Function != 0) throw new Exception("Invalid Function for SECSMessage");
            if (!msg.IsPrimary) return SECSMessage.CreateErrorMsg("Invalid Function for SECSMessage", msg.SystemByte);

            if (!msg.WBit) msg.WBit = true;

            _replyMsg = null;
            _T3TimeoutOccured = false;
            _timerT3.Change(T3, Timeout.Infinite);

            //regist msg in receiver
            if (!_receiver.RegistReplyForSystemByte(msg.SystemByte, GetReplyMessage))
                return SECSMessage.CreateErrorMsg("Fail to Regist Primary Message", msg.SystemByte);

            //enqueue msg into sender
            //await Task.Run(() => _sender.Enqueue(msg));
            _sender.Enqueue(msg);

            //return await Task.Run(async () =>
            await Task<SECSMessage>.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_T3TimeoutOccured)
                    {
                        _receiver.UnregistReplyForSystemByte(msg.SystemByte);
                        return SECSMessage.CreateErrorMsg($"Occured T3 Timeout for S{ msg.Stream }F{ msg.Function } SystemByte:{ msg.SystemByte }", msg.SystemByte);
                        //throw new Exception($"Occured T3 Timeout for SystemByte:{msg.SystemByte}");
                        //return null;
                    }
                    if (_replyMsg != null)
                    {
                        _timerT3.Change(Timeout.Infinite, Timeout.Infinite);
                        return _replyMsg;
                    }

                    //await Task.Delay(2);
                }
            }, TaskCreationOptions.LongRunning);
            //});



            //return null;
        }

        internal async Task SendOnly(SECSMessage msg)
        {
            //if (!msg.IsPrimary && msg.Function != 0) throw new Exception("Invalid Function for SECSMessage");
            if (!msg.IsPrimary) throw new Exception("SECSMessage to send is not primary");
            if (msg.WBit)
            {
                msg.WBit = false;
                _logger.Warning("SendOnly message with W bit, remove W bit by auto");
            }
            //enqueue msg into sender
            await Task.Run(() => _sender.Enqueue(msg));
        }

        internal async Task Reply(SECSMessage receivedMsg, SECSMessage replyMsg)
        {
            if (!receivedMsg.IsPrimary) throw new Exception("SECSMessage received is not primary");
            if (!receivedMsg.WBit) throw new Exception("SECSMessage received is not a Wait-Reply message");
            if (replyMsg.IsPrimary) throw new Exception("SECSMessage to reply is not a secondary message");
            
            //enqueue msg into sender
            await Task.Run(() => _sender.Enqueue(replyMsg));
        }

        internal async Task AbnormalReply(SECSMessage receivedMsg, SECSMessage replyMsg)
        {
            //this function is to send S9FX and SXF0
        }

        private void GetReplyMessage(SECSMessage replyMsg)
        {
            _replyMsg = replyMsg;
        }


    }
}
