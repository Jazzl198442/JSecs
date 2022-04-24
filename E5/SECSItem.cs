using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public abstract class SECSItem
    {
        //public List<T> _itemList;

        public byte[] Bytes { get; set; }

        public SECSItem()
        { }


        public SECSItem(byte[] bytes)
        {
            Bytes = bytes;
        }

        public SECSItem(byte[] bytes, int offset)
        {

        }

        public SECSItem(byte[] bytes, ref int offset, int length)
        {
        }

        public SECSType SECSType { get; protected set; }
        //public bool IsSECSList { get; protected set; } = false;

        internal List<SECSItem> _items;

        public SECSItem this[int index]
        {
            get
            {
                switch (this.SECSType)
                {
                    case SECSType.B:
                        var itemB = this as SECSValueItem<B>;
                        B[] arrB = new B[1] { itemB[index] };
                        return new SECSValueItem<B>(arrB);
                    case SECSType.Bool:
                        var itemBool = this as SECSValueItem<Bool>;
                        Bool[] arrBool = new Bool[1] { itemBool[index] };
                        return new SECSValueItem<Bool>(arrBool);
                    case SECSType.F4:
                        var itemF4 = this as SECSValueItem<F4>;
                        F4[] arrF4 = new F4[1] { itemF4[index] };
                        return new SECSValueItem<F4>(arrF4);
                    case SECSType.F8:
                        var itemF8 = this as SECSValueItem<F8>;
                        F8[] arrF8 = new F8[1] { itemF8[index] };
                        return new SECSValueItem<F8>(arrF8);
                    case SECSType.I1:
                        var itemI1 = this as SECSValueItem<I1>;
                        I1[] arrI1 = new I1[1] { itemI1[index] };
                        return new SECSValueItem<I1>(arrI1);
                    case SECSType.I2:
                        var itemI2 = this as SECSValueItem<I2>;
                        I2[] arrI2 = new I2[1] { itemI2[index] };
                        return new SECSValueItem<I2>(arrI2);
                    case SECSType.I4:
                        var itemI4 = this as SECSValueItem<I4>;
                        I4[] arrI4 = new I4[1] { itemI4[index] };
                        return new SECSValueItem<I4>(arrI4);
                    case SECSType.I8:
                        var itemI8 = this as SECSValueItem<I8>;
                        I8[] arrI8 = new I8[1] { itemI8[index] };
                        return new SECSValueItem<I8>(arrI8);
                    case SECSType.U1:
                        var itemU1 = this as SECSValueItem<U1>;
                        U1[] arrU1 = new U1[1] { itemU1[index] };
                        return new SECSValueItem<U1>(arrU1);
                    case SECSType.U2:
                        var itemU2 = this as SECSValueItem<U2>;
                        U2[] arrU2 = new U2[1] { itemU2[index] };
                        return new SECSValueItem<U2>(arrU2);
                    case SECSType.U4:
                        var itemU4 = this as SECSValueItem<U4>;
                        U4[] arrU4 = new U4[1] { itemU4[index] };
                        return new SECSValueItem<U4>(arrU4);
                    case SECSType.U8:
                        var itemU8 = this as SECSValueItem<U8>;
                        U8[] arrU8 = new U8[1] { itemU8[index] };
                        return new SECSValueItem<U8>(arrU8);
                    case SECSType.A:
                        var itemA = this as SECSStreamItem<A>;
                        A[] arrA = new A[1] { itemA[index] };
                        return new SECSStreamItem<A>(arrA);
                    case SECSType.L:
                        var itemL = this as SECSListItem;
                        SECSItem item = itemL[index];
                        return item;

                }

                throw new Exception("Illegal SECS Type in SECSItem indexer");
                //return _items[index];
            } 
            set
            {
                if (_items == null)
                { 
                    _items = new List<SECSItem>(); 
                }
                _items[index] = value;
            }
            
        }
        public SECSItem this[string attribute]
        {
            get 
            {
                foreach (var item in this._items)
                {
                    if (item.Attribute == attribute)
                    {
                        return item;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Attribute == attribute)
                    {
                        _items[i] = value;
                        return;
                    }
                }
            }
        }
        public string Attribute { get; set; } = string.Empty;
        virtual public bool IsEmpty { get => false; }
        virtual public int Length { get => 0; }
        virtual internal int DataLength { get => Length; }
        
        virtual internal byte[] Encode()
        {
            return null;
        }

        public virtual string ToString(int tab = 0, bool isShowCount = false, bool isShowIndex = false, int index = 0, bool isShowAttribute = false)
        {
            return string.Empty;
        }

        public virtual void Add(SECSItem item)
        {
            _items.Add(item);
        }

        #region DATA TRANSFER
        //To
        public static implicit operator B(SECSItem value)
        {
            if (value.SECSType == SECSType.B)
            {
                return (value as SECSValueItem<B>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator Bool(SECSItem value)
        {
            if (value.SECSType == SECSType.Bool)
            {
                return (value as SECSValueItem<Bool>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator F4(SECSItem value)
        {
            if (value.SECSType == SECSType.F4)
            {
                return (value as SECSValueItem<F4>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator F8(SECSItem value)
        {
            if (value.SECSType == SECSType.F8)
            {
                return (value as SECSValueItem<F8>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator I1(SECSItem value)
        {
            if (value.SECSType == SECSType.I1)
            {
                return (value as SECSValueItem<I1>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator I2(SECSItem value)
        {
            if (value.SECSType == SECSType.I2)
            {
                return (value as SECSValueItem<I2>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator I4(SECSItem value)
        {
            if (value.SECSType == SECSType.I4)
            {
                return (value as SECSValueItem<I4>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator I8(SECSItem value)
        {
            if (value.SECSType == SECSType.I8)
            {
                return (value as SECSValueItem<I8>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator U1(SECSItem value)
        {
            if (value.SECSType == SECSType.U1)
            {
                return (value as SECSValueItem<U1>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator U2(SECSItem value)
        {
            if (value.SECSType == SECSType.U2)
            {
                return (value as SECSValueItem<U2>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator U4(SECSItem value)
        {
            if (value.SECSType == SECSType.U4)
            {
                return (value as SECSValueItem<U4>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator U8(SECSItem value)
        {
            if (value.SECSType == SECSType.U8)
            {
                return (value as SECSValueItem<U8>)[0];
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator A(SECSItem value)
        {
            if (value.SECSType == SECSType.A)
            {
                return value as SECSStreamItem<A>;
                //var item = value as SECSStreamItem<A>;
                //A b = item;
                //return b;
            }
            else
                throw new Exception("Illegal type transfer");
        }
        public static implicit operator L(SECSItem value)
        {
            if (value.SECSType == SECSType.L)
            {
                return value as SECSListItem;
            }
            else
                throw new Exception("Illegal type transfer");
        }

        //From
        public static implicit operator SECSItem(string value) => new SECSStreamItem<A>(value); // as SECSItem;
        public static implicit operator SECSItem(B[] items) => new SECSValueItem<B>(items);
        public static implicit operator SECSItem(Bool[] items) => new SECSValueItem<Bool>(items);
        public static implicit operator SECSItem(F4[] items) => new SECSValueItem<F4>(items);
        public static implicit operator SECSItem(F8[] items) => new SECSValueItem<F8>(items);
        public static implicit operator SECSItem(I1[] items) => new SECSValueItem<I1>(items);
        public static implicit operator SECSItem(I2[] items) => new SECSValueItem<I2>(items);
        public static implicit operator SECSItem(I4[] items) => new SECSValueItem<I4>(items);
        public static implicit operator SECSItem(I8[] items) => new SECSValueItem<I8>(items);
        public static implicit operator SECSItem(U1[] items) => new SECSValueItem<U1>(items);
        public static implicit operator SECSItem(U2[] items) => new SECSValueItem<U2>(items);
        public static implicit operator SECSItem(U4[] items) => new SECSValueItem<U4>(items);
        public static implicit operator SECSItem(U8[] items) => new SECSValueItem<U8>(items);
        #endregion

    }


    public class SECSValueItem<T> : SECSItem
    {
        new T[] _items;
        //T _mark;
        public T[] Items
        {
            get => _items ?? new T[0];
            set => _items = value;
        }
            
        public new T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        static Type SECSTypeB = typeof(B);
        static Type SECSTypeBool = typeof(Bool);
        static Type SECSTypeF4 = typeof(F4);
        static Type SECSTypeF8 = typeof(F8);
        static Type SECSTypeU1 = typeof(U1);
        static Type SECSTypeU2 = typeof(U2);
        static Type SECSTypeU4 = typeof(U4);
        static Type SECSTypeU8 = typeof(U8);
        static Type SECSTypeI1 = typeof(I1);
        static Type SECSTypeI2 = typeof(I2);
        static Type SECSTypeI4 = typeof(I4);
        static Type SECSTypeI8 = typeof(I8);

        public override bool IsEmpty => _items == null ? true : (_items.Length == 0);
        public override int Length { get => Items.Length; }

        public override void Add(SECSItem item)
        {
            var orilen = _items.Length;
            var len = item.Length;
            Array.Resize(ref _items, orilen + len);
            Array.Copy((item as SECSValueItem<T>).Items, 0, _items, orilen, len);
        }

        public override string ToString(int tab = 0, bool isShowCount = false, bool isShowIndex = false, int index = 0, bool isShowAttribute = false)
        {
            string tabStr = string.Empty;
            for (int i = 0; i < tab; i++)
                tabStr += "    ";
            string typeStr = typeof(T).ToString().Split('.')[2];
            string countStr = string.Empty;
            if (isShowCount)
                countStr = $"[{Length}]";
            string indexStr = string.Empty;
            if (isShowIndex)
                indexStr = $"({index})";
            string attStr = string.Empty;
            if (isShowAttribute && Attribute != "")
                attStr = $@"    /*{Attribute}*/";
            string bodyStr = string.Empty;

            string s = $"{tabStr}{indexStr}<{typeStr}{countStr}";
            int startlen = s.Length + 1;
            if (typeStr == "Bool")
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    T item = _items[i];
                    if (isShowIndex)
                        bodyStr += $" ({i}){(item as ValueBase).ToString()}";
                    else
                        bodyStr += $" {(item as ValueBase).ToString()}";
                }
                bodyStr += ">";
            }
            else if (typeStr == "B")
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    T item = _items[i];
                    if (isShowIndex)
                        bodyStr += $" ({i}){(item as ValueBase).ToString("X2")}";
                    else
                        bodyStr += $" {(item as ValueBase).ToString("X2")}";
                }
                bodyStr += ">";
            }
            else
            {
                string offsetStr = string.Empty;

                for (int i = 0; i < startlen; i++)
                    offsetStr += " ";

                for (int i = 0; i < _items.Length; i++)
                {
                    T item = _items[i];
                    if (isShowIndex)
                    {
                        if (i == 0)
                            bodyStr += $" ({i}){(item as ValueBase).ToString()}";
                        else
                        {
                            bodyStr += $"\r{offsetStr}({i}){(item as ValueBase).ToString()}";
                        }
                    }
                    else
                    {
                        if (i == 0)
                            bodyStr += $" {(item as ValueBase).ToString()}";
                        else
                        {
                            bodyStr += $"\r{offsetStr}{(item as ValueBase).ToString()}";
                        }
                    }
                }
                if (_items.Length > 1)
                {
                    if (isShowIndex)
                    {
                        var indexStrLen = indexStr.Length;
                        string indexSpace = string.Empty;
                        for (int i = 0; i < indexStrLen; i++)
                            indexSpace += " ";
                        bodyStr += $"\r{tabStr}{indexSpace}>";
                    }
                    else
                        bodyStr += $"\r{tabStr}>";
                }
                else
                    bodyStr += ">";
            }

            s += $"{bodyStr}{attStr}";
            return s;
        }
        internal override int DataLength
        {
            get
            {
                switch (SECSType)
                {
                    case SECSType.B:
                        return Length * B.Length;
                    case SECSType.Bool:
                        return Length * Bool.Length;
                    case SECSType.F4:
                        return Length * F4.Length;
                    case SECSType.F8:
                        return Length * F8.Length;
                    case SECSType.I1:
                        return Length * I1.Length;
                    case SECSType.I2:
                        return Length * I2.Length;
                    case SECSType.I4:
                        return Length * I4.Length;
                    case SECSType.I8:
                        return Length * I8.Length;
                    case SECSType.U1:
                        return Length * U1.Length;
                    case SECSType.U2:
                        return Length * U2.Length;
                    case SECSType.U4:
                        return Length * U4.Length;
                    case SECSType.U8:
                        return Length * U8.Length;
                    default:
                        throw new Exception("Invalid SECS type for SECSValueItem");
                }
                //Items.Length * (T.Length).DataLength;
            }
        }
        public T Value { get => Items[0]; set => Items[0] = value; }
        internal SECSType GetSecsType(Type type)
        {
            if (type == SECSTypeB) return SECSType.B;
            if (type == SECSTypeBool) return SECSType.Bool;
            if (type == SECSTypeF4) return SECSType.F4;
            if (type == SECSTypeF8) return SECSType.F8;
            if (type == SECSTypeI1) return SECSType.I1;
            if (type == SECSTypeI2) return SECSType.I2;
            if (type == SECSTypeI4) return SECSType.I4;
            if (type == SECSTypeI8) return SECSType.I8;
            if (type == SECSTypeU1) return SECSType.U1;
            if (type == SECSTypeU2) return SECSType.U2;
            if (type == SECSTypeU4) return SECSType.U4;
            if (type == SECSTypeU8) return SECSType.U8;
            throw new Exception("Unsupported SECSType when GetSecsType in SecsValueItem");
        }

        public SECSValueItem(int count)
        {
            SECSType = GetSecsType(typeof(T));
            Items = new T[count];
        }

        internal SECSValueItem(T value)
        {
            SECSType = GetSecsType(typeof(T));
            _items = new T[1] { value };
        }

        internal SECSValueItem(int count, SECSType stype)
        {
            SECSType = stype;
            Items = new T[count];
        }

        public SECSValueItem(params T[] items)
        {
            SECSType = GetSecsType(typeof(T));
            Items = items;
        }

        internal SECSValueItem(T[] items, SECSType stype)
        {
            SECSType = stype;
            Items = items;
        }

        public SECSValueItem(byte[] bytes, ref int offset, int length)
        {
            SECSType = GetSecsType(typeof(T));
            switch (SECSType)
            {
                case SECSType.B:
                    Items = B.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.Bool:
                    Items = Bool.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.F4:
                    Items = F4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.F8:
                    Items = F8.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I1:
                    Items = I1.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I2:
                    Items = I2.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I4:
                    Items = I4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I8:
                    Items = I8.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U1:
                    Items = U1.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U2:
                    Items = U2.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U4:
                    Items = U4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U8:
                    Items = U8.Decode(bytes, ref offset, length) as T[];
                    break;
                default:
                    throw new Exception("Invalid SECS type for SECSValueItem");
            }
        }
        //for received message data decode
        internal SECSValueItem(byte[] bytes, ref int offset, int length, SECSType stype) : base(bytes, ref offset, length)
        {
            SECSType = stype;
            switch (stype)
            {
                case SECSType.B:
                    Items = B.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.Bool:
                    Items = Bool.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.F4:
                    Items = F4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.F8:
                    Items = F8.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I1:
                    Items = I1.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I2:
                    Items = I2.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I4:
                    Items = I4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.I8:
                    Items = I8.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U1:
                    Items = U1.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U2:
                    Items = U2.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U4:
                    Items = U4.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.U8:
                    Items = U8.Decode(bytes, ref offset, length) as T[];
                    break;
                default:
                    throw new Exception("Invalid SECS type for SECSValueItem");
            }
            
        }

        internal override byte[] Encode()
        {
            switch (SECSType)
            {
                case SECSType.B:
                    return Bytes = B.Encode(this as SECSValueItem<B>);
                case SECSType.Bool:
                    return Bytes = Bool.Encode(this as SECSValueItem<Bool>);
                case SECSType.F4:
                    return Bytes = F4.Encode(this as SECSValueItem<F4>);
                case SECSType.F8:
                    return Bytes = F8.Encode(this as SECSValueItem<F8>);
                case SECSType.I1:
                    return Bytes = I1.Encode(this as SECSValueItem<I1>);
                case SECSType.I2:
                    return Bytes = I2.Encode(this as SECSValueItem<I2>);
                case SECSType.I4:
                    return Bytes = I4.Encode(this as SECSValueItem<I4>);
                case SECSType.I8:
                    return Bytes = I8.Encode(this as SECSValueItem<I8>);
                case SECSType.U1:
                    return Bytes = U1.Encode(this as SECSValueItem<U1>);
                case SECSType.U2:
                    return Bytes = U2.Encode(this as SECSValueItem<U2>);
                case SECSType.U4:
                    return Bytes = U4.Encode(this as SECSValueItem<U4>);
                case SECSType.U8:
                    return Bytes = U8.Encode(this as SECSValueItem<U8>);
                default:
                    throw new Exception("Invalid SECS type for SECSValueItem");
            }
        }

        public static implicit operator T(SECSValueItem<T> value) => value.Value;
        public static implicit operator SECSValueItem<T>(T[] value) => new SECSValueItem<T>(value);
    }

    public class SECSStreamItem<T> : SECSItem where T : StreamBase
    {
        //T _strBody;
        new T[] _items;
        public T[] Items
        {
            get => _items ?? new T[0];
            set => _items = value;
        }

        public new T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        static Type SECSTypeA = typeof(A);
        static Type SECSTypeJIS = typeof(JIS);
        static Type SECSTypeUnicode = typeof(Unicode);

        public override bool IsEmpty => _items == null ? true : (_items.Length == 0);

        public override int Length { get => _items.Length; }
        //internal override int DataLength { get => Length; }

        public override void Add(SECSItem item)
        {
            var orilen = _items.Length;
            var len = item.Length;
            Array.Resize(ref _items, _items.Length + len);
            Array.Copy((item as SECSStreamItem<T>).Items, 0, _items, orilen, len);
        }

        public override string ToString(int tab = 0, bool isShowCount = false, bool isShowIndex = false, int index = 0, bool isShowAttribute = false)
        {
            string tabStr = string.Empty;
            for (int i = 0; i < tab; i++)
                tabStr += "    ";
            string typeStr = typeof(T).ToString().Split('.')[2];
            string countStr = string.Empty;
            if (isShowCount)
                countStr = $"[{Length}]";
            string indexStr = string.Empty;
            if (isShowIndex)
                indexStr = $"({index})";
            string attStr = string.Empty;
            if (isShowAttribute && Attribute != string.Empty)
                attStr = $@"    /*{Attribute}*/";
            string bodyStr = string.Empty;
            if (typeStr == "A")
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    bodyStr += _items[i].Value;
                }
            }

            string s = $"{tabStr}{indexStr}<{typeStr}{countStr} \"{bodyStr}\">{attStr}";
            return s;
        }

        public T Value 
        {
            get 
            {
                if (typeof(T) == typeof(A))
                {
                    A item = _items as A[];
                    return item as T;
                }
                return null;
            }
            set
            {
                if (value.GetType() == typeof(A))
                {
                     A[] items = value as A;
                    _items = items as T[];
                }
            }
        }
        internal SECSType GetSecsType(Type type)
        {
            if (type == SECSTypeA) return SECSType.A;
            if (type == SECSTypeJIS) return SECSType.JIS;
            if (type == SECSTypeUnicode) return SECSType.Unicode;

            throw new Exception("Unsupported SECSType when GetSecsType in SecsStreamItem");
        }

        internal SECSStreamItem(int count)
        {
            SECSType = GetSecsType(typeof(T));
            Items = new T[count];
        }

        internal SECSStreamItem(int count, SECSType stype)
        {
            SECSType = stype;
            Items = new T[count];
        }

        internal SECSStreamItem(T[] items)
        {
            SECSType = GetSecsType(typeof(T));
            Items = items;
        }

        internal SECSStreamItem(T[] items, SECSType stype)
        {
            SECSType = stype;
            Items = items;
        }

        internal SECSStreamItem(byte[] bytes, ref int offset, int length)
        {
            SECSType = GetSecsType(typeof(T));
            switch (SECSType)
            {
                case SECSType.A:
                    Items = A.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.JIS:
                    //Items = JIS.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.Unicode:
                    //Items = Unicode.Decode(bytes, ref offset, length) as T[];
                    break;
                default:
                    throw new Exception("Invalid SECS type for SECSStreamItem");
            }
        }
        //for received message data decode
        internal SECSStreamItem(byte[] bytes, ref int offset, int length, SECSType stype) : base(bytes, ref offset, length)
        {
            SECSType = stype;
            switch (stype)
            {
                case SECSType.A:
                    A[] arr = A.Decode(bytes, ref offset, length);
                    Items = arr as T[];
                    break;
                case SECSType.JIS:
                    //Items = JIS.Decode(bytes, ref offset, length) as T[];
                    break;
                case SECSType.Unicode:
                    //Items = Unicode.Decode(bytes, ref offset, length) as T[];
                    break;
                
                default:
                    throw new Exception("Invalid SECS type for SECSStreamItem");
            }

        }

        internal SECSStreamItem(string value)
        {
            SECSType = SECSType.A;
            Value = new A(value) as T;
        }


        internal override byte[] Encode()
        {
            switch (SECSType)
            {
                case SECSType.A:
                    return Bytes = A.Encode(this as SECSStreamItem<A>);
                case SECSType.JIS:
                    return null;
                case SECSType.Unicode:
                    return null;
                default:
                    throw new Exception("Invalid SECS type for SECSStreamItem");
            }
        }

        #region DATA_TRANSFER
        public static implicit operator T(SECSStreamItem<T> value) => value.Value;
        public static implicit operator string(SECSStreamItem<T> value)
        {
            if (typeof(T) == typeof(A))
            {
                A str = value.Items as A[];
                return str;
            }

            if (typeof(T) == typeof(JIS))
            {
                return null;
            }

            if (typeof(T) == typeof(Unicode))
            {
                return null;
            }

            throw new Exception("Invalid SECS type for SECSStreamItem");
        }
        public static implicit operator SECSStreamItem<T>(string value) => (new SECSStreamItem<A>(value)) as SECSStreamItem<T>;

        #endregion
    }
    public class SECSListItem : SECSItem
    {
        //T _strBody;
        new SECSItem[] _items;
        public List<SECSItem> Items
        {
            get => _items == null ? new List<SECSItem>() : new List<SECSItem>(_items);
            set => _items = value?.ToArray();
        }

        public new SECSItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        static Type SECSTypeL = typeof(L);

        public override bool IsEmpty => _items == null ? true : (_items.Length == 0);

        public override int Length { get => _items.Length; }

        public override void Add(SECSItem item)
        {
            Array.Resize(ref _items, _items.Length + 1);
            _items[_items.Length - 1] = item;
        }

        public override string ToString(int tab = 0, bool isShowCount = false, bool isShowIndex = false, int index = 0, bool isShowAttribute = false)
        {
            string tabStr = string.Empty;
            for (int i = 0; i < tab; i++)
                tabStr += "    ";
            string typeStr = "L";
            string countStr = string.Empty;
            if (isShowCount)
                countStr = $"[{Length}]";
            string indexStr = string.Empty;
            if (isShowIndex)
                indexStr = $"({index})";
            string attStr = string.Empty;
            if (isShowAttribute && Attribute != string.Empty)
                attStr = $@"    /*{Attribute}*/";

            if (_items.Length == 0)
                return $"{tabStr}{indexStr}<{typeStr}{countStr}>{attStr}";

            string s = $"{tabStr}{indexStr}<{typeStr}{countStr}\r";
            string bodyStr = string.Empty;

            for (int i = 0; i < _items.Length; i++)
            {
                bodyStr += _items[i].ToString(tab + 1, isShowCount, isShowIndex, i, isShowAttribute);
                if (i != _items.Length - 1)
                    bodyStr += "\r";
            }

            if (isShowIndex)
            {
                var indexStrLen = indexStr.Length;
                string indexSpace = string.Empty;
                for (int i = 0; i < indexStrLen; i++)
                    indexSpace += " ";
                s += $"{bodyStr}\r{tabStr}{indexSpace}>{attStr}";
            }
            else
                s += $"{bodyStr}\r{tabStr}>{attStr}";
            return s;
        }

        internal SECSListItem(int count)
        {
            SECSType = SECSType.L;
            _items = new SECSItem[count];
        }

        internal SECSListItem(SECSItem[] items)
        {
            SECSType = SECSType.L;
            _items = items;
        }

        internal SECSListItem(List<SECSItem> items)
        {
            SECSType = SECSType.L;
            _items = items.ToArray();
        }

        //for received message data decode
        internal SECSListItem(byte[] bytes, ref int offset, int length)
        {
            SECSType = SECSType.L;
            _items = L.Decode(bytes, ref offset, length).ToArray();
        }

        internal override byte[] Encode()
        {
            return Bytes = L.Encode(this);
        }

        //public static implicit operator L(SECSListItem value) => new L(value._items);


    }
}
