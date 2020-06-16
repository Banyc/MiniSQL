using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.CatalogManager;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.Interpreter;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;
using MiniSQL.RecordManager;

namespace MiniSQL.Api.Controllers
{
    public class View
    {
        private readonly IIndexManager _bTreeController;
        private readonly IInterpreter _interpreter;
        private readonly ICatalogManager _catalogManager;
        private readonly IRecordManager _recordManager;
        private readonly IApi _api;
        private readonly Pager _pager;
        private bool isCtrlC = false;

        public View(IIndexManager bTreeController, IInterpreter interpreter, ICatalogManager catalogManager, IRecordManager recordManager, IApi api, Pager pager)
        {
            // ensure writing back when ctrl-c
            Console.CancelKeyPress += OnExit;

            // print prologue
            Console.WriteLine();
            Console.WriteLine("Hello MiniSQL!");
            Console.WriteLine();
            
            // init
            _pager = pager;
            _bTreeController = bTreeController;
            _interpreter = interpreter;
            _catalogManager = catalogManager;
            // IIndexManager indexManager = new IndexManager();
            _recordManager = recordManager;
            _api = api;
        }

        // interactive(blocking) view of the whole solution 
        public void Interactive()
        {
            // init
            Stopwatch stopwatch = new Stopwatch();
            // read input
            StringBuilder input = new StringBuilder();
            bool isExit = false;
            while (!isExit && !this.isCtrlC)
            {
                // read input
                Console.Write("> ");
                // change color to yellow
                ConsoleColor defaultColor;
                defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                string line = Console.ReadLine();
                // restore the previous color
                Console.ForegroundColor = defaultColor;
                input.Append(line);
                // ctrl-c pressed
                if (line == null)
                {
                    return;
                }
                // exit when "exit"
                if (line == "exit")
                {
                    isExit = true;
                    continue;
                }
                // perform SQL for each input when the last line ends with ';'
                if (!line.TrimEnd().EndsWith(";"))
                {
                    // compensate '\n'
                    input.Append("\n");
                    continue;
                }
                // execute SQL
                stopwatch.Reset();
                stopwatch.Start();
                try
                {
                    List<SelectResult> selectResults = _api.Query(input.ToString());
                    // print results for select statement
                    foreach (var selectResult in selectResults)
                    {
                        PrintRows(selectResult);
                        Console.WriteLine();
                    }
                }
                catch (StatementPreCheckException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (KeyNotExistException ex)
                {   
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (RepeatedKeyException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                stopwatch.Stop();
                // print time consumed
                defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Time cost: {stopwatch.Elapsed.TotalSeconds}s");
                Console.ForegroundColor = defaultColor;
                // clear input to the interpreter
                input.Clear();
            }
            // save database file
            _pager.Close();
        }

        // BUG: NOT WORKING
        private void OnExit(object sender, EventArgs e)
        {
            isCtrlC = true;
            _pager.Close();
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
                Console.Write($" {string.Format(format, name.AttributeName)} ");
                if (columnIndex < result.ColumnDeclarations.Count - 1)
                    Print("|", ConsoleColor.DarkGray);
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
                    Console.Write($" {string.Format(format, stringToPrint)} ");
                    if (columnIndex < row.Count - 1)
                        Print("|", ConsoleColor.DarkGray);
                    columnIndex++;
                }
                Console.WriteLine();
            }
            // restore the previous color
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

        private static void Print(string toPrint, ConsoleColor color)
        {
            // change color
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            // print
            Console.Write(toPrint);
            // restore the previous color
            Console.ForegroundColor = defaultColor;
        }
    }
}