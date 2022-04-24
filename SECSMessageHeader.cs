using System;

namespace JSecs
{
    internal class SECSMessageHeader
    {
        internal ushort SessionID
        {
            get => (ushort)(Bytes[0] << 8 + Bytes[1]);
            set
            {
                if (value > 32767) throw new Exception("HSMS-SS support max deviceId is 32767");
                Bytes[0] = (byte)(value >> 8);
                Bytes[1] = (byte)value;
            }
        }
        internal ushort DeviceID { get => SessionID; set => SessionID = value; }
        internal bool Wbit
        {
            get => Bytes[2] >> 7 == 1;
            set //=> var value ? (Bytes[2] |= 128) : (Bytes[2] &= 127);
            {
                if (value)
                    Bytes[2] |= 128;
                else
                    Bytes[2] &= 127;
            }
        }
        internal byte Stream 
        { 
            get => (byte)(Bytes[2] & 127);
            set 
            {
                if (value > 127 || value == 0) throw new Exception("Stream should be between 1~127");
                Bytes[2] = (byte)(Bytes[2] & 128 + value);
            }
        }
        internal byte Function { get => Bytes[3]; set => Bytes[3] = value; }
        internal byte PType { get => Bytes[4]; set => Bytes[4] = value; }
        internal byte SType { get => Bytes[5]; set => Bytes[5] = value; }
        internal int SystemByte
        {
            get => (Bytes[6] << 24) + (Bytes[7] << 16) + (Bytes[8] << 8) + Bytes[9];
            set //only set when CreateErrorMsg
            {
                Bytes[6] = (byte)(value >> 24);
                Bytes[7] = (byte)(value >> 16);
                Bytes[8] = (byte)(value >> 8);
                Bytes[9] = (byte)value;
            }
        }
        internal byte[] Bytes { get; }

        internal SECSMessageHeader(byte[] header)
        {
            if (header.Length != 10) throw new Exception("SECS Message header not 10 bytes");
            Bytes = header;
        }

        internal SECSMessageHeader(SECSMessage msg)
        {
            byte[] bytes = new byte[10];
            Buffer.BlockCopy(msg.Bytes, 0, bytes, 0, 10);
            Bytes = bytes;
        }

    }
}