namespace MiniSQL.Library.Models
{
    public enum ShowType
    {
        Table,
        Index,
        Database
    }

    public class ShowStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.ShowStatement;
        public ShowType ShowType { get; set; }
        public string FromTable { get; set; }
    }
}
