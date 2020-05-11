using System;
using System.Collections.Generic;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BufferManager Test Begin");

            List<AtomValue> values = new List<AtomValue>();
            AtomValue value1 = new AtomValue() {Type = AttributeTypes.Int, IntegerValue = 222};
            AtomValue value2 = new AtomValue() {Type = AttributeTypes.Null};
            AtomValue value3 = new AtomValue() {Type = AttributeTypes.Char, CharLimit = 5, StringValue = "222"};
            values.Add(value1);
            values.Add(value2);
            values.Add(value3);
            DBRecord record = new DBRecord(values);

            List<AtomValue> valueOut1;
            valueOut1 = record.GetValues();

            byte[] raw = record.Pack();
            record.UnPack(raw, 0);
            List<AtomValue> valueOut2 = record.GetValues();

            Console.WriteLine("BufferManager Test End");
        }
    }
}
