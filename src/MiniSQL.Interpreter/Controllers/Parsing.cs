using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.Interpreter.Controllers
{
    public class Parsing : IInterpreter
    {
        public Query GetQuery(string input)
        {
            return StaticGetQuery(input);
        }
        
        public static Query StaticGetQuery(string input)
        {
            ICharStream stream = CharStreams.fromString(input);
            ITokenSource lexer = new MiniSQLLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            MiniSQLParser parser = new MiniSQLParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();
            
            MiniSQLVisitor visitor = new MiniSQLVisitor();
            Query query = (Query)visitor.Visit(tree);

            // import file
            int index = 0;
            while (index < query.StatementList.Count)
            {
                IStatement statement = query.StatementList[index];
                if (statement.Type == StatementType.ExecFileStatement)
                {
                    ExecFileStatement execFile = (ExecFileStatement)statement;
                    query.StatementList.RemoveAt(index);
                    string fileText = File.ReadAllText(execFile.FilePath);
                    Query ret = StaticGetQuery(fileText);
                    query.StatementList.InsertRange(index, ret.StatementList);
                    continue;
                }
                index++;
            }
            return query;
        } 
    }
}
