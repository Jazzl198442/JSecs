using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class B : ValueBase
    {
        public byte Value { get; set; }
        internal static int Length { get => 1; }
        public static SECSType SECSType => SECSType.B;
        //this function is for indexer and operator overloading
        private B()
        {
            Value = 0;
        }

        public B(byte b)
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

        //this function is for decoder
        //public B(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.B;
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

        internal static B[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 1;
            var itemCount = length / dataLen;
            B[] items = new B[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                byte value = bytes[offset + i];
                items[i] = value;
            }

            offset += length;
            return items;
        }


        internal static byte[] Encode(SECSValueItem<B> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.B) throw new Exception("SECSItem invalid when encode B");

            byte[] bytes = new byte[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                bytes[i] = items[i];
            }
            return bytes;
        }

        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator B(byte value) => new B(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator B(char value) => new B((byte)value);
        public static explicit operator B(decimal value) => new B((byte)value);
        public static explicit operator B(double value) => new B((byte)value);
        public static explicit operator B(float value) => new B((byte)value);
        public static explicit operator B(int value) => new B((byte)value);
        public static explicit operator B(long value) => new B((byte)value);
        public static explicit operator B(sbyte value) => new B((byte)value);
        public static explicit operator B(short value) => new B((byte)value);
        public static explicit operator B(uint value) => new B((byte)value);
        public static explicit operator B(ulong value) => new B((byte)value);
        public static explicit operator B(ushort value) => new B((byte)value);

        //[3]implicit class transfer [TO]
        public static implicit operator byte(B value) => value.Value;
        public static implicit operator decimal(B value) => value.Value;
        public static implicit operator double(B value) => value.Value;
        public static implicit operator float(B value) => value.Value;
        public static implicit operator int(B value) => value.Value;
        public static implicit operator long(B value) => value.Value;
        public static implicit operator short(B value) => value.Value;
        public static implicit operator uint(B value) => value.Value;
        public static implicit operator ulong(B value) => value.Value;
        public static implicit operator ushort(B value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator char(B value) => (char)value.Value;
        public static explicit operator sbyte(B value) => (sbyte)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator U1(B value) => value.Value;
        public static implicit operator U2(B value) => value.Value;
        public static implicit operator U4(B value) => value.Value;
        public static implicit operator U8(B value) => value.Value;
        public static implicit operator I2(B value) => value.Value;
        public static implicit operator I4(B value) => value.Value;
        public static implicit operator I8(B value) => value.Value;
        public static implicit operator F4(B value) => value.Value;
        public static implicit operator F8(B value) => value.Value;
        public static implicit operator SECSValueItem<B>(B value) => new SECSValueItem<B>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator I1(B value) => (sbyte)value.Value;

        #endregion



    }
}
