using System.Collections.Generic;

namespace MiniSQL.Library.Models
{
    public class SelectResult
    {
        public List<AttributeDeclaration> ColumnDeclarations = null;
        public List<List<AtomValue>> Rows = null;
    }
}
