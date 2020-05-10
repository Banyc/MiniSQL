namespace MiniSQL.Library.Models
{
    public class QuitStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.QuitStatement;
    }
}
