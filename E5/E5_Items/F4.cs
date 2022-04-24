using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class F4 : ValueBase
    {
        public float Value { get; set; }
        internal static int Length { get => 4; }
        public static SECSType SECSType => SECSType.F4;
        //this function is for indexer and operator overloading
        private F4()
        {
            Value = 0f;
        }

        public F4(float f)
        {
            Value = f;
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
        //public F4(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length) 
        //{
        //    SECSType = SECSType.F4;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 4;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to F4");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<float>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        float value = BitConverter.ToSingle(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static F4[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 4;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to F4");

            var itemCount = length / dataLen;
            F4[] items = new F4[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                float value = BitConverter.ToSingle(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }


        internal static byte[] Encode(SECSValueItem<F4> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.F4) throw new Exception("SECSItem invalid when encode F4");

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
        public static implicit operator F4(float value) => new F4(value);   //any integer or uinteger can transfer into float by system implicit if trans as parameter

        //[2]explicit class transfer [FROM]   
        public static explicit operator F4(decimal value) => new F4((float)value); 
        public static explicit operator F4(double value) => new F4((float)value);

        //[3]implicit class transfer [TO]
        public static implicit operator double(F4 value) => value.Value; 
        public static implicit operator float(F4 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(F4 value) => (byte)value.Value;
        public static explicit operator char(F4 value) => (char)value.Value;
        public static explicit operator decimal(F4 value) => (decimal)value.Value;
        public static explicit operator int(F4 value) => (int)value.Value;
        public static explicit operator long(F4 value) => (long)value.Value;
        public static explicit operator sbyte(F4 value) => (sbyte)value.Value;
        public static explicit operator short(F4 value) => (short)value.Value;
        public static explicit operator uint(F4 value) => (uint)value.Value;
        public static explicit operator ulong(F4 value) => (ulong)value.Value;
        public static explicit operator ushort(F4 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator F8(F4 value) => value.Value;
        public static implicit operator SECSValueItem<F4>(F4 value) => new SECSValueItem<F4>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(F4 value) => (byte)value.Value;
        public static implicit operator U1(F4 value) => (byte)value.Value;
        public static explicit operator U2(F4 value) => (ushort)value.Value;
        public static explicit operator U4(F4 value) => (uint)value.Value;
        public static explicit operator U8(F4 value) => (ulong)value.Value;
        public static explicit operator I1(F4 value) => (sbyte)value.Value;
        public static explicit operator I2(F4 value) => (short)value.Value;
        public static explicit operator I4(F4 value) => (int)value.Value;
        public static explicit operator I8(F4 value) => (long)value.Value;
        #endregion


        #region OPERATOR OVERLOADING
        //operator overloading +
        //public static F4 operator +(F4 value1, double value2) => new F4((float)(value1._valueList[0] + value2));
        //public static F4 operator +(F4 value1, F8 value2) => new F8((float)(value1._valueList[0] + (float)value2.valueList[0]));


        #endregion
    }
}
