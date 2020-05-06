using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public enum CreateType
    {
        Table,
        Index,
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
