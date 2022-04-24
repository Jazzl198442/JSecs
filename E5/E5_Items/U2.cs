using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class U2 : ValueBase
    {
        public ushort Value { get; set; }
        internal static int Length { get => 2; }
        public static SECSType SECSType => SECSType.U2;
        //this function is for indexer and operator overloading
        private U2()
        {
            Value = 0;
        }

        public U2(ushort ush)
        {
            Value = ush;
        }

        public static string StringFormat { get; set; }

        public override string ToString(string format = "")
        {
            if (format == string.Empty)
                return Value.ToString(StringFormat);
            else
                return Value.ToString(format);
        }
        ////this function is for decoder
        //public U2(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.U2;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 2;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to U2");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<ushort>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        ushort value = BitConverter.ToUInt16(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        //this function is for decoder
        internal static U2[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 2;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to U2");

            var itemCount = length / dataLen;
            U2[] items = new U2[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                ushort value = BitConverter.ToUInt16(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<U2> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.U2) throw new Exception("SECSItem invalid when encode U2");

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
        public static implicit operator U2(ushort value) => new U2(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator U2(decimal value) => new U2((ushort)value);
        public static explicit operator U2(double value) => new U2((ushort)value);
        public static explicit operator U2(float value) => new U2((ushort)value);
        public static explicit operator U2(int value) => new U2((ushort)value);
        public static explicit operator U2(long value) => new U2((ushort)value);
        public static explicit operator U2(sbyte value) => new U2((ushort)value);
        public static explicit operator U2(short value) => new U2((ushort)value);
        public static explicit operator U2(uint value) => new U2((ushort)value);
        public static explicit operator U2(ulong value) => new U2((ushort)value);

        //[3]implicit class transfer [TO]
        public static implicit operator decimal(U2 value) => value.Value;
        public static implicit operator double(U2 value) => value.Value;
        public static implicit operator float(U2 value) => value.Value;
        public static implicit operator int(U2 value) => value.Value;
        public static implicit operator long(U2 value) => value.Value;
        public static implicit operator uint(U2 value) => value.Value;
        public static implicit operator ulong(U2 value) => value.Value;
        public static implicit operator ushort(U2 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(U2 value) => (byte)value.Value;
        public static explicit operator char(U2 value) => (char)value.Value;
        public static explicit operator sbyte(U2 value) => (sbyte)value.Value;
        public static explicit operator short(U2 value) => (short)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator U4(U2 value) => value.Value;
        public static implicit operator U8(U2 value) => value.Value;
        public static implicit operator I4(U2 value) => value.Value; 
        public static implicit operator I8(U2 value) => value.Value;
        public static implicit operator F4(U2 value) => value.Value;
        public static implicit operator F8(U2 value) => value.Value;
        public static implicit operator SECSValueItem<U2>(U2 value) => new SECSValueItem<U2>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(U2 value) => (byte)value.Value;
        public static explicit operator I1(U2 value) => (sbyte)value.Value; 
        public static explicit operator I2(U2 value) => (short)value.Value;
        public static explicit operator U1(U2 value) => (byte)value.Value;

        #endregion
    }
}
