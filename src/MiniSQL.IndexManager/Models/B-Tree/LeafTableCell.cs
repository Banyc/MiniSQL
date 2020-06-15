using System;
using System.Collections.Generic;
using System.Linq;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Models
{
    // <DBRecordSize (4 bytes)> <Key> <DBRecord>
    public class LeafTableCell : BTreeCell
    {
        // constructor
        public LeafTableCell(byte[] data, int startIndex)
        {
            this.Types = CellTypes.LeafTableCell;
            Unpack(data, startIndex);
        }

        // constructor
        public LeafTableCell(DBRecord key, DBRecord record)
        {
            this.Types = CellTypes.LeafTableCell;
            this.Key = key;
            this.DBRecord = record;
        }

        // DBRecord is a database record and Key is its primary key.
        public DBRecord DBRecord { get; set; }
        // size of the DBRecord field in binary form
        public int DBRecordSize
        {
            get { return this.DBRecord.RecordSize; }
        }

        // to bytes
        public override byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            pack.AddRange(BitConverter.GetBytes(this.DBRecordSize));
            pack.AddRange(this.Key.Pack());
            pack.AddRange(this.DBRecord.Pack());
            return pack.ToArray();
        }

        // from bytes
        public override void Unpack(byte[] data, int startIndex)
        {
            int startOffsetOfKey = 4;
            this.Key = new DBRecord(data, startIndex + startOffsetOfKey);
            int keySize = this.Key.RecordSize;
            int startOffsetOfRecord = startOffsetOfKey + keySize;
            this.DBRecord = new DBRecord(data, startIndex + startOffsetOfRecord);
        }
    }
}
