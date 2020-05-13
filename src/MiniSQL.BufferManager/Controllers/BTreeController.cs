using System;
using MiniSQL.BufferManager.Models;

namespace MiniSQL.BufferManager.Controllers
{
    public class BTreeController
    {
        private readonly Pager _pager;

        // constructor
        public BTreeController(Pager pager)
        {
            this._pager = pager;
        }

        public void InsertCell(BTreeNode root, BTreeCell cell)
        {
            throw new NotImplementedException();
        }

        public void DeleteCell(BTreeNode root, BTreeCell cell)
        {
            throw new NotImplementedException();
        }

        public BTreeCell FindCell(DBRecord key)
        {
            throw new NotImplementedException();
        }

        private void SplitNode(BTreeNode node)
        {
            throw new NotImplementedException();
        }

        // This functionality might no required
        private void MergeNode(BTreeNode leftNode, BTreeNode rightNode)
        {
            throw new NotImplementedException();
        }
    }
}
