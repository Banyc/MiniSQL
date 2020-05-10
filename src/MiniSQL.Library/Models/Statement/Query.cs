using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    // root of the abstract syntax tree
    public class Query
    {
        public List<IStatement> StatementList { get; set; } = new List<IStatement>();
    }
}
