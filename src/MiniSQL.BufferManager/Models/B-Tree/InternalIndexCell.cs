using System;
using System.Collections.Generic;
using MiniSQL.Library.Utilities;

namespace MiniSQL.BufferManager.Models
{
    // <childPage (4 bytes)> <remaining size (1 byte)> <header size (1 bytes)> <key-idx size (1 byte)> <key-pk size (1 byte)> <key-idx> <key-pk>
    public class InternalIndexCell : BTreeCell
    {
        // ChildPage “pointer”
        // this “pointer” is the number of the page where the referenced node can be found
        // ChildPage is the number of the page containing the entries with keys less than or equal to Key.
        public UInt32 ChildPage { get; set; }
        public DBRecord PrimaryKey { get; set; }

        // Key is the key being indexed

        public uint RemainingSize
        {
            get
            {
                uint size = this.HeaderSize + (uint)this.Key.RecordSize + (uint)this.PrimaryKey.RecordSize;
                return size + (uint)VarintSize.GetVarintSize(size + 4);
            }
        }

        public uint HeaderSize
        {
            get
            {
                uint size = (uint)VarintSize.GetVarintSize(this.KeyIdxSize) + (uint)VarintSize.GetVarintSize(this.KeyPKSize);
                return size + (uint)VarintSize.GetVarintSize(size + 4);
            }
        }

        public uint KeyIdxSize { get { return (uint)this.Key.RecordSize; } }
        public uint KeyPKSize { get { return (uint)this.PrimaryKey.RecordSize; } }

        public InternalIndexCell(byte[] data, int startIndex)
        {
            Unpack(data, startIndex);
        }

        public InternalIndexCell(DBRecord key, UInt32 childPage, DBRecord primaryKey)
        {
            this.Key = key;
            this.ChildPage = childPage;
            this.PrimaryKey = primaryKey;
        }

        public override byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            pack.AddRange(BitConverter.GetBytes(this.ChildPage));
            pack.AddRange(VarintBitConverter.ToVarint(this.RemainingSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.HeaderSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.KeyIdxSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.KeyPKSize).Item1);
            pack.AddRange(this.Key.Pack());
            pack.AddRange(this.PrimaryKey.Pack());
            return pack.ToArray();
        }

        public override void Unpack(byte[] data, int startIndex)
        {
            uint tmpUInt;
            VarintType type;
            // child page
            this.ChildPage = BitConverter.ToUInt32(data, startIndex);
            // remaining size
            int startOffsetOfRemainingSize = 4;
            (tmpUInt, type) = VarintBitConverter.FromVarint(data, startIndex + startOffsetOfRemainingSize);
            // header size
            int startOffsetOfHeaderSize = startOffsetOfRemainingSize + VarintSize.GetVarintSize(tmpUInt);
            (tmpUInt, type) = VarintBitConverter.FromVarint(data, startIndex + startOffsetOfHeaderSize);
            // key-idx size
            int startOffsetOfKeyIdxSize = startOffsetOfHeaderSize + VarintSize.GetVarintSize(tmpUInt);
            (tmpUInt, type) = VarintBitConverter.FromVarint(data, startIndex + startOffsetOfKeyIdxSize);
            // key-pk size
            int startOffsetOfKeyPKSize = startOffsetOfKeyIdxSize + VarintSize.GetVarintSize(tmpUInt);
            (tmpUInt, type) = VarintBitConverter.FromVarint(data, startIndex + startOffsetOfKeyPKSize);
            // key-idx
            int startOffsetOfKeyIdx = startOffsetOfKeyPKSize + VarintSize.GetVarintSize(tmpUInt);
            this.Key = new DBRecord(data, startOffsetOfKeyIdx + startIndex);
            // key-pk
            int startOffsetOfKeyPK = startOffsetOfKeyIdx + this.Key.RecordSize;
            this.PrimaryKey = new DBRecord(data, startOffsetOfKeyPK + startIndex);
        }
    }
}
