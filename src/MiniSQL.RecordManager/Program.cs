using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.Library.Models;

namespace MiniSQL.RecordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[RecordManager] Test start!");

            TestInsertRecord();

            Console.WriteLine("[RecordManager] Test end!");
        }

        private static void TestInsertRecord()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            BTreeController controller = new BTreeController(pager, freeList);

            RecordManager recordManager = new RecordManager(pager, controller);

            // create new table
            CreateStatement createStatement = GetCreateStatement();
            int newRoot = recordManager.CreateTable(createStatement);

            // insert
            InsertStatement insertStatement;
            int key;
            int newRootAfterInsert;
            int i;

            for (i = 0; i < 130; i++)
            // for (i = 0; i < 199; i++)
            {
                (insertStatement, key) = GetInsertStatement(1);
                AtomValue atomValue = GetAtomValue(key);
                newRootAfterInsert = recordManager.InsertRecord(insertStatement, atomValue, newRoot);
                Debug.Assert(newRoot == newRootAfterInsert);
            }
        }

        private static CreateStatement GetCreateStatement()
        {
            CreateStatement createStatement = new CreateStatement();
            createStatement.CreateType = CreateType.Table;
            createStatement.TableName = "randomTable";
            createStatement.AttributeDeclarations = new List<AttributeDeclaration>()
            {
                GetAttributeDeclaration("a"),
                GetAttributeDeclaration("id"),
                GetAttributeDeclaration("b"),
            };
            createStatement.PrimaryKey = "id";
            return createStatement;
        }

        private static (InsertStatement, int) GetInsertStatement(int returnIndex)
        {
            Random rnd = new Random();
            List<int> list = new List<int>()
            {
                rnd.Next(),
                rnd.Next(),
                rnd.Next(),
            };
            InsertStatement insertStatement = new InsertStatement();
            insertStatement.TableName = "randomTable";
            insertStatement.Values = new List<AtomValue>()
            {
                GetAtomValue(list[0]),
                GetAtomValue(list[1]),
                GetAtomValue(list[2]),
            };
            return (insertStatement, list[returnIndex]);
        }

        private static AtomValue GetAtomValue(int key)
        {
            AtomValue value = new AtomValue();
            value.Type = AttributeTypes.Int;
            value.IntegerValue = key;
            return value;
        }

        private static AttributeDeclaration GetAttributeDeclaration(string name)
        {
            AttributeDeclaration attribute = new AttributeDeclaration();
            attribute.AttributeName = name;
            attribute.Type = AttributeTypes.Int;
            return attribute;
        }
    }
}
