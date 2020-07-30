using MiniSQL.Library.Models;
using MiniSQL.IndexManager.Models;
using System.Collections.Generic;

namespace MiniSQL.IndexManager.Interfaces
{
    public interface IIndexManager
    {
        /// <summary>
        /// Use when creating table
        /// </summary>
        /// <returns>The root node of the new B+ tree</returns>
        BTreeNode OccupyNewTableNode();
        /// <summary>
        /// Use when dropping table
        /// </summary>
        /// <param name="root">root node of the B+ Tree</param>
        void RemoveTree(BTreeNode root);
        /// <summary>
        /// Insert a row/record/cell
        /// </summary>
        /// <param name="root">the root of the B+ tree</param>
        /// <param name="key">primary key in table tree and indexed column in index tree</param>
        /// <param name="dBRecord">new row of values to insert</param>
        /// <returns>new root node of the B+ tree</returns>
        BTreeNode InsertCell(BTreeNode root, DBRecord key, DBRecord dBRecord);
        /// <summary>
        /// Delete cell(s) that satisfy `conDition`
        /// </summary>
        /// <param name="root">the root of the B+ tree</param>
        /// <param name="condition">condition to satisfy</param>
        /// <param name="keyName">primary key in table tree; indexed value in index tree</param>
        /// <param name="attributeDeclarations">the names of the columns</param>
        /// <returns>new root node of the B+ tree</returns>
        BTreeNode DeleteCells(BTreeNode root, Expression condition, string keyName, List<AttributeDeclaration> attributeDeclarations);
        /// <summary>
        /// Return matches that satisfy `condition`
        /// </summary>
        /// <param name="root">the root of the B+ tree</param>
        /// <param name="condition">condition to satisfy</param>
        /// <param name="keyName">primary key in table tree; indexed value in index tree</param>
        /// <param name="attributeDeclarations">the names of the columns</param>
        /// <returns>matches that satisfy `condition`</returns>
        List<BTreeCell> FindCells(BTreeNode root, Expression condition, string keyName, List<AttributeDeclaration> attributeDeclarations);
        /// <summary>
        /// Find a row/record/cell by the key value
        /// </summary>
        /// <param name="key">primary key in table tree; indexed value in index tree</param>
        /// <param name="root">the root of the B+ tree</param>
        /// <returns>the matched cell</returns>
        BTreeCell FindCell(DBRecord key, BTreeNode root);
        /// <summary>
        /// Enumerate all leaf nodes of the B+ tree
        /// </summary>
        /// <param name="root">the root of the B+ tree</param>
        /// <returns>each leaf node</returns>
        IEnumerable<BTreeCell> LinearSearch(BTreeNode root);
    }
}
