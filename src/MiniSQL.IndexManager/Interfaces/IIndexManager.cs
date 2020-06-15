using MiniSQL.Library.Models;
using MiniSQL.IndexManager.Models;
using System.Collections.Generic;

namespace MiniSQL.IndexManager.Interfaces
{
    public interface IIndexManager
    {
        // when create table
        BTreeNode OccupyNewTableNode();
        // when drop table
        void RemoveTree(BTreeNode root);
        // insert cell
        BTreeNode InsertCell(BTreeNode Root, DBRecord key, DBRecord dBRecord);
        // delete cell(s) that satisfy `expression`
        BTreeNode DeleteCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
        // return matches that satisfy `expression`
        List<BTreeCell> FindCells(BTreeNode root, Expression expression, string keyName, List<AttributeDeclaration> attributeDeclarations);
        BTreeCell FindCell(DBRecord key, BTreeNode root);
        System.Collections.Generic.IEnumerable<BTreeCell> LinearSearch(BTreeNode root);
    }
}
