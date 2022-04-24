using JSecs.E5;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSecs
{
    public class SECSMessage
    {
        private byte[] _buffer;
        internal bool IsError { get; set; } = false;
        //public bool IsBadHeader { get; set; } = false;
        internal string ErrorMsg { get; set; }
        public int SystemByte { get => Header.SystemByte; internal set => Header.SystemByte = value; }
        public ushort SessionID { get => Header.SessionID; }
        public HSMSConnection FromConnection { get; internal set; }
        public byte[] Bytes 
        {
            get 
            {
                if (_buffer != null)    //received secs message is instantly assign byte to buffer
                    return _buffer;
                else                    //new secs message by driver should encode to bytes
                {
                    //var headerBuffer = Header.Bytes;
                    //var bodyBuffer = SECSEncoder.Encode(Root);
                    //_buffer = new byte[10 + bodyBuffer.Length];
                    //Buffer.BlockCopy(headerBuffer, 0, _buffer, 0, headerBuffer.Length);
                    //Buffer.BlockCopy(bodyBuffer, 0, _buffer, 10, bodyBuffer.Length);

                    //return _buffer;
                    return CreateBytes();
                    ////throw new NotImplementedException();
                }
            }
        }

        internal byte[] CreateBytes()
        {
            var headerBuffer = Header.Bytes;
            var bodyBuffer = SECSEncoder.Encode(Root);
            _buffer = new byte[10 + bodyBuffer.Length];
            Buffer.BlockCopy(headerBuffer, 0, _buffer, 0, headerBuffer.Length);
            Buffer.BlockCopy(bodyBuffer, 0, _buffer, 10, bodyBuffer.Length);

            return _buffer;
        }
        public bool IsPrimary { get => Function % 2 == 1; }
        public int Stream
        {
            get => Header.Stream;
            set
            {
                if (value < 1 || value > 127) throw new Exception("Stream should be between 1 & 127");
                Header.Stream = (byte)value;
            }
        }
        public int Function
        {
            get => Header.Function;
            set
            {
                if (value < 0 || value > 255) throw new Exception("Function should be between 0 & 255");
                Header.Function = (byte)value;
            }
        }
        public bool WBit { get => Header.Wbit; set => Header.Wbit = value; }
        internal SECSMessageHeader Header { get; set; }

        private SECSItem _body;

        public SECSItem Root
        {
            get { return _body; }
            set { _body = value; }
        }

        //use for error message for replying ctrl msg
        internal SECSMessage() { }

        public SECSMessage(int S, int F, bool W = false) 
        {
            Header = GetInitHeader(S, F, W);
        }

        public SECSMessage(string SxFx)
        {
            string sf = SxFx.ToUpper();
            string regex = @"^S([1-9]|[1-9][0-9]|1[01][0-9]|12[0-7])F([01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])(W{0,1}|(\sW){0,1})$";
            if (!Regex.Match(sf, regex).Success)
                throw new Exception("SxFx String format not support");
            int S, F;
            bool W = sf.EndsWith('W');
            sf = sf.Trim(' ').TrimEnd('W');
            string[] s = sf.Split('S', 'F');
            S = int.Parse(s[1]);
            F = int.Parse(s[2]);
            Header = GetInitHeader(S, F, W);
        }

        private SECSMessageHeader GetInitHeader(int S, int F, bool W = false)
        {
            if (S < 1 || S > 127) throw new Exception("Stream should be between 1 & 127");
            if (F < 0 || F > 255) throw new Exception("Function should be between 0 & 255");

            byte[] bytes = new byte[10];
            bytes[2] = (byte)((W ? 128 : 0) + S);
            bytes[3] = (byte)F;
            return new SECSMessageHeader(bytes);
        }

        //only used by ReceiveQueue
        internal SECSMessage(byte[] buffer)
        {
            this._buffer = buffer;
            Header = new SECSMessageHeader(this);
            if (buffer.Length > 10)
            {
                try
                {
                    int offset = 10;
                    SECSItem body = SECSDecoder.Decode(buffer, ref offset);

                    if (offset != buffer.Length)
                        throw new Exception("Decode not to the end of the message");

                    Root = body;
                    //Debug Root body here
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        internal static SECSMessage CreateErrorMsg(string errorMsg, int systemByte)
        {
            SECSMessage msg = new SECSMessage();
            msg.IsError = true;
            msg.ErrorMsg = errorMsg;
            //msg.SystemByte = systemByte;
            return msg;
        }

        public SECSItem this[int index]
        {
            get
            {
                if (Root.SECSType != SECSType.L)
                    return Root[index];
                else
                    return (Root as SECSListItem)[index];

            }
            set
            {
                if (Root.SECSType != SECSType.L)
                    Root[index] = value;
                else
                    (Root as SECSListItem)[index] = value;
            }
        }

        public async Task<SECSMessage> SendRequestAsync(HSMSConnection conn)
        {
            Header.SessionID = conn.DeviceId;
            Header.SType = 0;
            Header.PType = 0;
            return await conn.SendRequest(this);
        }

        public SECSMessage SendRequest(HSMSConnection conn)
        {
            Func<Task> t;
            SECSMessage reply = null; //= new SECSMessage();
            t = async () =>
            {
                reply = await SendRequestAsync(conn);
            };
            Task.Run(t).Wait();
            return reply;
        }

        public async Task ReplyAsync(SECSMessage replyMsg)
        {
            if (this.FromConnection == null) throw new Exception("Can't get the connection of the Received Msg, or the Msg is not a connection based receiving SECS Primary Msg");
            //if (replyMsg.Stream != this.Stream) throw new Exception("SECSMessage to reply doesn't have the same Stream as the primary message");
            //if (replyMsg.Function != this.Function + 1) throw new Exception("SECSMessage to reply doesn't have the invalid Function according to the primary message");

            replyMsg.Header.SessionID = this.FromConnection.DeviceId;
            replyMsg.Header.SType = 0;
            replyMsg.Header.PType = 0;
            replyMsg.SystemByte = this.SystemByte;
            //replyMsg.IsSimMessage = this.IsSimMessage;

            await this.FromConnection.Reply(this, replyMsg);
        }

        public void Reply(SECSMessage replyMsg)
        {
            Func<Task> t;
            t = async () =>
            {
                await ReplyAsync(replyMsg);
            };
            Task.Run(t).Wait();
        }

        public void Add(SECSItem item)
        {
            if (Root == null || Root.Length == 0)
            {
                Root = item;
            }
            else
            {
                if (Root.SECSType != SECSType.L)
                    throw new Exception("SECS Message only support 1 Non-List item on the root");

                L l = Root as SECSListItem;
                l.Add(item);
            }
        }

        public string ToString(bool isShowCount = false, bool isShowIndex = false, bool isShowAttribute = false)
        {
            string wbitStr = WBit ? " W\r" : "\r";
            string msgBody = Root.ToString(0, isShowCount, isShowIndex, 0, isShowAttribute);
            string s = $"S{Stream}F{Function}{wbitStr}{msgBody}";
            return s;
        }


    }
}