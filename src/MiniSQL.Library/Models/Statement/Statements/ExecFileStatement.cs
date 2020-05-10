namespace MiniSQL.Library.Models
{
    public class ExecFileStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.ExecFileStatement;
        public string FilePath  { get; set; } = "";
    }
}
