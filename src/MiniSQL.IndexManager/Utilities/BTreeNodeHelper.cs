using System;
using System.Collections.Generic;
using System.Diagnostics;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.IndexManager.Utilities
{
    public static class BTreeNodeHelper
    {
        public static BTreeNode GetBTreeNode(Pager pager, int pageNumber)
        {
            MemoryPage page = pager.ReadPage(pageNumber);
            BTreeNode node = new BTreeNode(page);
            return node;
        }

        // level-order
        public static void VisualizeIntegerTree(Pager pager, BTreeNode root)
        {
            Console.WriteLine("[Visualizer] Start");
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
            Console.WriteLine("[Visualizer] End");
        }

        public static void AssertAtomValue(AtomValue v1, AtomValue v2)
        {
            Debug.Assert(v1.Type == v2.Type);
            Debug.Assert(v1.CharLimit == v2.CharLimit);
            Debug.Assert(v1.FloatValue == v2.FloatValue);
            Debug.Assert(v1.IntegerValue == v2.IntegerValue);
            Debug.Assert(v1.StringValue == v2.StringValue);
        }

        public static void AssertDBRecords(DBRecord r1, DBRecord r2)
        {
            List<AtomValue> valuesOut1 = r1.GetValues();
            List<AtomValue> valuesOut2 = r2.GetValues();

            Debug.Assert(valuesOut1.Count == valuesOut2.Count);

            int i;
            for (i = 0; i < valuesOut2.Count; i++)
            {
                AssertAtomValue(valuesOut1[i], valuesOut2[i]);
            }
        }

        public static void AssertCell(BTreeCell c1, BTreeCell c2)
        {
            Debug.Assert(c1.Types == c2.Types);
            AssertDBRecords(c1.Key, c2.Key);
        }
    }
}
