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
                // read input
                Console.Write("> ");
                string line = Console.ReadLine();
                input.Append(line);
                // exit when "exit"
                if (line == "exit")
                {
                    isExit = true;
                    continue;
                }
                // perform SQL for each input when the last line ends with ';'
                if (!line.TrimEnd().EndsWith(";"))
                {
                    continue;
                }
                // execute SQL
                stopwatch.Reset();
                stopwatch.Start();
                List<SelectResult> selectResults = api.Query(input.ToString());
                stopwatch.Stop();
                // print results for select statement
                foreach (var selectResult in selectResults)
                {
                    PrintRows(selectResult);
                    Console.WriteLine();
                }
                // print time consumed
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Time cost: {stopwatch.Elapsed.TotalSeconds}s");
                Console.ForegroundColor = defaultColor;
                // clear input to the interpreter
                input.Clear();
            }

            pager.Close();
        }

        private static void PrintRows(SelectResult result)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            // print names
            foreach (AttributeDeclaration name in result.ColumnDeclarations)
            {
                Console.Write($"{name.AttributeName}\t|\t");
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            // print rows
            foreach (List<AtomValue> row in result.Rows)
            {
                foreach (AtomValue value in row)
                {
                    switch (value.Type)
                    {
                        case AttributeTypes.Int:
                            Console.Write($"{value.IntegerValue}\t|\t");
                            break;
                        case AttributeTypes.Char:
                            Console.Write($"\"{value.StringValue}\"\t|\t");
                            break;
                        case AttributeTypes.Float:
                            Console.Write($"{value.FloatValue.ToString("0.##")}\t|\t");
                            break;
                        case AttributeTypes.Null:
                            Console.Write($"NULL\t|\t");
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = defaultColor;
        }
    }
}
