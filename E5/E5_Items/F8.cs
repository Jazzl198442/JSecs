using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JSecs.E5
{
    public class F8 : ValueBase
    {
        public double Value { get; set; }
        internal static int Length => 8;
        public static SECSType SECSType => SECSType.F8;
        //this function is for indexer and operator overloading
        private F8()
        {
            Value = 0;
        }

        public F8(double d)
        {
            Value = d;
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
        //public F8(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{

        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 8;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to F8");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<double>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        double value = BitConverter.ToDouble(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static F8[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 8;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to F8");

            var itemCount = length / dataLen;
            F8[] items = new F8[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                double value = BitConverter.ToDouble(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<F8> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.F8) throw new Exception("SECSItem invalid when encode F8");

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
        public static implicit operator F8(double value) => new F8(value);   //any integer or uinteger can transfer into double by system implicit if trans as parameter

        //[2]explicit class transfer [FROM]   
        public static explicit operator F8(decimal value) => new F8((double)value);

        //[3]implicit class transfer [TO]
        public static implicit operator double(F8 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(F8 value) => (byte)value.Value;
        public static explicit operator char(F8 value) => (char)value.Value;
        public static explicit operator decimal(F8 value) => (decimal)value.Value;
        public static explicit operator float(F8 value) => (float)value.Value;
        public static explicit operator int(F8 value) => (int)value.Value;
        public static explicit operator long(F8 value) => (long)value.Value;
        public static explicit operator sbyte(F8 value) => (sbyte)value.Value;
        public static explicit operator short(F8 value) => (short)value.Value;
        public static explicit operator uint(F8 value) => (uint)value.Value;
        public static explicit operator ulong(F8 value) => (ulong)value.Value;
        public static explicit operator ushort(F8 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        //public static implicit operator SECSValueItem<F8>(F8 value) => new SECSValueItem<F8>(new F8[1] { value });
        public static implicit operator SECSValueItem<F8>(F8 value) => new SECSValueItem<F8>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(F8 value) => (byte)value.Value;
        public static implicit operator F4(F8 value) => (float)value.Value;
        public static implicit operator U1(F8 value) => (byte)value.Value;
        public static explicit operator U2(F8 value) => (ushort)value.Value;
        public static explicit operator U4(F8 value) => (uint)value.Value;
        public static explicit operator U8(F8 value) => (ulong)value.Value;
        public static explicit operator I1(F8 value) => (sbyte)value.Value;
        public static explicit operator I2(F8 value) => (short)value.Value;
        public static explicit operator I4(F8 value) => (int)value.Value;
        public static explicit operator I8(F8 value) => (long)value.Value;

        #endregion


    }
}
