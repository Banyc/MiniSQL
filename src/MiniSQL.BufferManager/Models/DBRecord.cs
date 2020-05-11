using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MiniSQL.Library.Models;
using MiniSQL.Library.Utilities;

namespace MiniSQL.BufferManager.Models
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
        // TEXT = 2⋅n + 13
        TEXT = 13,
    }

    // TODO: test
    public class DBRecord
    {
        public byte[] FieldData { get; set; }

        public byte HeaderSize { get; set; }

        public List<int> HeaderList { get; private set; } = new List<int>();

        private List<int> FieldOffsets = new List<int>();

        private void InitializeEmpty()
        {
            this.HeaderSize = 0;
            this.HeaderList.Clear();
            this.FieldData = null;
            this.FieldOffsets.Clear();
        }

        // get stored values
        public List<AtomValue> GetValues()
        {
            List<AtomValue> values = new List<AtomValue>();
            int i;
            for (i = 0; i < this.HeaderList.Count; i++)
            {
                AtomValue value = new AtomValue();
                int headerValue = this.HeaderList[i];
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
                    value.Type = AttributeTypes.Null;

                else if ((headerValue - (int)HeaderValue.TEXT) % 2 == 0)
                {
                    value.Type = AttributeTypes.Char;
                    int stringLength = (headerValue - (int)HeaderValue.TEXT) / 2;
                    value.StringValue = Encoding.UTF8.GetString(this.FieldData, this.FieldOffsets[i], stringLength).TrimEnd('\0');
                }
                else
                    throw new Exception($"Header value {headerValue} does not exists");

                values.Add(value);
            }
            return values;
        }

        public void SetValues(List<AtomValue> values)
        {
            InitializeEmpty();

            // the first bit is reserved for 
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
                        // int stringLength = Encoding.UTF8.GetByteCount(value.StringValue);  // only for varchar(n), not for char(n)
                        int stringLength = value.CharLimit;
                        this.HeaderList.Add(stringLength * 2 + (int)HeaderValue.TEXT);
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
                        Array.Resize(ref binaryValue, value.CharLimit);
                        field.AddRange(binaryValue);
                        fieldOffset += binaryValue.Length;
                        break;
                }
            }
            this.FieldData = field.ToArray();
        }

        public DBRecord(byte[] data, int startIndex)
        {
            UnPack(data, startIndex);
        }

        public DBRecord(List<AtomValue> values)
        {
            SetValues(values);
        }

        // from raw data to object
        public void UnPack(byte[] data, int startIndex)
        {
            InitializeEmpty();
            // first byte
            this.HeaderSize = data[0];
            // header
            int index = 1;
            while (index < this.HeaderSize)
            {
                int value;
                VarintType varintType;
                (value, varintType) = VarintBitConverter.FromVarint(data, index);

                this.HeaderList.Add(value);
                if (varintType == VarintType.Varint32)
                    // string
                    index += 4;
                else
                    index += 1;
            }

            int fieldStartIndex = index;

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

        public byte[] Pack()
        {
            List<byte> pack = new List<byte>();
            // header
            pack.Add(this.HeaderSize);
            foreach (int headerValue in this.HeaderList)
            {
                if (headerValue == (int)HeaderValue.NULL
                    || headerValue == (int)HeaderValue.INTEGER)
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
