using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class U4 : ValueBase
    {
        public uint Value { get; set; }
        internal static int Length { get => 4; }
        public static SECSType SECSType => SECSType.U4;
        //this function is for indexer and operator overloading
        private U4()
        {
            Value = 0;
        }

        public U4(uint ui)
        {
            Value = ui;
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
        //public U4(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.U4;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 4;
        //    if (length % dataLen != 0) throw new Exception("data length invalid for decode to U4");

        //    var itemCount = length / dataLen;
        //    _valueList = new List<uint>(itemCount);
        //    byte[] buffer = new byte[dataLen];
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
        //        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
        //        uint value = BitConverter.ToUInt32(buffer, 0);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        //this function is for decoder
        internal static U4[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 4;
            if (length % dataLen != 0) throw new Exception("data length invalid for decode to U4");

            var itemCount = length / dataLen;
            U4[] items = new U4[itemCount];
            byte[] buffer = new byte[dataLen];
            for (int i = 0; i < itemCount; i++)
            {
                Buffer.BlockCopy(bytes, offset + i * dataLen, buffer, 0, dataLen);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                uint value = BitConverter.ToUInt32(buffer, 0);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<U4> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.U4) throw new Exception("SECSItem invalid when encode U4");

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
        public static implicit operator U4(uint value) => new U4(value);

        //[2]explicit class transfer [FROM]
        public static explicit operator U4(decimal value) => new U4((uint)value);
        public static explicit operator U4(double value) => new U4((uint)value);
        public static explicit operator U4(float value) => new U4((uint)value);
        public static explicit operator U4(int value) => new U4((uint)value);
        public static explicit operator U4(long value) => new U4((uint)value);
        public static explicit operator U4(sbyte value) => new U4((uint)value);
        public static explicit operator U4(short value) => new U4((uint)value);
        public static explicit operator U4(ulong value) => new U4((uint)value);

        //[3]implicit class transfer [TO]
        public static implicit operator decimal(U4 value) => value.Value;
        public static implicit operator double(U4 value) => value.Value;
        public static implicit operator float(U4 value) => value.Value;
        public static implicit operator long(U4 value) => value.Value;
        public static implicit operator uint(U4 value) => value.Value;
        public static implicit operator ulong(U4 value) => value.Value;

        //[4]explicit class transfer [TO]
        public static explicit operator byte(U4 value) => (byte)value.Value;
        public static explicit operator char(U4 value) => (char)value.Value;
        public static explicit operator int(U4 value) => (int)value.Value;
        public static explicit operator sbyte(U4 value) => (sbyte)value.Value;
        public static explicit operator short(U4 value) => (short)value.Value;
        public static explicit operator ushort(U4 value) => (ushort)value.Value;

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator U8(U4 value) => value.Value;
        public static implicit operator I8(U4 value) => value.Value;
        public static implicit operator F4(U4 value) => value.Value; 
        public static implicit operator F8(U4 value) => value.Value;
        public static implicit operator SECSValueItem<U4>(U4 value) => new SECSValueItem<U4>(value);

        //[6]SECSItem explicit class transfer [TO]
        public static explicit operator B(U4 value) => (byte)value.Value; 
        public static explicit operator I1(U4 value) => (sbyte)value.Value;
        public static explicit operator I2(U4 value) => (short)value.Value;
        public static explicit operator I4(U4 value) => (int)value.Value;
        public static explicit operator U1(U4 value) => (byte)value.Value; 
        public static explicit operator U2(U4 value) => (ushort)value.Value; 
        

        #endregion
    }
}
