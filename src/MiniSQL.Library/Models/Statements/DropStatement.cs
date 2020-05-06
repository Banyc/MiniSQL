using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public enum DropTarget
    {
        Table,
        Index,
    }

    public class DropStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.DropStatement;
        public DropTarget Target { get; set; }
        public List<string> TargetNames { get; set; }
    }
}
