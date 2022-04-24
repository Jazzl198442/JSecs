using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class I4 : ValueBase
    {
        public int Value { get; set; }
        internal static int Length { get => 4; }
        public static SECSType SECSType => SECSType.I4;
        //this function is for indexer and operator overloading
        private I4()
        {
            Value = 0;
        }

        public I4(int i)
        {
            Value = i;
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
        //public I4(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.I4;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 4;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to I4");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<int>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        int value = BitConverter.ToInt32(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static I4[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 4;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to I4");

            var itemCount = length / dataLen;
            I4[] items = new I4[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                int value = BitConverter.ToInt32(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<I4> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.I4) throw new Exception("SECSItem invalid when encode I4");

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
        public static implicit operator I4(int value) => new I4(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator I4(char value) => new I4((short)value);
        public static explicit operator I4(decimal value) => new I4((short)value);
        public static explicit operator I4(double value) => new I4((short)value);
        public static explicit operator I4(float value) => new I4((short)value);
        public static explicit operator I4(long value) => new I4((short)value);
        public static explicit operator I4(uint value) => new I4((short)value);
        public static explicit operator I4(ulong value) => new I4((short)value);

        //[3]implicit class transfer [TO]
        public static implicit operator float(I4 value) => value.Value;
        public static implicit operator double(I4 value) => value.Value;
        public static implicit operator decimal(I4 value) => value.Value;
        public static implicit operator int(I4 value) => value.Value;
        public static implicit operator long(I4 value) => value.Value;
        
        //[4]explicit class transfer [TO]
        public static explicit operator byte(I4 value) => (byte)value.Value;
        public static explicit operator char(I4 value) => (char)value.Value;
        public static explicit operator sbyte(I4 value) => (sbyte)value.Value;
        public static explicit operator short(I4 value) => (short)value.Value; 
        public static explicit operator uint(I4 value) => (uint)value.Value;
        public static explicit operator ulong(I4 value) => (ulong)value.Value;
        public static explicit operator ushort(I4 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator I8(I4 value) => value.Value;
        public static implicit operator F4(I4 value) => value.Value;
        public static implicit operator F8(I4 value) => value.Value;
        public static implicit operator SECSValueItem<I4>(I4 value) => new SECSValueItem<I4>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(I4 value) => (byte)value.Value;
        public static implicit operator U1(I4 value) => (byte)value.Value;
        public static explicit operator U2(I4 value) => (ushort)value.Value;
        public static explicit operator U4(I4 value) => (uint)value.Value;
        public static explicit operator U8(I4 value) => (ulong)value.Value;
        public static explicit operator I1(I4 value) => (sbyte)value.Value;
        public static explicit operator I2(I4 value) => (short)value.Value;
        #endregion


    }
}
