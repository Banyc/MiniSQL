using System.Collections.Generic;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Interfaces;
using MiniSQL.IndexManager.Models;
using MiniSQL.IndexManager.Utilities;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.RecordManager.Controllers
{
    // TODO: review + test
    public class RecordContext : IRecordManager
    {
        private readonly IIndexManager _bTree;
        private readonly Pager _pager;

        public RecordContext(Pager pager, IIndexManager bTreeController)
        {
            _pager = pager;
            _bTree = bTreeController;
        }

        public int CreateTable()
        {
            return _bTree.OccupyNewTableNode().RawPage.PageNumber;
        }

        public int CreateIndex(int tableRootPage, string indexedColumnName, List<AttributeDeclaration> attributeDeclarations)
        {
            BTreeNode indexRoot = _bTree.OccupyNewTableNode();
            BTreeNode tableRoot = BTreeNodeHelper.GetBTreeNode(_pager, tableRootPage);

            int indexedColumnIndex = attributeDeclarations.FindIndex(x => x.AttributeName == indexedColumnName);

            foreach (BTreeCell tableCell in _bTree.LinearSearch(tableRoot))
            {
                AtomValue indexedValue = ((LeafTableCell)tableCell).DBRecord.GetValues()[indexedColumnIndex];
                AtomValue primaryKey = ((LeafTableCell)tableCell).Key.GetValues()[0];
                // List<AtomValue> indexPrimaryKeyPair = new List<AtomValue>() { indexedValue, primaryKey };
                List<AtomValue> wrappedPrimaryKey = new List<AtomValue>() { primaryKey };
                DBRecord wrappedKey = new DBRecord(new List<AtomValue>() { indexedValue });
                // DBRecord wrappedValues = new DBRecord(indexPrimaryKeyPair);
                DBRecord wrappedValues = new DBRecord(wrappedPrimaryKey);
                indexRoot = _bTree.InsertCell(indexRoot, wrappedKey, wrappedValues);
            }

            return indexRoot.RawPage.PageNumber;
        }

        public int DeleteRecords(Expression condition, string primaryKeyName, List<AttributeDeclaration> attributeDeclarations, int rootPage)
        {
            BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
            BTreeNode newRoot = _bTree.DeleteCells(node, condition, primaryKeyName, attributeDeclarations);
            return newRoot.RawPage.PageNumber;
        }

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
        public int InsertRecord(List<AtomValue> values, AtomValue key, int rootPage)
        {
            BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
            DBRecord wrappedKey = new DBRecord(new List<AtomValue>() { key });
            DBRecord wrappedValues = new DBRecord(values);
            BTreeNode newRoot = _bTree.InsertCell(node, wrappedKey, wrappedValues);
            return newRoot.RawPage.PageNumber;
        }

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

        // return null if not found
        public List<AtomValue> SelectRecord(AtomValue key, int rootPage)
        {
            List<AtomValue> wrapper = new List<AtomValue> { key };
            DBRecord keyDBRecord = new DBRecord(wrapper);
            BTreeNode node = BTreeNodeHelper.GetBTreeNode(_pager, rootPage);
            BTreeCell cell = _bTree.FindCell(keyDBRecord, node);
            List<AtomValue> result = null;
            result = ((LeafTableCell)cell)?.DBRecord.GetValues();
            return result;
        }
    }
}
