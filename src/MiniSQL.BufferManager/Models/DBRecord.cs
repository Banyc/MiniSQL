using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MiniSQL.Library.Models;

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

    public class DBRecord
    {
        public byte[] Data { get; set; }

        public byte HeaderSize
        {
            get { return this.Data[0]; }
            set { this.Data[0] = value; }
        }

        public int FieldCount  { get; set; }

        public byte[] Header
        {
            get { return this.Data.Take(this.HeaderSize).ToArray(); }
            set { Array.Copy(value, 0, this.Data, 0, this.HeaderSize); }
        }

        public byte[] Field
        {
            get { return this.Data.Skip(this.HeaderSize).ToArray(); }
            set { Array.Copy(value, 0, this.Data, 0, value.Length); }
        }

        public DBRecord(byte[] data, int startIndex)
        {
            // TODO

        }

        // TODO: Test
        public DBRecord(List<AtomValue> row, string primaryKey = "")
        {
            // count the length of Data array
            int count = 0;
            // headersize + 1
            count += 1;
            foreach (var value in row)
            {
                switch (value.Type)
                {
                    case AttributeType.Int:
                        // header size + 1
                        count += 1;
                        if (value.IntegerValue != null)
                            // field size + 4
                            count += 4;
                        break;
                    case AttributeType.Float:
                        // header size + 1
                        count += 1;
                        if (value.FloatValue != null)
                            // field size + 8
                            count += 8;
                        break;
                    case AttributeType.Char:
                        if (value.StringValue != null)
                        {
                            // header size + 4
                            count += 4;
                            // field size + n
                            count += Encoding.UTF8.GetBytes(value.StringValue).Length;
                        }
                        else
                        {
                            // header size + 1
                            count += 1;
                        }
                        break;
                }
            }
            this.Data = new byte[count];

            int index = 0;
            index++;
            // set header
            foreach (var value in row)
            {
                switch (value.Type)
                {
                    case AttributeType.Int:
                        if (value.IntegerValue != null)
                        {
                            this.Data[index] = (byte)HeaderValue.INTEGER;
                        }
                        else
                        {
                            this.Data[index] = (byte)HeaderValue.NULL;
                        }
                        index++;
                        break;
                    case AttributeType.Float:
                        if (value.FloatValue != null)
                        {
                            this.Data[index] = (byte)HeaderValue.FloatingPoint;
                        }
                        else
                        {
                            this.Data[index] = (byte)HeaderValue.NULL;
                        }
                        index++;
                        break;
                    case AttributeType.Char:
                        if (value.StringValue != null)
                        {
                            Array.Copy(BitConverter.GetBytes(Encoding.UTF8.GetBytes(value.StringValue).Length * 2 + 13), 0, this.Data, index, 4);
                            index += 4;
                        }
                        else
                        {
                            this.Data[index] = (byte)HeaderValue.NULL;
                            index += 1;
                        }
                        break;
                }
            }
            
            // set headerSize
            this.Data[0] = (byte)index;

            // set field
            foreach (var value in row)
            {
                switch (value.Type)
                {
                    case AttributeType.Int:
                        if (value.IntegerValue != null)
                        {
                            Array.Copy(BitConverter.GetBytes(value.IntegerValue.Value), 0, this.Data, index, 4);
                            index += 4;
                        }
                        break;
                    case AttributeType.Float:
                        if (value.IntegerValue != null)
                        {
                            Array.Copy(BitConverter.GetBytes(value.FloatValue.Value), 0, this.Data, index, 8);
                            index += 8;
                        }
                        break;
                    case AttributeType.Char:
                        if (value.StringValue != null)
                        {
                            byte[] StringBinary = Encoding.UTF8.GetBytes(value.StringValue);
                            Array.Copy(StringBinary, 0, this.Data, index, StringBinary.Length);
                            index += 4;
                        }
                        break;
                }
            }

            // assert
            if (count != index)
                throw new Exception($"count {count} != index {index}");
        }

        // public List<AtomValue> GetValues(CreateStatement schemaTable)
        public List<AtomValue> GetValues(List<AttributeDeclaration> declarations, AttributeValue primaryKey = null)
        {
            List<AtomValue> values = new List<AtomValue>();
            byte[] header = this.Header;
            byte[] field = this.Field;
            int headerIndex = 1;
            int fieldIndex = 0;

            foreach (var declaration in declarations)
            {
                AtomValue value = new AtomValue();
                value.Type = declaration.Type;

                switch (declaration.Type)
                {
                    case AttributeType.Int:
                        if (header[headerIndex] == (byte)HeaderValue.NULL)
                        {
                            value.IntegerValue = null;
                        }
                        else
                        {
                            value.IntegerValue = BitConverter.ToInt32(field, fieldIndex);
                            fieldIndex += 4;
                        }
                        headerIndex += 1;
                        break;
                    case AttributeType.Float:
                        if (header[headerIndex] == (byte)HeaderValue.NULL)
                        {
                            value.FloatValue = null;
                        }
                        else
                        {
                            value.FloatValue = BitConverter.ToDouble(field, fieldIndex);
                            fieldIndex += 8;
                        }
                        headerIndex += 1;
                        break;
                    case AttributeType.Char:
                        if (header[headerIndex] == (byte)HeaderValue.NULL)
                        {
                            value.StringValue = null;
                        }
                        else
                        {
                            int stringLength = (BitConverter.ToInt32(header, headerIndex) - 13) / 2;
                            value.StringValue = Encoding.UTF8.GetString(field.Skip(fieldIndex).Take(stringLength).ToArray());
                            fieldIndex += 8;
                        }
                        headerIndex += 1;
                        break;
                }

                // the value that corresponds to the table’s primary key is always stored as a NULL value
                // so, we need to recover primary key
                if (primaryKey != null)
                {
                    if (primaryKey.AttributeName == declaration.AttributeName)
                    {
                        switch (declaration.Type)
                        {
                            case AttributeType.Float:
                                value.FloatValue = primaryKey.FloatValue;
                                break;
                            case AttributeType.Int:
                                value.IntegerValue = primaryKey.IntegerValue;
                                break;
                            case AttributeType.Char:
                                value.StringValue = primaryKey.StringValue;
                                break;
                        }
                    }
                }

                values.Add(value);
            }

            return values;
        }
    
        // public HeaderValue GetType(int headerIndex)
        // {

        // }
    }
}
