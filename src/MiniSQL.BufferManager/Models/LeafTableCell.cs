using System;
using System.Linq;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Models
{
    public class LeafTableCell
    {
        public byte[] Data;

        public LeafTableCell(byte[] data, int startIndex)
        {
            // TODO
        }

        // Length of DB–Record in bytes.
        public UInt32 DBRecordSize
        {
            get { return BitConverter.ToUInt32(this.Data, 0); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 0, 4); }
        }
        // TODO: replace `UInt32 Key` to `byte[] Key` and `DBRecord KeyObject`
        // DBRecord is a database record and Key is its primary key.
        public UInt32 Key
        {
            get { return BitConverter.ToUInt32(this.Data, 4); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.Data, 4, 4); }
        }
        public byte[] DBRecord
        {
            get { return this.Data.Skip(8).ToArray(); }
            set
            {
                Array.Copy(value, 0, this.Data, 8, value.Length);
                this.DBRecordSize = (uint)this.Data.Length;
            }
        }
        public DBRecord DBRecordObject
        {
            get { return new DBRecord(this.DBRecord, 0); }
            set { this.DBRecord = value.Data; }
        }
    }
}
