namespace MiniSQL.Library.Models
{
    public class DeleteStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.DeleteStatement;
        public string TableName { get; set; }
        // if no condition, it is set to null
        public Expression Condition { get; set; } = null;
    }
}
