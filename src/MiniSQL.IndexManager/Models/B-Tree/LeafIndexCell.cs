using System;
using System.Collections.Generic;
using MiniSQL.Library.Utilities;

namespace MiniSQL.IndexManager.Models
{
    public class LeafIndexCell : BTreeCell
    {
        // primary key of the table
        // it is NOT the key being indexed
        public DBRecord PrimaryKey { get; set; }

        // the property `Key` is the key being indexed

        // the size of the cell
        public uint RemainingSize
        {
            get
            {
                uint size = this.HeaderSize + (uint)this.Key.RecordSize + (uint)this.PrimaryKey.RecordSize;
                return size + (uint)VarintSize.GetVarintSize(size + 4);
            }
        }

        // the size of header excluding the field <remaining size>
        public uint HeaderSize
        {
            get
            {
                uint size = (uint)VarintSize.GetVarintSize(this.KeyIdxSize) + (uint)VarintSize.GetVarintSize(this.KeyPKSize);
                return (uint)VarintSize.GetVarintSize(size + 4);
            }
        }
        // size of index key
        public uint KeyIdxSize { get { return (uint)this.Key.RecordSize; } }
        // size of primary key (not the key being indexed)
        public uint KeyPKSize { get { return (uint)this.PrimaryKey.RecordSize; } }

        // constructor
        public LeafIndexCell(byte[] data, int startIndex)
        {
            this.Types = CellTypes.LeafIndexCell;
            Unpack(data, startIndex);
        }

        // constructor
        public LeafIndexCell(DBRecord key, DBRecord primaryKey)
        {
            this.Types = CellTypes.LeafIndexCell;
            this.Key = key;
            this.PrimaryKey = primaryKey;
        }

        // to bytes
        public override byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            pack.AddRange(VarintBitConverter.ToVarint(this.RemainingSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.HeaderSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.KeyIdxSize).Item1);
            pack.AddRange(VarintBitConverter.ToVarint(this.KeyPKSize).Item1);
            pack.AddRange(this.Key.Pack());
            pack.AddRange(this.PrimaryKey.Pack());
            return pack.ToArray();
        }

        // from bytes
        public override void Unpack(byte[] data, int startIndex)
        {
            uint tmpUInt;
            VarintType type;
            // remaining size
            int startOffsetOfRemainingSize = 0;
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
