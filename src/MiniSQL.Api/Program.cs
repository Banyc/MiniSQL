using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Interfaces;
using MiniSQL.BufferManager.Models;
using MiniSQL.CatalogManager;
using MiniSQL.Interpreter;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;
using MiniSQL.RecordManager;

namespace MiniSQL.Api
{
    class Program
    {
        // TODO: view of the whole solution 
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string dbPath = "./testdbfile.minidb";
            // File.Delete(dbPath);
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            IBufferManager bTreeController = new BTreeController(pager, freeList);

            Stopwatch stopwatch = new Stopwatch();

            IInterpreter interpreter = new Parsing();
            ICatalogManager catalogManager = new Catalog();
            // IIndexManager indexManager = new IndexManager();
            IRecordManager recordManager = new RecordContext(pager, bTreeController);

            IApi api = new Api(interpreter, catalogManager, recordManager);

            StringBuilder input = new StringBuilder();

            bool isExit = false;

            while (!isExit)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                input.Append(line);
                if (line == "exit")
                {
                    isExit = true;
                    continue;
                }
                if (!line.TrimEnd().EndsWith(";"))
                {
                    continue;
                }
                stopwatch.Reset();
                stopwatch.Start();
                List<SelectResult> selectResults = api.Query(input.ToString());
                stopwatch.Stop();
                Console.WriteLine($"Time cost: {stopwatch.Elapsed.TotalSeconds}s");
                foreach (var selectResult in selectResults)
                {
                    PrintRows(selectResult);
                    Console.WriteLine();
                }
                input.Clear();
            }

            pager.Close();
        }

        private static void PrintRows(SelectResult result)
        {
            // print names
            foreach (AttributeDeclaration name in result.ColumnDeclarations)
            {
                Console.Write($"{name.AttributeName} | ");
            }
            Console.WriteLine();
            // print rows
            foreach (List<AtomValue> row in result.Rows)
            {
                foreach (AtomValue value in row)
                {
                    switch (value.Type)
                    {
                        case AttributeTypes.Int:
                            Console.Write($"{value.IntegerValue} | ");
                            break;
                        case AttributeTypes.Char:
                            Console.Write($"{value.StringValue} | ");
                            break;
                        case AttributeTypes.Float:
                            Console.Write($"{value.FloatValue} | ");
                            break;
                        case AttributeTypes.Null:
                            Console.Write($"NULL | ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
