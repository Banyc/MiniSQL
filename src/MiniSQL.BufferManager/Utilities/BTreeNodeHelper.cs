using System;
using System.Collections.Generic;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager.Utilities
{
    public static class BTreeNodeHelper
    {
        public static BTreeNode GetBTreeNode(Pager pager, int rootPage)
        {
            MemoryPage page = pager.ReadPage(rootPage);
            BTreeNode node = new BTreeNode(page);
            return node;
        }

        // level-order
        public static void VisualizeIntegerTree(Pager pager, BTreeNode root)
        {
            int thisLevelwidth = 1;
            int nextLevelwidth = 0;

            Queue<BTreeNode> queue = new Queue<BTreeNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                // visit node
                BTreeNode visitedNode = queue.Dequeue();
                thisLevelwidth--;
                
                int childPage;
                BTreeNode child;
                foreach (var cell in visitedNode)
                {
                    // visit cell
                    Console.Write($"{cell.Key.GetValues()[0].IntegerValue},");
                    // enqueue children
                    switch (cell.Types)
                    {
                        case CellTypes.InternalIndexCell:
                            childPage = (int)((InternalIndexCell)cell).ChildPage;
                            child = BTreeNodeHelper.GetBTreeNode(pager, childPage);
                            queue.Enqueue(child);
                            nextLevelwidth++;
                            break;
                        case CellTypes.InternalTableCell:
                            childPage = (int)((InternalTableCell)cell).ChildPage;
                            child = BTreeNodeHelper.GetBTreeNode(pager, childPage);
                            queue.Enqueue(child);
                            nextLevelwidth++;
                            break;
                    }
                }
                // enqueue right-most child
                switch (visitedNode.PageType)
                {
                    case PageTypes.InternalIndexPage:
                    case PageTypes.InternalTablePage:
                        childPage = (int)visitedNode.RightPage;
                        child = BTreeNodeHelper.GetBTreeNode(pager, childPage);
                        queue.Enqueue(child);
                        nextLevelwidth++;
                        break;
                }
                Console.Write("|");
                if (thisLevelwidth <= 0)
                {
                    Console.WriteLine();
                    thisLevelwidth = nextLevelwidth;
                    nextLevelwidth = 0;
                }
            }
        }
    }
}
