using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Utilities
{
    public static class AttributeHelper
    {
        public static List<AttributeValue> GetAttributeValues(List<AttributeDeclaration> declarations, List<AtomValue> values)
        {
            if (declarations.Count != values.Count)
                throw new System.Exception("declarations and values are not in the same size");
            List<AttributeValue> attributes = new List<AttributeValue>();
            int idx = 0;
            while (idx < declarations.Count)
            {
                attributes.Add(new AttributeValue(declarations[idx].AttributeName, values[idx]));
                idx++;
            }
            return attributes;
        }
    }
}
