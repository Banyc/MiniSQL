using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public class InsertStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.DeleteStatement;
        public string TableName { get; set; }
        public List<AttributeValue> Values { get; set; }
    }
}
