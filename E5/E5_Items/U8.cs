using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class U8 : ValueBase
    {
        public ulong Value { get; set; }
        internal static int Length => 8;
        public static SECSType SECSType => SECSType.U8;
        //this function is for indexer and operator overloading
        private U8()
        {
            Value = 0;
        }

        public U8(ulong ul)
        {
            Value = ul;
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
        //public U8(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.U8;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 8;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to U8");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<ulong>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        ulong value = BitConverter.ToUInt64(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static U8[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 8;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to U8");

            var itemCount = length / dataLen;
            U8[] items = new U8[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                ulong value = BitConverter.ToUInt64(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }
        internal static byte[] Encode(SECSValueItem<U8> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.U8) throw new Exception("SECSItem invalid when encode U8");

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
        public static implicit operator U8(ulong value) => new U8(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator U8(decimal value) => new U8((ulong)value);
        public static explicit operator U8(double value) => new U8((ulong)value);
        public static explicit operator U8(float value) => new U8((ulong)value);
        public static explicit operator U8(int value) => new U8((ulong)value);
        public static explicit operator U8(long value) => new U8((ulong)value);
        public static explicit operator U8(sbyte value) => new U8((ulong)value);
        public static explicit operator U8(short value) => new U8((ulong)value);

        //[3]implicit class transfer [TO]
        public static implicit operator decimal(U8 value) => value.Value;
        public static implicit operator double(U8 value) => value.Value;
        public static implicit operator float(U8 value) => value.Value;
        public static implicit operator ulong(U8 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(U8 value) => (byte)value.Value;
        public static explicit operator char(U8 value) => (char)value.Value;
        public static explicit operator int(U8 value) => (int)value.Value;
        public static explicit operator long(U8 value) => (long)value.Value;
        public static explicit operator sbyte(U8 value) => (sbyte)value.Value;
        public static explicit operator short(U8 value) => (short)value.Value;
        public static explicit operator ushort(U8 value) => (ushort)value.Value;
        public static explicit operator uint(U8 value) => (uint)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator F4(U8 value) => value.Value;
        public static implicit operator F8(U8 value) => value.Value;
        public static implicit operator SECSValueItem<U8>(U8 value) => new SECSValueItem<U8>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(U8 value) => (byte)value.Value;
        public static explicit operator I1(U8 value) => (sbyte)value.Value;
        public static explicit operator I2(U8 value) => (short)value.Value;
        public static explicit operator I4(U8 value) => (int)value.Value;
        public static explicit operator I8(U8 value) => (long)value.Value;
        public static explicit operator U1(U8 value) => (byte)value.Value;
        public static explicit operator U2(U8 value) => (ushort)value.Value;
        public static explicit operator U4(U8 value) => (uint)value.Value;
        

        #endregion

    }
}
