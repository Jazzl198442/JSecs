using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JSecs
{
    class ControlMessageGenerator
    {
        ReceiveQueue _receiver;
        SendQueue _sender;
        SystemByteGenerator _systemByteGenerator;
        ISecsGemLogger _logger;
        public int T6 { get; set; }
        public ControlMessageGenerator(ReceiveQueue receiver, SendQueue sender, int T6, SystemByteGenerator systemByteGenerator, ISecsGemLogger logger)
        {
            _receiver = receiver;
            _sender = sender;
            _systemByteGenerator = systemByteGenerator;
            this.T6 = T6;
            _logger = logger;
        }

        public int NewSystemByte => _systemByteGenerator.New();

        /// <summary>
        /// Select
        /// </summary>
        /// <returns></returns>
        public async Task<SECSMessage> SelectReq()
        {
            byte[] bytes = new byte[14] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 1, 0, 0, 0, 0 };
            byte[] systemBytes = BitConverter.GetBytes(NewSystemByte);
            if (BitConverter.IsLittleEndian) Array.Reverse(systemBytes);
            Buffer.BlockCopy(systemBytes, 0, bytes, 10, 4);
            
            _logger.Info("Select Req...");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T6: T6);
            var result = await trans.ControlMessageRequest(bytes);
            return result;
        }

        public async Task SelectRsp(byte[] selectReq, bool isAccept)
        {
            byte[] selectRsp = new byte[14];
            Buffer.BlockCopy(selectReq, 0, selectRsp, 4, 10);
            selectRsp[3] = 10;
            selectRsp[7] = (byte)(isAccept ? 0 : 1);
            selectRsp[9] = 2;   //SType
            await Task.Run(() => _sender.Enqueue(selectRsp));
        }

        /// <summary>
        /// Deselect, HSMS-SS do not use deselect
        /// </summary>
        /// <returns></returns>
        public async Task<SECSMessage> DeselectReq()
        {
            byte[] bytes = new byte[14] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 3, 0, 0, 0, 0 };
            byte[] systemBytes = BitConverter.GetBytes(NewSystemByte);
            if (BitConverter.IsLittleEndian) Array.Reverse(systemBytes);
            Buffer.BlockCopy(systemBytes, 0, bytes, 10, 4);

            _logger.Info("Deselect Req...");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T6: T6);
            var result = await trans.ControlMessageRequest(bytes);
            return result;
        }

        public async Task DeselectRsp(byte[] selectReq, bool isAccept)
        {
            byte[] selectRsp = new byte[14];
            Buffer.BlockCopy(selectReq, 0, selectRsp, 4, 10);
            selectRsp[3] = 10;
            selectRsp[7] = (byte)(isAccept ? 0 : 1);
            selectRsp[9] = 4;   //SType
            await Task.Run(() => _sender.Enqueue(selectRsp));
        }

        /// <summary>
        /// Liketest
        /// </summary>
        /// <returns></returns>
        public async Task<SECSMessage> LiketestReq()
        {
            byte[] bytes = new byte[14] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 5, 0, 0, 0, 0 };
            byte[] systemBytes = BitConverter.GetBytes(NewSystemByte);
            if (BitConverter.IsLittleEndian) Array.Reverse(systemBytes);
            Buffer.BlockCopy(systemBytes, 0, bytes, 10, 4);

            _logger.Info("Liketest Req...");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger, T6: T6);
            var result = await trans.ControlMessageRequest(bytes);
            return result;
        }

        public async Task LiketestRsp(byte[] selectReq, bool isAccept)
        {
            byte[] selectRsp = new byte[14];
            Buffer.BlockCopy(selectReq, 0, selectRsp, 4, 10);
            selectRsp[3] = 10;
            selectRsp[7] = (byte)(isAccept ? 0 : 1);
            selectRsp[9] = 6;   //SType
            await Task.Run(() => _sender.Enqueue(selectRsp));
        }

        /// <summary>
        /// Reject
        /// </summary>
        /// <returns></returns>
        public async Task RejectReq(byte[] requestHeader, byte reasonCode)
        {
            //                            0  1  2   3                 4                 5  6  7  8  9 10 11 12 13
            byte[] bytes = new byte[14] { 0, 0, 0, 10, requestHeader[0], requestHeader[1], 0, 0, 0, 7, 0, 0, 0, 0 };

            bytes[7] = reasonCode;
            switch (reasonCode)
            {
                case 1: //SType not supported, set byte 2 as req SType
                    bytes[6] = requestHeader[5];
                    break;
                case 2: //PType not supported, set byte 2 as req PType
                    bytes[6] = requestHeader[4];
                    break;
                case 3: //Transaction not open (ReceiveQueue not regist)
                case 4: //Entity not selected
                    if (requestHeader[5] != 0) //req SType is not 0 means ctrl message, set as SType
                        bytes[6] = requestHeader[5];
                    else //req SType = 0 means data message, set as PType                 
                        bytes[6] = requestHeader[4];
                    break;
                
                default: //subsidiary standard-specific reason or local entity-specific reason
                    if (requestHeader[5] != 0) //req SType is not 0 means ctrl message, set as SType
                        bytes[6] = requestHeader[5];
                    else //req SType = 0 means data message, set as PType                 
                        bytes[6] = requestHeader[4];
                    break;
            }
            //copy system byte
            Buffer.BlockCopy(requestHeader, 6, bytes, 10, 4);

            _logger.Info("Reject Req...");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger);
            await trans.ControlMessageSendOnly(bytes);
        }

        public async Task SeparateReq()
        {
            byte[] bytes = new byte[14] { 0, 0, 0, 10, 255, 255, 0, 0, 0, 9, 0, 0, 0, 0 };
            byte[] systemBytes = BitConverter.GetBytes(NewSystemByte);
            if (BitConverter.IsLittleEndian) Array.Reverse(systemBytes);
            Buffer.BlockCopy(systemBytes, 0, bytes, 10, 4);

            _logger.Info("Separate Req...");
            SECSTransaction trans = new SECSTransaction(_receiver, _sender, _logger);
            await trans.ControlMessageSendOnly(bytes);
        }
    }
}
