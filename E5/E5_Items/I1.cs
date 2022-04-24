using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class I1 : ValueBase
    {
        public sbyte Value { get; set; }
        internal static int Length { get => 1; }
        public static SECSType SECSType => SECSType.I1;
        //this function is for indexer and operator overloading
        private I1()
        {
            Value = 0;
        }

        public I1(sbyte sb)
        {
            Value = sb;
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
        //public I1(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.I1;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 1;
        //    var itemCount = length / dataLen;
        //    _valueList = new List<sbyte>(itemCount);
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        sbyte value = (sbyte)bytes[offset + i];
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        //this function is for decoder
        internal static I1[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 1;
            var itemCount = length / dataLen;
            I1[] items = new I1[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                sbyte value = (sbyte)bytes[offset + i];
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<I1> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.I1) throw new Exception("SECSItem invalid when encode I1");

            int len = Length;
            byte[] bytes = new byte[items.Length * len];
            for (int i = 0; i < items.Length; i++)
            {
                bytes[i] = BitConverter.GetBytes(items[i])[0];
            }

            return bytes;

        }


        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator I1(sbyte value) => new I1(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator I1(byte value) => new I1((sbyte)value);
        public static explicit operator I1(char value) => new I1((sbyte)value);
        public static explicit operator I1(decimal value) => new I1((sbyte)value);
        public static explicit operator I1(double value) => new I1((sbyte)value);
        public static explicit operator I1(float value) => new I1((sbyte)value);
        public static explicit operator I1(int value) => new I1((sbyte)value);
        public static explicit operator I1(long value) => new I1((sbyte)value);
        public static explicit operator I1(short value) => new I1((sbyte)value);
        public static explicit operator I1(uint value) => new I1((sbyte)value);
        public static explicit operator I1(ulong value) => new I1((sbyte)value);
        public static explicit operator I1(ushort value) => new I1((sbyte)value);
        
        //[3]implicit class transfer [TO]
        public static implicit operator decimal(I1 value) => value.Value;
        public static implicit operator double(I1 value) => value.Value;
        public static implicit operator float(I1 value) => value.Value; 
        public static implicit operator int(I1 value) => value.Value;
        public static implicit operator long(I1 value) => value.Value;
        public static implicit operator sbyte(I1 value) => value.Value;
        public static implicit operator short(I1 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(I1 value) => (byte)value.Value;
        public static explicit operator char(I1 value) => (char)value.Value;
        public static explicit operator uint(I1 value) => (uint)value.Value;
        public static explicit operator ulong(I1 value) => (ulong)value.Value;
        public static explicit operator ushort(I1 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator I2(I1 value) => value.Value;
        public static implicit operator I4(I1 value) => value.Value;
        public static implicit operator I8(I1 value) => value.Value;
        public static implicit operator F4(I1 value) => value.Value;
        public static implicit operator F8(I1 value) => value.Value;
        public static implicit operator SECSValueItem<I1>(I1 value) => new SECSValueItem<I1>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(I1 value) => (byte)value.Value;
        public static explicit operator U1(I1 value) => (byte)value.Value;
        public static explicit operator U2(I1 value) => (ushort)value.Value;
        public static explicit operator U4(I1 value) => (uint)value.Value;
        public static explicit operator U8(I1 value) => (ulong)value.Value;

        #endregion


    }
}
