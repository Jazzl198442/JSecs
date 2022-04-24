using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JSecs
{
    class DataMessageGenerator
    {
        ReceiveQueue _receiver;
        SendQueue _sender;
        SystemByteGenerator _systemByteGenerator;
        ISecsGemLogger _logger;
        public int T3 { get; set; }
        public DataMessageGenerator(ReceiveQueue receiver, SendQueue sender, int T3, SystemByteGenerator systemByteGenerator, ISecsGemLogger logger)
        {
            _receiver = receiver;
            _sender = sender;
            _systemByteGenerator = systemByteGenerator;
            this.T3 = T3;
            _logger = logger;
        }

        public int NewSystemByte => _systemByteGenerator.New();

        public async Task<SECSMessage> SendPrimaryMessage(SECSMessage msg)
        {
            msg.Header.SystemByte = NewSystemByte;
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T3);
            var result = await trans.SendRequest(msg);
            return result;
        }

        public async Task Reply(SECSMessage receivedMsg, SECSMessage replyMsg)
        {
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T3);
            await trans.Reply(receivedMsg, replyMsg);
        }

        public async Task<SECSMessage> AreYouThere()
        {
            byte[] bytes = new byte[14] { 0, 0, 0, 10, 0, 1, 0x81, 1, 0, 0, 0, 0, 0, 0 };
            byte[] systemBytes = BitConverter.GetBytes(NewSystemByte);
            if (BitConverter.IsLittleEndian) Array.Reverse(systemBytes);
            Buffer.BlockCopy(systemBytes, 0, bytes, 10, 4);

            _logger.Info("S1F1 Are You There");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T3);
            var result = await trans.SendRequest(bytes);
            return result;
        }
    }
}
