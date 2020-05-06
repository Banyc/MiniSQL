namespace MiniSQL.Library.Models
{
    public class DeleteStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.DeleteStatement;
        public string TableName { get; set; }
        public Expression Condition { get; set; }
    }
}
