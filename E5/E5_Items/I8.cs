using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class I8 : ValueBase
    {
        public long Value { get; set; }
        internal static int Length { get => 8; }
        public static SECSType SECSType => SECSType.I8;
        //this function is for indexer and operator overloading
        private I8()
        {
            Value = 0;
        }

        public I8(long l)
        {
            Value = l;
        }

        public static string StringFormat { get; set; }

        public override string ToString(string format = "")
        {
            if (format == string.Empty)
                return Value.ToString(StringFormat);
            else
                return Value.ToString(format);
        }
        //this function is for decoder
        //public I8(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.I8;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 8;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to I8");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<long>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        long value = BitConverter.ToInt64(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static I8[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 8;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to I8");

            var itemCount = length / dataLen;
            I8[] items = new I8[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                long value = BitConverter.ToInt64(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<I8> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.I8) throw new Exception("SECSItem invalid when encode I8");

            int len = Length;
            byte[] bytes = new byte[items.Length * len];
            byte[] temp = new byte[len];
            for (int i = 0; i < items.Length; i++)
            {
                temp = BitConverter.GetBytes(items[i]);
                if (BitConverter.IsLittleEndian) Array.Reverse(temp);
                Buffer.BlockCopy(temp, 0, bytes, i * len, len);
            }

            return bytes;

        }


        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator I8(long value) => new I8(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator I8(char value) => new I8((short)value);
        public static explicit operator I8(decimal value) => new I8((short)value);
        public static explicit operator I8(double value) => new I8((short)value);
        public static explicit operator I8(float value) => new I8((short)value);
        public static explicit operator I8(ulong value) => new I8((short)value);

        //[3]implicit class transfer [TO]
        public static implicit operator float(I8 value) => value.Value;
        public static implicit operator double(I8 value) => value.Value;
        public static implicit operator decimal(I8 value) => value.Value;
        public static implicit operator long(I8 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(I8 value) => (byte)value.Value;
        public static explicit operator char(I8 value) => (char)value.Value;
        public static explicit operator int(I8 value) => (int)value.Value;
        public static explicit operator sbyte(I8 value) => (sbyte)value.Value;
        public static explicit operator short(I8 value) => (short)value.Value;
        public static explicit operator uint(I8 value) => (uint)value.Value;
        public static explicit operator ulong(I8 value) => (ulong)value.Value;
        public static explicit operator ushort(I8 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator F4(I8 value) => value.Value;
        public static implicit operator F8(I8 value) => value.Value;
        public static implicit operator SECSValueItem<I8>(I8 value) => new SECSValueItem<I8>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(I8 value) => (byte)value.Value;
        public static explicit operator U1(I8 value) => (byte)value.Value;
        public static explicit operator U2(I8 value) => (ushort)value.Value;
        public static explicit operator U4(I8 value) => (uint)value.Value;
        public static explicit operator U8(I8 value) => (ulong)value.Value;
        public static explicit operator I1(I8 value) => (sbyte)value.Value;
        public static explicit operator I2(I8 value) => (short)value.Value;
        public static explicit operator I4(I8 value) => (int)value.Value;
        #endregion


    }
}
