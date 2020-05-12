using System;

namespace MiniSQL.BufferManager.Models
{
    public enum CellTypes
    {
        LeafTableCell,
        InternalTableCell,
        LeafIndexCell,
        InternalIndexCell,
    }

    public abstract class BTreeCell
    {
        public CellTypes Types { get; set; }
        // key being indexed if it is in an index cell
        public DBRecord Key { get; set; }

        protected BTreeCell() { }

        public abstract void Unpack(byte[] data, int startIndex);
        public abstract byte[] Pack();
    }
}
