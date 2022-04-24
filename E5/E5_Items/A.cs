using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JSecs.E5
{
    public class A : StreamBase //StreamBase is an empty base class for limit the T scope for SECSStreamItem<T>
    {
        //private List<byte> _valueList;

        public int Length => Value.Length;

        public static SECSType SECSType => SECSType.A;
        //public new static SECSType SECSType => SECSType.A;
        //this function is for indexer and operator overloading
        private A()
        {
            Value = string.Empty;
        }

        public A(string s)
        {
            Value = s;
        }

        ////this function is for decoder
        //internal A(byte[] bytes, ref int offset, int length) : base(bytes, ref offset, length)
        //{
        //    SECSType = SECSType.A;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
        //     |____||| |________________________| |________________________________________________... 
        //       fc  lol         length             data
        //    */

        //    Value = Encoding.ASCII.GetString(bytes, offset, length);
        //    offset += length;
        //}

        internal static A Decode(byte[] bytes, ref int offset, int length)
        {
            /*                                 now
              byte1    byte2   [byte3]  [byte4] ↓ data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            A item = Encoding.ASCII.GetString(bytes, offset, length);
            //item.SECSType = SECSType.A;
            offset += length;
            return item;
            //return A.Split(item);
        }

        public static A[] Split(A item) 
        {
            A[] items = new A[item.Length];
            for (int i = 0; i < item.Length; i++)
            {
                items[i] = item[i].ToString();
            }
            return items;
        }

        internal static byte[] Encode(SECSStreamItem<A> value)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (SECSType != SECSType.A) throw new Exception("SECSItem invalid when encode A");

            return Encoding.ASCII.GetBytes(value);
        }

        //indexer
        public char this[int index]
        {
            get { return Value[index]; }
        }

        #region IMPLEMENT OF STRING CLASS

        //func
        public object Clone() => Value.Clone();

        public int CompareTo(object? value) => Value.CompareTo(value);
        public int CompareTo(string? strB) => Value.CompareTo(strB);

        public bool Contains(char value) => Value.Contains(value);
        public bool Contains(string value) => Value.Contains(value);
        public bool Contains(char value, StringComparison comparisonType) => Value.Contains(value, comparisonType);
        public bool Contains(string value, StringComparison comparisonType) => Value.Contains(value, comparisonType);

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) => Value.CopyTo(sourceIndex, destination, destinationIndex, count);

        public bool EndsWith(char value) => Value.EndsWith(value);
        public bool EndsWith(string value) => Value.EndsWith(value);
        public bool EndsWith(string value, StringComparison comparisonType) => Value.EndsWith(value, comparisonType);
        public bool EndsWith(string value, bool ignoreCase, CultureInfo? culture) => Value.EndsWith(value, ignoreCase, culture);

        public StringRuneEnumerator EnumerateRunes() => Value.EnumerateRunes();

        public bool Equals(object? obj) => Value.Equals(obj);
        public bool Equals(string? value) => Value.Equals(value);
        public bool Equals(string? value, StringComparison comparisonType) => Value.Equals(value, comparisonType);

        public CharEnumerator GetEnumerator() => Value.GetEnumerator();

        public int GetHashCode() => Value.GetHashCode();
        public int GetHashCode(StringComparison comparisonType) => Value.GetHashCode(comparisonType);

        public Type GetType() => Value.GetType();    //??

        public TypeCode GetTypeCode() => Value.GetTypeCode();

        public int IndexOf(char value) => Value.IndexOf(value);
        public int IndexOf(string value) => Value.IndexOf(value);
        public int IndexOf(char value, int startIndex) => Value.IndexOf(value, startIndex);
        public int IndexOf(char value, StringComparison comparisonType) => Value.IndexOf(value, comparisonType);
        public int IndexOf(string value, int startIndex) => Value.IndexOf(value, startIndex);
        public int IndexOf(string value, StringComparison comparisonType) => Value.IndexOf(value, comparisonType);
        public int IndexOf(char value, int startIndex, int count) => Value.IndexOf(value, startIndex, count);
        public int IndexOf(string value, int startIndex, int count) => Value.IndexOf(value, startIndex, count);
        public int IndexOf(string value, int startIndex, StringComparison comparisonType) => Value.IndexOf(value, startIndex, comparisonType);
        public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType) => Value.IndexOf(value, startIndex, count, comparisonType);

        public int IndexOfAny(char[] anyOf) => Value.IndexOfAny(anyOf);
        public int IndexOfAny(char[] anyOf, int startIndex) => Value.IndexOfAny(anyOf, startIndex);
        public int IndexOfAny(char[] anyOf, int startIndex, int count) => Value.IndexOfAny(anyOf, startIndex, count);

        public string Insert(int startIndex, string value) => Value.Insert(startIndex, value);

        public bool IsNormalized() => Value.IsNormalized();
        public bool IsNormalized(NormalizationForm normalizationForm) => Value.IsNormalized(normalizationForm);

        public int LastIndexOf(char value) => Value.LastIndexOf(value);
        public int LastIndexOf(string value) => Value.LastIndexOf(value);
        public int LastIndexOf(char value, int startIndex) => Value.LastIndexOf(value, startIndex);
        public int LastIndexOf(string value, int startIndex) => Value.LastIndexOf(value, startIndex);
        public int LastIndexOf(string value, StringComparison comparisonType) => Value.LastIndexOf(value, comparisonType);
        public int LastIndexOf(char value, int startIndex, int count) => Value.LastIndexOf(value, startIndex, count);
        public int LastIndexOf(string value, int startIndex, int count) => Value.LastIndexOf(value, startIndex, count);
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType) => Value.LastIndexOf(value, startIndex, comparisonType);
        public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType) => Value.LastIndexOf(value, startIndex, count, comparisonType);

        public int LastIndexOfAny(char[] anyOf) => Value.LastIndexOfAny(anyOf);
        public int LastIndexOfAny(char[] anyOf, int startIndex) => Value.LastIndexOfAny(anyOf, startIndex);
        public int LastIndexOfAny(char[] anyOf, int startIndex, int count) => Value.LastIndexOfAny(anyOf, startIndex, count);

        public string Normalize() => Value.Normalize();
        public string Normalize(NormalizationForm normalizationForm) => Value.Normalize(normalizationForm);

        public string PadLeft(int totalWidth) => Value.PadLeft(totalWidth);
        public string PadLeft(int totalWidth, char paddingChar) => Value.PadLeft(totalWidth, paddingChar);

        public string PadRight(int totalWidth) => Value.PadRight(totalWidth);
        public string PadRight(int totalWidth, char paddingChar) => Value.PadRight(totalWidth, paddingChar);

        public string Remove(int startIndex) => Value.Remove(startIndex);
        public string Remove(int startIndex, int count) => Value.Remove(startIndex, count);

        public string Replace(char oldChar, char newChar) => Value.Replace(oldChar, newChar);
        public string Replace(string oldValue, string? newValue) => Value.Replace(oldValue, newValue);
        public string Replace(string oldValue, string? newValue, StringComparison comparisonType) => Value.Replace(oldValue, newValue, comparisonType);
        public string Replace(string oldValue, string? newValue, bool ignoreCase, CultureInfo culture) => Value.Replace(oldValue, newValue, ignoreCase, culture);

        public string[] Split(params char[]? separator) => Value.Split(separator);
        public string[] Split(char separator, StringSplitOptions options = StringSplitOptions.None) => Value.Split(separator, options);
        public string[] Split(char[]? separator, int count) => Value.Split(separator, count);
        public string[] Split(char[]? separator, StringSplitOptions options) => Value.Split(separator, options);
        public string[] Split(string? separator, StringSplitOptions options = StringSplitOptions.None) => Value.Split(separator, options);
        public string[] Split(string[]? separator, StringSplitOptions options) => Value.Split(separator, options);
        public string[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None) => Value.Split(separator, count, options);
        public string[] Split(char[]? separator, int count, StringSplitOptions options) => Value.Split(separator, count, options);
        public string[] Split(string? separator, int count, StringSplitOptions options = StringSplitOptions.None) => Value.Split(separator, count, options);
        public string[] Split(string[]? separator, int count, StringSplitOptions options) => Value.Split(separator, count, options);

        public bool StartsWith(char value) => Value.StartsWith(value);
        public bool StartsWith(string value) => Value.StartsWith(value);
        public bool StartsWith(string value, StringComparison comparisonType) => Value.StartsWith(value, comparisonType);
        public bool StartsWith(string value, bool ignoreCase, CultureInfo? culture) => Value.StartsWith(value, ignoreCase, culture);

        public string Substring(int startIndex) => Value.Substring(startIndex);
        public string Substring(int startIndex, int length) => Value.Substring(startIndex, length);

        public char[] ToCharArray() => Value.ToCharArray();
        public char[] ToCharArray(int startIndex, int length) => Value.ToCharArray(startIndex, length);

        public string ToLower() => Value.ToLower();
        public string ToLower(CultureInfo? culture) => Value.ToLower(culture);

        public string ToLowerInvariant() => Value.ToLowerInvariant();

        public override string ToString() => Value.ToString(); //need reset
        public string ToString(IFormatProvider provider) => Value.ToString(provider); //need reset

        public string ToUpper() => Value.ToUpper();
        public string ToUpper(CultureInfo? culture) => Value.ToUpper(culture);

        public string ToUpperInvariant() => Value.ToUpperInvariant();

        public string Trim() => Value.Trim();
        public string Trim(char trimChar) => Value.Trim(trimChar);
        public string Trim(params char[]? trimChars) => Value.Trim(trimChars);

        public string TrimEnd() => Value.TrimEnd();
        public string TrimEnd(char trimChar) => Value.TrimEnd(trimChar);
        public string TrimEnd(params char[]? trimChars) => Value.TrimEnd(trimChars);

        public string TrimStart() => Value.TrimStart();
        public string TrimStart(char trimChar) => Value.TrimStart(trimChar);
        public string TrimStart(params char[]? trimChars) => Value.TrimStart(trimChars);
        #endregion

        #region CLASS TRANSFER
        //[1]implicit class transfer [FROM]    
        public static implicit operator A(string value) => new A(value);

        //[2]explicit class transfer [FROM]
        
        //[3]implicit class transfer [TO]
        public static implicit operator string(A value) => value.Value;

        //[4]explicit class transfer [TO]

        //[5]SECSItem implicit class transfer [TO]
        
        public static implicit operator A[](A item)
        {
            A[] items = new A[item.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = item[i].ToString();
            }
            return items;
        }

        public static implicit operator A(A[] items)
        {
            StringBuilder sb = new StringBuilder(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                sb.Append(items[i]);
            }
            return new A(sb.ToString());
        }

        public static implicit operator SECSStreamItem<A>(A item)
        {
            A[] items = item;
            return new SECSStreamItem<A>(items);
        }

        //[6] SECSItem explicit class transfer [TO]


        #endregion





    }
}
