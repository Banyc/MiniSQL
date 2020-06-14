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
            // print prologue
            Console.WriteLine();
            Console.WriteLine("Hello MiniSQL!");
            Console.WriteLine();
            // init
            string dbPath = "./testdbfile.minidb";
            Pager pager = new Pager(dbPath);
            FreeList freeList = new FreeList(pager);
            IBufferManager bTreeController = new BTreeController(pager, freeList);
            Stopwatch stopwatch = new Stopwatch();
            IInterpreter interpreter = new Parsing();
            ICatalogManager catalogManager = new Catalog();
            // IIndexManager indexManager = new IndexManager();
            IRecordManager recordManager = new RecordContext(pager, bTreeController);
            IApi api = new Api(interpreter, catalogManager, recordManager);
            // read input
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
            // save database file
            pager.Close();
        }

        private static void PrintRows(SelectResult result)
        {
            // get size of each column
            List<int> sizes = GetColumnSize(result);
            // change color
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            // print names
            int columnIndex = 0;
            foreach (AttributeDeclaration name in result.ColumnDeclarations)
            {
                string format = "{0, " + (-sizes[columnIndex]).ToString() + "}";
                Console.Write($" {string.Format(format, name.AttributeName)} |");
                columnIndex++;
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            // print rows
            foreach (List<AtomValue> row in result.Rows)
            {
                columnIndex = 0;
                foreach (AtomValue value in row)
                {
                    string format = "{0, " + sizes[columnIndex].ToString() + "}";
                    string stringToPrint = "";
                    switch (value.Type)
                    {
                        case AttributeTypes.Int:
                            stringToPrint = $"{value.IntegerValue}";
                            break;
                        case AttributeTypes.Char:
                            stringToPrint = $"\"{value.StringValue}\"";
                            break;
                        case AttributeTypes.Float:
                            stringToPrint = $"{value.FloatValue.ToString("0.##")}";
                            break;
                        case AttributeTypes.Null:
                            stringToPrint = $"NULL";
                            break;
                    }
                    Console.Write($" {string.Format(format, stringToPrint)} |");
                    columnIndex++;
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = defaultColor;
        }

        private static List<int> GetColumnSize(SelectResult result)
        {
            List<int> sizes = new List<int>();
            foreach (AttributeDeclaration name in result.ColumnDeclarations)
            {
                sizes.Add(name.AttributeName.Length);
            }
            foreach (List<AtomValue> row in result.Rows)
            {
                int columnIndex = 0;
                foreach (AtomValue value in row)
                {
                    int size = 0;
                    switch (value.Type)
                    {
                        case AttributeTypes.Int:
                            size = value.IntegerValue.ToString().Length;
                            break;
                        case AttributeTypes.Char:
                            size = $"\"{value.StringValue}\"".Length;
                            break;
                        case AttributeTypes.Float:
                            size = value.FloatValue.ToString("0.##").Length;
                            break;
                        case AttributeTypes.Null:
                            size = "NULL".Length;
                            break;
                    }
                    if (sizes[columnIndex] < size)
                        sizes[columnIndex] = size;
                    columnIndex++;
                }
            }
            return sizes;
        }
    }
}
