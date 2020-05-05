using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public enum CreateTarget
    {
        Table,
        Index,
    }

    public enum AttributeType
    {
        Int,
        Char,
        Float,
    }

    public class AttributeTypePair
    {
        public string AttributeName { get; set; }
        public AttributeType Type { get; set; }
        public int CharLimit  { get; set; }
    }

    public class CreateStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.CreateStatement;
        public CreateTarget Target { get; set; }
        public bool IsUnique { get; set; }
        public string TableName { get; set; }
        // create index only
        public List<string> Attributes { get; set; }
        // create table only
        public List<string> PrimaryKeys { get; set; }
        public List<AttributeTypePair> AttributeTypePairs { get; set; }
    }
}
