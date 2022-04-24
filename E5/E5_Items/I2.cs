using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class I2 : ValueBase
    {
        public short Value { get; set; }
        internal static int Length { get => 2; }
        public static SECSType SECSType => SECSType.I2;
        //this function is for indexer and operator overloading
        private I2()
        {
            Value = 0;
        }

        public I2(short sh)
        {
            Value = sh;
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
        //public I2(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.I2;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 2;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to I2");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<short>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        short value = BitConverter.ToInt16(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static I2[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 2;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to I2");

            var itemCount = length / dataLen;
            I2[] items = new I2[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                short value = BitConverter.ToInt16(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<I2> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.I2) throw new Exception("SECSItem invalid when encode I2");

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
        public static implicit operator I2(short value) => new I2(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator I2(char value) => new I2((short)value);
        public static explicit operator I2(decimal value) => new I2((short)value);
        public static explicit operator I2(double value) => new I2((short)value);
        public static explicit operator I2(float value) => new I2((short)value);
        public static explicit operator I2(int value) => new I2((short)value);
        public static explicit operator I2(long value) => new I2((short)value);
        public static explicit operator I2(uint value) => new I2((short)value);
        public static explicit operator I2(ulong value) => new I2((short)value);
        public static explicit operator I2(ushort value) => new I2((short)value);

        //[3]implicit class transfer [TO]
        public static implicit operator float(I2 value) => value.Value;
        public static implicit operator double(I2 value) => value.Value;
        public static implicit operator decimal(I2 value) => value.Value;
        public static implicit operator int(I2 value) => value.Value;
        public static implicit operator long(I2 value) => value.Value;
        public static implicit operator short(I2 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(I2 value) => (byte)value.Value;
        public static explicit operator char(I2 value) => (char)value.Value;
        public static explicit operator sbyte(I2 value) => (sbyte)value.Value;
        public static explicit operator uint(I2 value) => (uint)value.Value;
        public static explicit operator ulong(I2 value) => (ulong)value.Value;
        public static explicit operator ushort(I2 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator I4(I2 value) => value.Value;
        public static implicit operator I8(I2 value) => value.Value;
        public static implicit operator F4(I2 value) => value.Value;
        public static implicit operator F8(I2 value) => value.Value;
        public static implicit operator SECSValueItem<I2>(I2 value) => new SECSValueItem<I2>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(I2 value) => (byte)value.Value;
        public static implicit operator U1(I2 value) => (byte)value.Value; 
        public static explicit operator U2(I2 value) => (ushort)value.Value;
        public static explicit operator U4(I2 value) => (uint)value.Value;
        public static explicit operator U8(I2 value) => (ulong)value.Value;
        public static explicit operator I1(I2 value) => (sbyte)value.Value;
        
        #endregion


    }
}
