using MiniSQL.Library.Models;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Interfaces
{
    public interface IBufferManager
    {
        void InsertCell(BTreeNode root, BTreeCell cell);

        void DeleteCell(BTreeNode root, BTreeCell cell);

        BTreeCell FindCell(DBRecord key);
    }
}
