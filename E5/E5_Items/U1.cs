using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class U1 : ValueBase
    {
        public byte Value { get; set; }
        internal static int Length { get => 1; }
        public static SECSType SECSType => SECSType.U1;
        //this function is for indexer and operator overloading
        private U1()
        {
            Value = 0;
        }

        public U1(byte b)
        {
            Value = b;
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
        //public U1(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.U1;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 1;
        //    var itemCount = length / dataLen;
        //    _valueList = new List<byte>(itemCount);
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        byte value = bytes[offset + i];
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}



        //this function is for decoder
        internal static U1[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 1;
            var itemCount = length / dataLen;
            U1[] items = new U1[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                byte value = bytes[offset + i];
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<U1> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.U1) throw new Exception("SECSItem invalid when encode U1");

            byte[] bytes = new byte[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                bytes[i] = items[i];
            }
            return bytes;

        }

        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator U1(byte value) => new U1(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator U1(char value) => new U1((byte)value);
        public static explicit operator U1(decimal value) => new U1((byte)value);
        public static explicit operator U1(double value) => new U1((byte)value);
        public static explicit operator U1(float value) => new U1((byte)value);
        public static explicit operator U1(int value) => new U1((byte)value);
        public static explicit operator U1(long value) => new U1((byte)value);
        public static explicit operator U1(sbyte value) => new U1((byte)value);
        public static explicit operator U1(short value) => new U1((byte)value);
        public static explicit operator U1(uint value) => new U1((byte)value);
        public static explicit operator U1(ulong value) => new U1((byte)value);
        public static explicit operator U1(ushort value) => new U1((byte)value);

        //[3]implicit class transfer [TO]
        public static implicit operator byte(U1 value) => value.Value;
        public static implicit operator decimal(U1 value) => value.Value;
        public static implicit operator double(U1 value) => value.Value;
        public static implicit operator float(U1 value) => value.Value;
        public static implicit operator int(U1 value) => value.Value;
        public static implicit operator long(U1 value) => value.Value;
        public static implicit operator short(U1 value) => value.Value;
        public static implicit operator uint(U1 value) => value.Value;
        public static implicit operator ulong(U1 value) => value.Value;
        public static implicit operator ushort(U1 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator char(U1 value) => (char)value.Value;
        public static explicit operator sbyte(U1 value) => (sbyte)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator B(U1 value) => value.Value;
        public static implicit operator U2(U1 value) => value.Value;
        public static implicit operator U4(U1 value) => value.Value;
        public static implicit operator U8(U1 value) => value.Value;
        public static implicit operator I2(U1 value) => value.Value;
        public static implicit operator I4(U1 value) => value.Value;
        public static implicit operator I8(U1 value) => value.Value;
        public static implicit operator F4(U1 value) => value.Value;
        public static implicit operator F8(U1 value) => value.Value;
        public static implicit operator SECSValueItem<U1>(U1 value) => new SECSValueItem<U1>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator I1(U1 value) => (sbyte)value.Value;

        #endregion


        #region OPERATOR OVERLOADING
        //operator overloading +
        //public static U1 operator +(U1 value1, double value2) => new U1((float)(value1._valueList[0] + value2));
        //public static U1 operator +(U1 value1, F8 value2) => new F8((float)(value1._valueList[0] + (float)value2.valueList[0]));


        #endregion

    }
}
