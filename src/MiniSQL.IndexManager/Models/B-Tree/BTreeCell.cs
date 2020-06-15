using System;

namespace MiniSQL.IndexManager.Models
{
    public enum CellTypes
    {
        LeafTableCell,
        InternalTableCell,
        LeafIndexCell,
        InternalIndexCell,
    }

    // base class for all types of B-Tree cells
    // NOTICE: you are only getting a COPY, any modification on the cell will NOT affect the node
    public abstract class BTreeCell
    {
        public CellTypes Types { get; set; }
        // key being indexed if it is in an index cell
        // or the primary key if it is in a table cell
        public DBRecord Key { get; set; }

        protected BTreeCell() { }

        public abstract void Unpack(byte[] data, int startIndex);
        public abstract byte[] Pack();
    }
}
