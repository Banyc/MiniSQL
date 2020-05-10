using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public class InsertStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.InsertStatement;
        public string TableName { get; set; }
        public List<AtomValue> Values { get; set; }
    }
}
