namespace MiniSQL.Library.Models
{
    public enum StatementType
    {
        // DDL
        CreateStatement,
        DropStatement,

        // DML
        DeleteStatement,
        InsertStatement,
        SelectStatement,

        // Custom
        QuitStatement,
        ExecFileStatement,
    }

    public interface IStatement
    {
        StatementType Type { get; set; }
    }
}
