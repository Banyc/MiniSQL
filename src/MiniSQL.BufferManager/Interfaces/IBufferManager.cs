using MiniSQL.Library.Models;
using MiniSQL.BufferManager.Models;
using System.Collections.Generic;

namespace MiniSQL.BufferManager.Interfaces
{
    public interface IBufferManager
    {
        // when create table
        int OccupyNewNode();
        // when drop table
        void RemoveTree(BTreeNode root);
        // insert cell
        int InsertCell(BTreeNode root, BTreeCell cell);
        // delete cell(s) that satisfy `expression`
        int DeleteCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
        // return matches that satisfy `expression`
        List<BTreeCell> FindCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
    }
}
