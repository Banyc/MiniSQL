using MiniSQL.Library.Models;

namespace MiniSQL.Interpreter.Models
{
    public enum TableElementType
    {
        PrimaryKey,
        AttributeDeclaration,
    }

    public class TableElement
    {
        public TableElementType Type { get; set; }
        public string PrimaryKey { get; set; }
        public AttributeDeclaration AttributeDeclaration { get; set; }
    }
}
