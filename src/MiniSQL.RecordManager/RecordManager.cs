using System.Collections.Generic;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Interfaces;
using MiniSQL.BufferManager.Models;
using MiniSQL.BufferManager.Utilities;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.RecordManager
{
    // TODO: review + test
    public class RecordManager : IRecordManager
    {
        private readonly IBufferManager _bTree;
        private readonly Pager _pager;

        public RecordManager(Pager pager, IBufferManager bTreeController)
        {
            _pager = pager;
            _bTree = bTreeController;
        }

        public int CreateTable(CreateStatement createStatement)
        {
            return _bTree.OccupyNewTableNode();
        }

        // public int DeleteRecords(DeleteStatement deleteStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage)
        // {
        //     BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
        //     return _bTree.DeleteCells(node, deleteStatement.Condition, primaryKeyName, attributeDeclarations);
        // }

        public int DeleteRecords(List<AtomValue> primaryKeys, int rootPage)
        {
            throw new System.NotImplementedException();
        }

        public void DropTable(int rootPage)
        {
            MemoryPage page = _pager.ReadPage(rootPage);
            BTreeNode node = new BTreeNode(page);
            _bTree.RemoveTree(node);
        }

        // return new root page number
        // public int InsertRecord(InsertStatement insertStatement, AtomValue key, int rootPage)
        // {
        //     BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
        //     DBRecord wrappedKey = new DBRecord(new List<AtomValue>() { key });
        //     DBRecord values = new DBRecord(insertStatement.Values);
        //     return _bTree.InsertCell(node, wrappedKey, values);
        // }

        public List<List<AtomValue>> SelectRecords(SelectStatement selectStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage)
        {
            BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
            List<BTreeCell> cells = _bTree.FindCells(node, selectStatement.Condition, primaryKeyName, attributeDeclarations);
            List<List<AtomValue>> rows = new List<List<AtomValue>>();
            foreach (BTreeCell cell in cells)
            {
                List<AtomValue> row = ((LeafTableCell)cell).DBRecord.GetValues();
                rows.Add(row);
            }
            return rows;
        }

        public List<List<AtomValue>> SelectRecords(List<AtomValue> primaryKeys, int rootPage)
        {
            throw new System.NotImplementedException();
        }
    }
}
