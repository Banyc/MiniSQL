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
        public int CharLimit { get; set; } = 1;
        public bool IsUnique { get; set; } = false;
    }

    public class CreateStatement : IStatement
    {
        public StatementType Type { get; set; } = StatementType.CreateStatement;
        public CreateType CreateType { get; set; }
        public bool IsUnique { get; set; } = false;
        public string TableName { get; set; }
        // create index only
        public string IndexName { get; set; }
        public string AttributeName { get; set; }
        // create table only
        public string PrimaryKey { get; set; }
        public List<AttributeDeclaration> AttributeDeclarations { get; set; }
    }
}
