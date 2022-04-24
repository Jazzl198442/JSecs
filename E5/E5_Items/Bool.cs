using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class Bool : ValueBase
    {
        public bool Value { get; set; }
        internal static int Length { get => 1; }
        public static SECSType SECSType => SECSType.Bool;
        //this function is for indexer and operator overloading
        private Bool()
        {
            Value = false;
        }

        public Bool(bool b)
        {
            Value = b;
        }

        //public static string StringFormat { get; set; }

        public override string ToString(string format = "")
        {
            return Value.ToString();
        }
        //this function is for decoder
        //public Bool(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.Bool;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */
        //    var dataLen = 1;
        //    var itemCount = length / dataLen;
        //    _valueList = new List<bool>(itemCount);
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        bool value = BitConverter.ToBoolean(bytes, offset + i);
        //        _valueList.Add(value);
        //    }

        //    offset += length;
        //}

        internal static Bool[] Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            var dataLen = 1;
            var itemCount = length / dataLen;
            Bool[] items = new Bool[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                bool value = BitConverter.ToBoolean(bytes, offset + i);
                items[i] = value;
            }

            offset += length;
            return items;
        }

        internal static byte[] Encode(SECSValueItem<Bool> items)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (items.SECSType != SECSType.Bool) throw new Exception("SECSItem invalid when encode Bool");

            byte[] bytes = new byte[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                bytes[i] = BitConverter.GetBytes(items[i])[0];
            }

            return bytes;
        }



        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator Bool(bool value) => new Bool(value);

        //[2]explicit class transfer [FROM]

        //[3]implicit class transfer [TO]
        public static implicit operator bool(Bool value) => value.Value;

        //[4]explicit class transfer [TO]

        //[5]SECSItem implicit class transfer [TO]
        public static implicit operator SECSValueItem<Bool>(Bool value) => new SECSValueItem<Bool>(value);

        //[6]SECSItem explicit class transfer [TO]
        #endregion
    }
}
