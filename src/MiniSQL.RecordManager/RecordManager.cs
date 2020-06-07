using System.Collections.Generic;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Interfaces;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.RecordManager
{
    // TODO: review + test
    public class RecordManager : IRecordManager
    {
        private readonly IBufferManager _bTree;
        private readonly Pager _pager;

        public RecordManager(IBufferManager bTreeController)
        {
            _bTree = bTreeController;
        }

        public int CreateTable(CreateStatement createStatement)
        {
            throw new System.NotImplementedException();
        }

        public int DeleteRecords(DeleteStatement deleteStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage)
        {
            BTreeNode node = GetBTreeNode(rootPage);
            return _bTree.DeleteCells(node, deleteStatement.Condition, primaryKeyName, attributeDeclarations);
        }

        public int DeleteRecords(List<AtomValue> primaryKeys, int rootPage)
        {
            throw new System.NotImplementedException();
        }

        public void DropTable(int rootPage)
        {
            throw new System.NotImplementedException();
        }

        // return new root page number
        public int InsertRecord(InsertStatement insertStatement, AtomValue key, int rootPage)
        {
            BTreeNode node = GetBTreeNode(rootPage);
            DBRecord wrappedKey = new DBRecord(new List<AtomValue>() { key });
            DBRecord values = new DBRecord(insertStatement.Values);
            BTreeCell cell = new LeafTableCell(wrappedKey, values);
            return _bTree.InsertCell(node, cell);
        }

        public List<List<AtomValue>> SelectRecords(SelectStatement selectStatement, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage)
        {
            BTreeNode node = GetBTreeNode(rootPage);
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

        private BTreeNode GetBTreeNode(int rootPage)
        {
            MemoryPage page = _pager.ReadPage(rootPage);
            BTreeNode node = new BTreeNode(page);
            return node;
        }
    }
}
