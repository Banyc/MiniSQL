using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.Startup.Controllers
{
    public class View
    {
        private bool isUsingDatabase = false;
        private string nameOfDatabaseInUse;
        private IApi _api;
        private Pager _pager;
        private readonly DatabaseController _databaseController;
        private bool isCtrlC = false;

        public View(DatabaseController databaseController)
        {
            // ensure writing back when ctrl-c
            Console.CancelKeyPress += OnExit;

            // print prologue
            Console.WriteLine();
            Console.WriteLine("Hello MiniSQL!");
            Console.WriteLine();

            _databaseController = databaseController;
        }

        // TODO: wrap this to a class
        private void ChangeContext(string newDatabaseName)
        {
            if (isUsingDatabase)
            {
                _pager.Close();
            }
            // init
            this.isUsingDatabase = true;
            this.nameOfDatabaseInUse = newDatabaseName;
            (_api, _pager) = _databaseController.UseDatabase(newDatabaseName);
        }

        // TODO: wrap this to a class
        private void DropDatabase(string databaseName)
        {
            if (nameOfDatabaseInUse == databaseName)
            {
                _pager.Close();
                isUsingDatabase = false;
            }
            File.Delete($"{databaseName}.minidb");
            File.Delete($"{databaseName}.indices.txt");
            File.Delete($"{databaseName}.tables.txt");
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
                // use database
                if (Regex.IsMatch(line, @"(?i)\s*use\s*database\s*.*(?-i)"))
                {
                    string databaseName = line.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries)[2].TrimEnd(';');
                    ChangeContext(databaseName);
                    continue;
                }
                // drop database
                if (Regex.IsMatch(line, @"(?i)\s*drop\s*database\s*.*(?-i)"))
                {
                    string databaseName = line.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries)[2].TrimEnd(';');
                    DropDatabase(databaseName);
                    continue;
                }
                if (!this.isUsingDatabase)
                {
                    // WORKAROUND
                    Console.WriteLine("[Error] No database in use");
                    continue;
                }
                // flush all the dirty pages back to secondary memory and clean the main memory out of any page
                if (line == "flush")
                {
                    _pager.CleanAllPagesFromMainMemory();
                    continue;
                }
                input.Append(line);
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
                catch (TableOrIndexAlreadyExistsException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (TableOrIndexNotExistsException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (AttributeNotExistsException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (NumberOfAttributesNotMatchsException ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
                catch (TypeOfAttributeNotMatchsException ex)
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
            if (this.isUsingDatabase)
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
