﻿using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace MiniSQL.Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            // string input = File.ReadAllText("./Grammar/tests/index-create-delete-0.sql");
            // string input = File.ReadAllText("./Grammar/tests/table-insert-delete-0.sql");
            string input = File.ReadAllText("./Grammar/tests/execfile.sql");
            
            var query = Parsing.GetQuery(input);
            
            Console.WriteLine("Unit test end.");
        }
    }
}
