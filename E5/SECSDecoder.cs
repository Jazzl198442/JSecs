using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    //This class is for anaylze Data SECSMessage bytes without header
    class SECSDecoder
    {
        public static SECSItem Decode(byte[] bytes, ref int offset)
        {
            var header = bytes[offset];
            var formatCode = header >> 2;
            var lol = header & 3;   //0b00000011
            int length;
            switch (lol)
            {
                case 0:
                    throw new Exception("byte LOL illegal, data format error");
                case 1:
                    length = bytes[offset + 1];
                    offset += 2;
                    break;
                case 2:
                    length = (bytes[offset + 1] << 8) + bytes[offset + 2];
                    offset += 3;
                    break;
                case 3:
                    length = (bytes[offset + 2] << 16) + (bytes[offset + 1] << 8) + bytes[offset + 2];
                    offset += 4;
                    break;
                default:
                    throw new Exception("byte LOL illegal, data format error");
            }

            var stype = (SECSType)formatCode;
            switch (stype)
            {
                case SECSType.L:  //List: length in means how many secsitem in this list, for other types, it means byte for length 
                    return new SECSListItem(bytes, ref offset, length);
                case SECSType.A:
                    return new SECSStreamItem<A>(bytes, ref offset, length, stype);
                case SECSType.JIS:
                    throw new NotImplementedException();
                case SECSType.Unicode:
                    throw new NotImplementedException();
                case SECSType.B:
                    return new SECSValueItem<B>(bytes, ref offset, length, stype);
                case SECSType.Bool:
                    return new SECSValueItem<Bool>(bytes, ref offset, length, stype);
                case SECSType.I1:
                    return new SECSValueItem<I1>(bytes, ref offset, length, stype);
                case SECSType.I2:
                    return new SECSValueItem<I2>(bytes, ref offset, length, stype);
                case SECSType.I4:
                    return new SECSValueItem<I4>(bytes, ref offset, length, stype);
                case SECSType.I8:
                    return new SECSValueItem<I8>(bytes, ref offset, length, stype);
                case SECSType.F4:
                    return new SECSValueItem<F4>(bytes, ref offset, length, stype);
                case SECSType.F8:
                    return new SECSValueItem<F8>(bytes, ref offset, length, stype);
                case SECSType.U1:
                    return new SECSValueItem<U1>(bytes, ref offset, length, stype);
                case SECSType.U2:
                    return new SECSValueItem<U2>(bytes, ref offset, length, stype);
                case SECSType.U4:
                    return new SECSValueItem<U4>(bytes, ref offset, length, stype);
                case SECSType.U8:
                    return new SECSValueItem<U8>(bytes, ref offset, length, stype);
                default:
                    throw new Exception("Invalid format code or decode error");
            }
        }
    }
}
