using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public class SelectStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.SelectStatement;
        public string FromTable { get; set; } = "";
        // if no condition, it is set to null
        public Expression Condition { get; set; } = null;
    }
}
