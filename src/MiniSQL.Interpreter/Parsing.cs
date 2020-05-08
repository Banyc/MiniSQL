using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MiniSQL.Library.Models;

namespace MiniSQL.Interpreter
{
    public class Parsing
    {
        public static Query GetQuery(string input)
        {
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new MiniSQLLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            MiniSQLParser parser = new MiniSQLParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();
            
            MiniSQLVisitor visitor = new MiniSQLVisitor();
            Query query = (Query)visitor.Visit(tree);
            return query;
        } 
    }
}
