using System;
using System.Linq;

namespace MiniSQL.Library.Utilities
{
    public enum VarintType
    {
        Varint8,
        Varint32,
    }

    public class VarintBitConverter
    {
        // it could only distinguish unsigned varint32 and varint8
        public static (uint, VarintType) FromVarint(byte[] data, int startIndex)
        {
            VarintType type;
            uint value;
            // most significant bit is 1
            if ((data[startIndex] & 0x80) != 0)
            {
                type = VarintType.Varint32;

                value = VarintBitConverter.ToUInt32(data, startIndex);
            }
            // most significant bit is 0
            else
            {
                type = VarintType.Varint8;
                value = data[startIndex];
            }

            return (value, type);
        }

        // it could only distinguish unsigned varint32 and varint8
        public static byte[] ToVarint(uint value, VarintType type)
        {
            if (type == VarintType.Varint8)
                return VarintBitConverter.GetVarintBytes((byte)value);
            else
                return VarintBitConverter.GetVarintBytes(value);
        
        }
        // it could only distinguish unsigned varint32 and varint8
        public static (byte[], VarintType) ToVarint(uint value)
        {
            if (value < 0x80)
                return (VarintBitConverter.GetVarintBytes((byte)value), VarintType.Varint8);
            else
                return (VarintBitConverter.GetVarintBytes(value), VarintType.Varint32);
        }

        public static byte[] GetVarintBytes(byte value)
        {
            byte[] result = new byte[1];
            result[0] = value;
            return result;
        }

        public static byte[] GetVarintBytes(uint value)
        {
            byte[] result = new byte[4];

            uint aux;

            aux = value & 0x0000007F;
            aux |= ((value & 0x00003F80) << 1) | 0x00008000;
            aux |= ((value & 0x001FC000) << 2) | 0x00800000;
            aux |= ((value & 0x0FE00000) << 3) | 0x80000000;

            result[3] = (byte)(aux);
            result[2] = (byte)(aux >> 8);
            result[1] = (byte)(aux >> 16);
            result[0] = (byte)(aux >> 24);

            return result;
        }

        public static uint ToUInt32(byte[] data, int startIndex)
        {
            uint result = 0;

            result = ((uint)data[startIndex + 3] & 0x7f);
            result |= ((uint)data[startIndex + 2] & 0x7f) << 7;
            result |= ((uint)data[startIndex + 1] & 0x7f) << 14;
            result |= ((uint)data[startIndex + 0] & 0x7f) << 21;

            return result;
        }
    }
}
