using MiniSQL.Library.Models;
using MiniSQL.BufferManager.Models;
using System.Collections.Generic;

namespace MiniSQL.BufferManager.Interfaces
{
    public interface IBufferManager
    {
        void InsertCell(BTreeNode root, BTreeCell cell);

        void DeleteCell(BTreeNode root, DBRecord key);
        
        void DeleteCells(BTreeNode root, Expression expression);

        BTreeCell FindCell(DBRecord key);

        List<BTreeCell> FindCells(Expression expression);
    }
}
