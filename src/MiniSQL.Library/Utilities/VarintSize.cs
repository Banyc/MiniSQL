using System;

namespace MiniSQL.Library.Utilities
{
    public class VarintSize
    {
        public static int GetVarintSize(uint number)
        {
            (byte[] _, VarintType type) = VarintBitConverter.ToVarint(number);
                if (type == VarintType.Varint8)
                    return 1;
                else if (type == VarintType.Varint32)
                    return 4;
                else
                    throw new Exception($"Varint type {type} does not exist");
        }
    }
}
