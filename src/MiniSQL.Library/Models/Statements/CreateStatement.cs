using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public enum CreateType
    {
        Table,
        Index,
    }

    // used when declaration, not definition
    // it won't accept concrete value
    public class AttributeDeclaration
    {
        public string AttributeName { get; set; }
        public AttributeType Type { get; set; }
        public int CharLimit { get; set; }
        public bool IsUnique { get; set; }
    }

    public class CreateStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.CreateStatement;
        public CreateType CreateType { get; set; }
        public bool IsUnique { get; set; }
        public string TableName { get; set; }
        // create index only
        public string IndexName { get; set; }
        public string AttributeName { get; set; }
        // create table only
        public List<string> PrimaryKeys { get; set; }
        public List<AttributeDeclaration> AttributeTypePairs { get; set; }
    }
}
