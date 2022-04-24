using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    //This class is for anaylze Data SECSMessage bytes without header
    class SECSEncoder
    {
        public static byte[] Encode(SECSItem item)
        {
            int headerLen = 1;
            int dataLen = item.DataLength;
            if (dataLen > 255)
            {
                if (dataLen > 65535)
                {
                    if (dataLen > 16777215)
                    {
                        throw new Exception($"SECSItem length over limit: {dataLen}");
                    }
                    headerLen = 3;
                }
                headerLen = 2;
            }

            byte[] header = new byte[headerLen + 1];
            header[0] = (byte)(((byte)(item.SECSType) << 2) + headerLen);
            switch (headerLen)
            {
                case 1:
                    header[1] = (byte)dataLen;
                    break;
                case 2:
                    header[1] = (byte)(dataLen >> 8);
                    header[2] = (byte)dataLen;
                    break;
                case 3:
                    header[1] = (byte)(dataLen >> 16);
                    header[2] = (byte)(dataLen >> 8);
                    header[3] = (byte)dataLen;
                    break;
            }

            byte[] body = item.Encode();

            byte[] bytes = new byte[header.Length + body.Length];
            Buffer.BlockCopy(header, 0, bytes, 0, header.Length);
            Buffer.BlockCopy(body, 0, bytes, header.Length, body.Length);
            return bytes;
        }

    }
}
