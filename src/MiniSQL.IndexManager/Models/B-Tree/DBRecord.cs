using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MiniSQL.Library.Models;
using MiniSQL.Library.Utilities;

namespace MiniSQL.IndexManager.Models
{
    public enum HeaderValue
    {
        // header size := 1
        // content size := 0
        // the value that corresponds to the table’s primary key is always stored as a NULL value
        NULL = 0,
        // header size := 1
        // content size := 4
        INTEGER = 4,
        // header size := 1
        // content size := 8
        FloatingPoint = 7,
        // header size := 4
        // content size := 8
        // HeaderValue of type TEXT could be `2⋅n + 13` where `n` is a positive number
        TEXT = 13,
    }

    // __format__
    // <header size (1 byte)> <remaining header> <field>
    public class DBRecord
    {
        // data of field section only
        public byte[] FieldData { get; set; }
        // size of the header (metadata)
        public byte HeaderSize { get; set; }
        // size of the whole record in binary form
        public int RecordSize
        {
            get { return this.FieldData.Length + this.HeaderSize; }
        }
        // the list of the types of each field at the head of the record
        public List<uint> HeaderList { get; private set; } = new List<uint>();
        // the list of offsets to each field
        private readonly List<int> FieldOffsets = new List<int>();

        // constructor
        public DBRecord(byte[] data, int startIndex)
        {
            Unpack(data, startIndex);
        }

        // constructor
        public DBRecord(List<AtomValue> values)
        {
            SetValues(values);
        }

        // initialize the object to be empty
        private void InitializeEmpty()
        {
            this.HeaderSize = 0;
            this.HeaderList.Clear();
            this.FieldData = null;
            this.FieldOffsets.Clear();
        }

        // get stored values in models (objects)
        public List<AtomValue> GetValues()
        {
            List<AtomValue> values = new List<AtomValue>();
            int i;
            for (i = 0; i < this.HeaderList.Count; i++)
            {
                AtomValue value = new AtomValue();
                uint headerValue = this.HeaderList[i];
                if (headerValue == (int)HeaderValue.INTEGER)
                {
                    value.Type = AttributeTypes.Int;
                    value.IntegerValue = BitConverter.ToInt32(this.FieldData, this.FieldOffsets[i]);
                }
                else if (headerValue == (int)HeaderValue.FloatingPoint)
                {
                    value.Type = AttributeTypes.Float;
                    value.FloatValue = BitConverter.ToDouble(this.FieldData, this.FieldOffsets[i]);
                }
                else if (headerValue == (int)HeaderValue.NULL)
                {
                    value.Type = AttributeTypes.Null;
                }
                else if ((headerValue - (int)HeaderValue.TEXT) % 2 == 0)
                {
                    value.Type = AttributeTypes.Char;
                    int stringLength = (int)(headerValue - (uint)HeaderValue.TEXT) / 2;
                    value.CharLimit = stringLength;
                    value.StringValue = Encoding.UTF8.GetString(this.FieldData, this.FieldOffsets[i], stringLength).TrimEnd('\0');
                }
                else
                {
                    throw new Exception($"Header value {headerValue} does not exists");
                }

                values.Add(value);
            }
            return values;
        }

        // set the record based on a list of values
        private void SetValues(List<AtomValue> values)
        {
            InitializeEmpty();

            // the first bit is reserved for <header size>
            byte headerSize = 1;
            // set header
            foreach (AtomValue value in values)
            {
                switch (value.Type)
                {
                    case AttributeTypes.Null:
                        this.HeaderList.Add((int)HeaderValue.NULL);
                        headerSize += 1;
                        break;
                    case AttributeTypes.Int:
                        this.HeaderList.Add((int)HeaderValue.INTEGER);
                        headerSize += 1;
                        break;
                    case AttributeTypes.Float:
                        this.HeaderList.Add((int)HeaderValue.FloatingPoint);
                        headerSize += 1;
                        break;
                    case AttributeTypes.Char:
                        int stringLength = value.CharLimit;
                        if (BTreeConfiguration.IsIgnoreCharLimit)
                            stringLength = Encoding.UTF8.GetBytes(value.StringValue).Length;
                        this.HeaderList.Add((uint)stringLength * 2 + (int)HeaderValue.TEXT);
                        headerSize += 4;
                        break;
                }
            }

            // set headerSize
            this.HeaderSize = headerSize;

            // set field
            int fieldOffset = 0;
            List<byte> field = new List<byte>();
            foreach (AtomValue value in values)
            {
                byte[] binaryValue;
                this.FieldOffsets.Add(fieldOffset);
                switch (value.Type)
                {
                    case AttributeTypes.Null:
                        fieldOffset += 0;
                        break;
                    case AttributeTypes.Int:
                        binaryValue = BitConverter.GetBytes(value.IntegerValue);
                        field.AddRange(binaryValue);
                        fieldOffset += 4;
                        break;
                    case AttributeTypes.Float:
                        binaryValue = BitConverter.GetBytes(value.FloatValue);
                        field.AddRange(binaryValue);
                        fieldOffset += 8;
                        break;
                    case AttributeTypes.Char:
                        binaryValue = Encoding.UTF8.GetBytes(value.StringValue);
                        if (!BTreeConfiguration.IsIgnoreCharLimit)
                            Array.Resize(ref binaryValue, value.CharLimit);
                        field.AddRange(binaryValue);
                        fieldOffset += binaryValue.Length;
                        break;
                }
            }
            this.FieldData = field.ToArray();
        }

        // from raw data to this object
        public void Unpack(byte[] data, int startIndex)
        {
            InitializeEmpty();
            // first byte
            this.HeaderSize = data[startIndex];
            // header
            int headerOffset = 1;
            while (headerOffset < this.HeaderSize)
            {
                uint value;
                VarintType varintType;
                (value, varintType) = VarintBitConverter.FromVarint(data, headerOffset + startIndex);

                this.HeaderList.Add(value);
                if (varintType == VarintType.Varint32)
                    // string
                    headerOffset += 4;
                else
                    headerOffset += 1;
            }

            int fieldStartIndex = headerOffset + startIndex;

            // field
            int fieldOffset = 0;

            foreach (int headerValue in this.HeaderList)
            {
                this.FieldOffsets.Add(fieldOffset);
                if (headerValue == (int)HeaderValue.INTEGER)
                    fieldOffset += 4;
                else if (headerValue == (int)HeaderValue.FloatingPoint)
                    fieldOffset += 8;
                else if (headerValue == (int)HeaderValue.NULL)
                    fieldOffset += 0;
                else if ((headerValue - (int)HeaderValue.TEXT) % 2 == 0)
                {
                    int stringLength = (headerValue - (int)HeaderValue.TEXT) / 2;
                    fieldOffset += stringLength;
                }
                else
                    throw new Exception($"Header value {headerValue} does not exists");
            }

            int fieldLength = fieldOffset;

            this.FieldData = data.Skip(fieldStartIndex).Take(fieldLength).ToArray();
        }

        // from this object to raw data
        public byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            // header
            pack.Add(this.HeaderSize);
            foreach (int headerValue in this.HeaderList)
            {
                if (headerValue == (int)HeaderValue.NULL
                    || headerValue == (int)HeaderValue.INTEGER
                    || headerValue == (int)HeaderValue.FloatingPoint)
                    pack.AddRange(VarintBitConverter.ToVarint((uint)headerValue, VarintType.Varint8));
                else if ((headerValue - (int)HeaderValue.TEXT) % 2 == 0)
                    pack.AddRange(VarintBitConverter.ToVarint((uint)headerValue, VarintType.Varint32));
                else
                    throw new Exception($"Header value {headerValue} does not exists");
            }
            // field
            pack.AddRange(this.FieldData);
            return pack.ToArray();
        }
    }
}
