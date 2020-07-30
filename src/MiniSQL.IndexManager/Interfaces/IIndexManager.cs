using MiniSQL.Library.Models;
using MiniSQL.IndexManager.Models;
using System.Collections.Generic;
using System;

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
        // delete cell(s) that satisfy `condition`
        // `keyName` := primary key in table tree; indexed value in index tree
        BTreeNode DeleteCells(BTreeNode root, Expression condition, string keyName, List<AttributeDeclaration> attributeDeclarations);
        // return matches that satisfy `condition`
        // `keyName` := primary key in table tree; indexed value in index tree
        List<BTreeCell> FindCells(BTreeNode root, Expression condition, string keyName, List<AttributeDeclaration> attributeDeclarations);
        BTreeCell FindCell(DBRecord key, BTreeNode root);
        System.Collections.Generic.IEnumerable<BTreeCell> LinearSearch(BTreeNode root);
    }
}
