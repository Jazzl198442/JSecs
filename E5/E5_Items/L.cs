using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs.E5
{
    public class L
    {
        List<SECSItem> _items;
        public int Length { get => _items.Count; }

        public static SECSType SECSType => SECSType.L;

        public L()
        {
            _items = new List<SECSItem>();
        }
        public L(List<SECSItem> items)
        {
            _items = items;
        }

        public L(SECSItem[] items)
        {
            _items = new List<SECSItem>(items);
        }

        public SECSItem this[int index]
        { 
            get => _items[index];
            set => _items[index] = value;
        }

        public void Add(SECSItem item)
        {
            _items.Add(item);
        }

        //public L(byte[] bytes, ref int offset, int itemCount) : base(bytes, ref offset, itemCount)
        //{
        //    int start = offset;
        //    IsSECSList = true;
        //    SECSType = SECSType.L;
        //    /*                                 NOW
        //      byte1    byte2   [byte3]  [byte4] ↓ Data
        //    |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000....
        //       fc  lol         length             data
        //    */
        //    byte header;
        //    int formatCode;
        //    int lol;
        //    int length;

        //    _items = new List<SECSItem>(itemCount);

        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        header = bytes[offset];
        //        formatCode = header >> 2;
        //        lol = header & 3;   //0b00000011

        //        switch (lol)
        //        {
        //            case 0:
        //                throw new Exception("byte LOL illegal, data format error");
        //            case 1:
        //                length = bytes[offset + 1];
        //                offset += 2;
        //                break;
        //            case 2:
        //                length = (bytes[offset + 1] << 8) + bytes[offset + 2];
        //                offset += 3;
        //                break;
        //            case 3:
        //                length = (bytes[offset + 2] << 16) + (bytes[offset + 1] << 8) + bytes[offset + 2];
        //                offset += 4;
        //                break;
        //            default:
        //                throw new Exception("byte LOL illegal, data format error");
        //        }

        //        SECSType stype = (SECSType)formatCode;
        //        switch (stype)
        //        {
        //            case SECSType.L:  //List: length in means how many secsitem in this list, for other types, it means byte for length 
        //                _items.Add(new L(bytes, ref offset, length));
        //                break;
        //            case SECSType.B:
        //                //_items.Add(new B(bytes, ref offset, length));
        //                _items.Add(new SECSValueItem<B>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.Bool:
        //                //_items.Add(new Bool(bytes, ref offset, length));
        //                _items.Add(new SECSValueItem<Bool>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.A:
        //                _items.Add(new SECSStreamItem<A>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.JIS:
        //                throw new NotImplementedException();
        //                break;
        //            case SECSType.Unicode:
        //                throw new NotImplementedException();
        //                break;
        //            case SECSType.I8:
        //                _items.Add(new SECSValueItem<I8>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.I1:
        //                _items.Add(new SECSValueItem<I1>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.I2:
        //                _items.Add(new SECSValueItem<I2>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.I4:
        //                _items.Add(new SECSValueItem<I4>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.F8:
        //                _items.Add(new SECSValueItem<F8>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.F4:
        //                _items.Add(new SECSValueItem<F4>(bytes, ref offset, length, stype)); 
        //                break;
        //            case SECSType.U8:
        //                _items.Add(new SECSValueItem<U8>(bytes, ref offset, length, stype)); 
        //                break;
        //            case SECSType.U1:
        //                _items.Add(new SECSValueItem<U1>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.U2:
        //                _items.Add(new SECSValueItem<U2>(bytes, ref offset, length, stype));
        //                break;
        //            case SECSType.U4:
        //                _items.Add(new SECSValueItem<U4>(bytes, ref offset, length, stype));
        //                break;
        //            default:
        //                throw new Exception("Invalid format code or decode error");
        //        }
        //    }

        //    _length = offset - start;
        //}

        public static List<SECSItem> Decode(byte[] bytes, ref int offset, int itemCount)
        {
            int start = offset;
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000....
               fc  lol         length             data
            */
            byte header;
            int formatCode;
            int lol;
            int length;

            List<SECSItem> _items = new List<SECSItem>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                header = bytes[offset];
                formatCode = header >> 2;
                lol = header & 3;   //0b00000011

                switch (lol)
                {
                    case 0:
                        throw new Exception("byte LOL illegal, data format error");
                    case 1:
                        length = bytes[offset + 1];
                        offset += 2;
                        break;
                    case 2:
                        length = (bytes[offset + 1] << 8) + bytes[offset + 2];
                        offset += 3;
                        break;
                    case 3:
                        length = (bytes[offset + 2] << 16) + (bytes[offset + 1] << 8) + bytes[offset + 2];
                        offset += 4;
                        break;
                    default:
                        throw new Exception("byte LOL illegal, data format error");
                }

                SECSType stype = (SECSType)formatCode;
                switch (stype)
                {
                    case SECSType.L:  //List: length in means how many secsitem in this list, for other types, it means byte for length 
                        _items.Add(new SECSListItem(bytes, ref offset, length));
                        break;
                    case SECSType.B:
                        //_items.Add(new B(bytes, ref offset, length));
                        _items.Add(new SECSValueItem<B>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.Bool:
                        //_items.Add(new Bool(bytes, ref offset, length));
                        _items.Add(new SECSValueItem<Bool>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.A:
                        _items.Add(new SECSStreamItem<A>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.JIS:
                        throw new NotImplementedException();
                    case SECSType.Unicode:
                        throw new NotImplementedException();
                    case SECSType.I8:
                        _items.Add(new SECSValueItem<I8>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.I1:
                        _items.Add(new SECSValueItem<I1>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.I2:
                        _items.Add(new SECSValueItem<I2>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.I4:
                        _items.Add(new SECSValueItem<I4>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.F8:
                        _items.Add(new SECSValueItem<F8>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.F4:
                        _items.Add(new SECSValueItem<F4>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.U8:
                        _items.Add(new SECSValueItem<U8>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.U1:
                        _items.Add(new SECSValueItem<U1>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.U2:
                        _items.Add(new SECSValueItem<U2>(bytes, ref offset, length, stype));
                        break;
                    case SECSType.U4:
                        _items.Add(new SECSValueItem<U4>(bytes, ref offset, length, stype));
                        break;
                    default:
                        throw new Exception("Invalid format code or decode error");
                }
            }

            return _items;
            //_length = offset - start;
        }

        internal static byte[] Encode(SECSListItem list)
        {
            /*                                 NOW
              byte1    byte2   [byte3]  [byte4] ↓ Data
            |00000000|00000000|00000000|00000000|0000000000000000000000000000000000000000000000000...
             |____||| |________________________| |________________________________________________... 
               fc  lol         length             data
            */
            if (SECSType != SECSType.L) throw new Exception("SECSItem invalid when encode L");

            List<byte[]> lst = new List<byte[]>();
            int totalLen = 0;
            int count = list.Length;

            for (int i = 0; i < count; i++)
            {
                byte[] itemByte = SECSEncoder.Encode(list[i]);
                lst.Add(itemByte);
                totalLen += itemByte.Length;
            }

            byte[] bytes = new byte[totalLen];
            int offset = 0;
            int len;
            for (int i = 0; i < lst.Count; i++)
            {
                len = lst[i].Length;
                Buffer.BlockCopy(lst[i], 0, bytes, offset, len);
                offset += len;
            }
            //_length = bytes.Length;
            return bytes;
        }

        public static implicit operator L(SECSListItem value) => new L(value.Items);
        public static implicit operator SECSListItem(L value) => new SECSListItem(value._items);

    }
}
