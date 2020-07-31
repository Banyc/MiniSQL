using System;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;
using MiniSQL.Library.Exceptions;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
        /// <summary>
        /// Insert a row/record/cell.
        /// </summary>
        /// <param name="root">the root of the B+ tree.</param>
        /// <param name="key">primary key in table tree and indexed column in index tree.</param>
        /// <param name="dBRecord">new row of values to insert.</param>
        /// <returns>new root node of the B+ tree</returns>
        public BTreeNode InsertCell(BTreeNode root, DBRecord key, DBRecord dBRecord)
        {
            // create a new tree
            if (root == null)
            {
                BTreeNode newRootFromNull = GetNewNode(PageTypes.LeafTablePage);
                LeafTableCell newCell = new LeafTableCell(key, dBRecord);
                newRootFromNull.InsertBTreeCell(newCell);
                return newRootFromNull;
            }
            // no need to split
            if (root.NumCells < MaxCell)
            {
                InsertNonFull(root, key, dBRecord);
                return root;
            }

            // need to split, and return with a new root
            BTreeNode newRoot = GetNewNode(PageTypes.InternalTablePage);
            root.ParentPage = (uint)newRoot.RawPage.PageNumber;
            SplitNode(root, newRoot, key);
            InsertNonFull(newRoot, key, dBRecord);
            return newRoot;
        }

        /// <summary>
        /// Split `nodeTobeSplit`, which has a `parantNode`.
        /// </summary>
        /// <param name="nodeTobeSplit">node to be split.</param>
        /// <param name="parantNode">parent node of `nodeTobeSplit`.</param>
        /// <param name="newKey">the value of new key. This could affect where to split.</param>
        private void SplitNode(BTreeNode nodeTobeSplit, BTreeNode parantNode, DBRecord newKey)
        {
            int i;
            // new node will be appended to `nodeTobeSplit`
            BTreeNode splitNode = GetNewNode(nodeTobeSplit.PageType);
            // tmp is used for the change of parentPage
            InternalTableCell tmpCell;
            MemoryPage tmpPage;

            // key should be the primary key to be inserted in the parent node
            int deleteIndex = nodeTobeSplit.NumCells / 2;
            // right bound of the left node
            DBRecord rightmostKeyInLeftNode = nodeTobeSplit[deleteIndex - 1].Key;
            if ((newKey.GetValues()[0] >= rightmostKeyInLeftNode.GetValues()[0]).BooleanValue)
            {
                // the new key should be put at the split node on the right
                deleteIndex++;
                rightmostKeyInLeftNode = nodeTobeSplit[deleteIndex - 1].Key;
            }
            // `deleteIndex` is now pointing to the first cell of the pending right node

            // loop through the right part of `nodeTobeSplit`
            for (i = deleteIndex; i < this.MaxCell; i++)
            {
                ushort deleteOffSet = nodeTobeSplit.CellOffsetArray[deleteIndex];
                // change the parent Page fields of their children
                if (nodeTobeSplit.PageType == PageTypes.InternalTablePage)
                {
                    tmpCell = (InternalTableCell)nodeTobeSplit.GetBTreeCell(deleteOffSet);
                    tmpPage = _pager.ReadPage((int)tmpCell.ChildPage);
                    new BTreeNode(tmpPage)
                    {
                        ParentPage = (uint)splitNode.RawPage.PageNumber
                    };
                }
                // move the cells in `nodeTobeSplit` belonging to the pending right node to `splitNode`
                splitNode.InsertBTreeCell(nodeTobeSplit.GetBTreeCell(deleteOffSet));
                nodeTobeSplit.DeleteBTreeCell(deleteOffSet);
            }
            // new cell to be inserted to the parent node of `nodeTobeSplit`
            // If the `parentNode` need to spilt, this will be wrong. However, it could be ensured not happenning because it is a top-down process.
            InternalTableCell newCellToAscend = new InternalTableCell(rightmostKeyInLeftNode, (uint)nodeTobeSplit.RawPage.PageNumber);

            // make alias for readability
            BTreeNode leftNode = nodeTobeSplit;
            BTreeNode rightNode = splitNode;
            // connect two nodes `leftNode` and `rightNode` by `RightPage` pointer
            rightNode.RightPage = leftNode.RightPage;
            if (leftNode.PageType == PageTypes.LeafTablePage)
            {
                leftNode.RightPage = (uint)rightNode.RawPage.PageNumber;
            }
            // if `leftNode` is an internal node, the middle cell in the original `nodeTobeSplit` (the rightmost cell in the now `leftNode`) has to be deleted
            else if (leftNode.PageType == PageTypes.InternalTablePage)
            {
                // for internal node, the `ParentPage` of child page need to be changed
                // the rightmost child of `leftNode` sets its parent to `rightNode`
                tmpPage = _pager.ReadPage((int)leftNode.RightPage);
                new BTreeNode(tmpPage)
                {
                    ParentPage = (uint)rightNode.RawPage.PageNumber
                };

                // the rightmost Cell In the pending Left Node
                InternalTableCell rightmostCellInLeftNode = (InternalTableCell)leftNode[deleteIndex - 1];
                // set the rightmost child of `leftNode` to the left child of `rightmostCellInLeftNode`
                leftNode.RightPage = rightmostCellInLeftNode.ChildPage;
                // leftNode.DeleteBTreeCell(rightmostCellInLeftNode);  // the same as the code below, but the code below runs faster
                leftNode.DeleteBTreeCell(leftNode.CellOffsetArray[deleteIndex - 1]);

                // the deleted rightmost cell in left node sets the parent of its left child to that left node
                tmpPage = _pager.ReadPage((int)rightmostCellInLeftNode.ChildPage);
                new BTreeNode(tmpPage)
                {
                    ParentPage = (uint)leftNode.RawPage.PageNumber
                };
            }

            rightNode.ParentPage = (uint)parantNode.RawPage.PageNumber;

            // reconnect the particular cell (or right) in the parent node because of the change of the child node
            // This is a new empty root
            if (parantNode.NumCells == 0)
            {
                parantNode.RightPage = (uint)rightNode.RawPage.PageNumber;
            }
            // There are some cell in the parents node
            else
            {
                BTreeCell cell;
                (cell, _, _) = parantNode.FindBTreeCell(rightmostKeyInLeftNode);
                // The case when the node to be inserted into parent node is in the rightest position
                if (cell == null)
                {
                    parantNode.RightPage = (uint)rightNode.RawPage.PageNumber;
                }
                // The normal case
                else
                {
                    InternalTableCell tmp_cell = new InternalTableCell(cell.Key, (uint)rightNode.RawPage.PageNumber);
                    parantNode.DeleteBTreeCell(cell);
                    parantNode.InsertBTreeCell(tmp_cell);
                }
            }
            // This must be done after we reconnect the treeNode
            parantNode.InsertBTreeCell(newCellToAscend);
        }

        /// <summary>
        /// <para>Insert cell into a non-full node `node` if `node` is a leaf node.</para>
        /// <para>If `node` is not the leaf node, the child node of `node` might need to split before recursively performing this function to the child node.</para>
        /// <para>`node` should be ensured to be not full.</para>
        /// </summary>
        /// <param name="node">node being inserted</param>
        /// <param name="newKey">new key value</param>
        /// <param name="dBRecord">new row/record</param>
        /// <returns>new root node</returns>
        private BTreeNode InsertNonFull(BTreeNode node, DBRecord newKey, DBRecord dBRecord)
        {
            // the actual insertion performs in leaf node
            if (node.PageType == PageTypes.LeafTablePage)
            {
                BTreeCell check_repeat;
                (check_repeat, _, _) = node.FindBTreeCell(newKey, false);
                if (check_repeat != null)
                {
                    throw new RepeatedKeyException($"The primary key to be inserted ({newKey.GetValues()[0]}) is repeated!");
                }
                LeafTableCell newCell = new LeafTableCell(newKey, dBRecord);
                node.InsertBTreeCell(newCell);
                return node;
            }

            // find the child node to try to insert
            BTreeNode child;
            BTreeCell cell;
            UInt16 offset;
            (cell, offset, _) = node.FindBTreeCell(newKey);
            MemoryPage nextpage;
            if (offset == 0)  // rightest page
            {
                nextpage = _pager.ReadPage((int)node.RightPage);
                child = new BTreeNode(nextpage);
            }
            else
            {
                InternalTableCell internalTableCell = (InternalTableCell)cell;
                nextpage = _pager.ReadPage((int)internalTableCell.ChildPage);
                child = new BTreeNode(nextpage);
            }
            // child needs to split
            if (child.NumCells >= MaxCell)
            {
                SplitNode(child, node, newKey);
                return InsertNonFull(node, newKey, dBRecord);
            }
            return InsertNonFull(child, newKey, dBRecord);
        }
    }
}
