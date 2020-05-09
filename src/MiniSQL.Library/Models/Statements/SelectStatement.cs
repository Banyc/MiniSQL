using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public class SelectStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.SelectStatement;
        public string FromTable { get; set; } = "";
        public Expression Condition { get; set; } = null;
    }
}
