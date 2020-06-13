using MiniSQL.Library.Models;
using MiniSQL.BufferManager.Models;
using System.Collections.Generic;

namespace MiniSQL.BufferManager.Interfaces
{
    public interface IBufferManager
    {
        // when create table
        int OccupyNewTableNode();
        // when drop table
        void RemoveTree(BTreeNode root);
        // insert cell
        BTreeNode InsertCell(BTreeNode Root, DBRecord key, DBRecord dBRecord);
        // delete cell(s) that satisfy `expression`
        BTreeNode DeleteCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
        // return matches that satisfy `expression`
        List<BTreeCell> FindCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
    }
}
