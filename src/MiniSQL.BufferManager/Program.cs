using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;
using MiniSQL.Library.Models;

namespace MiniSQL.BufferManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[BufferManager] Test Begin");

            TestPager();

            TestPagerSwapping();

            TestFreeList();

            Console.WriteLine("[BufferManager] Test End");
        }

        static void TestFreeList()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 100);

            List<MemoryPage> pageList = new List<MemoryPage>();
            pageList.Add(pager.GetNewPage());  // page #2
            pageList.Add(pager.GetNewPage());  // page #3
            pageList.Add(pager.GetNewPage());  // page #4
            pageList.Add(pager.GetNewPage());  // page #5

            FreeList freeList = new FreeList(pager);

            // test initialization
            MemoryPage newPage = null;
            newPage = freeList.AllocatePage();  // freeList is now empty
            Debug.Assert(newPage == null);

            // recycle pages
            // MemoryPage tempPage = pager.ReadPage(3);
            freeList.RecyclePage(pageList[2]);  // freeList->4
            freeList.RecyclePage(pageList[1]);  // freeList->3->4
            freeList.RecyclePage(pageList[3]);  // freeList->5->3->4

            // fetch page from free list
            newPage = freeList.AllocatePage();  // freeList->3->4
            Debug.Assert(newPage.PageNumber == 5);
            newPage = freeList.AllocatePage();  // freeList->4
            Debug.Assert(newPage.PageNumber == 3);

            // recycle a page
            freeList.RecyclePage(pageList[3]);  // freeList->5->4

            // fetch remaining pages
            newPage = freeList.AllocatePage();  // freeList->4
            Debug.Assert(newPage.PageNumber == 5);
            newPage = freeList.AllocatePage();  // freeList->null
            Debug.Assert(newPage.PageNumber == 4);
            newPage = freeList.AllocatePage();  // freeList->null
            Debug.Assert(newPage == null);

            pager.Close();
        }

        private static Expression GetAndsExpression()
        {
            // __tree structure__
            //             and 1
            //        and 2,     > 3
            //     <= 4, <= 5, 6.6 6, c 7
            // 15 8, a, 9, b 10, "str" 11

            // 32
            Expression node8 = new Expression();
            node8.Operator = Operator.AtomConcreteValue;
            node8.ConcreteValue = new AtomValue();
            node8.ConcreteValue.Type = AttributeTypes.Int;
            node8.ConcreteValue.IntegerValue = 15;
            // a
            Expression node9 = new Expression();
            node9.Operator = Operator.AtomVariable;
            node9.AttributeName = "a";
            // b
            Expression node10 = new Expression();
            node10.Operator = Operator.AtomVariable;
            node10.AttributeName = "b";
            // "str"
            Expression node11 = new Expression();
            node11.Operator = Operator.AtomConcreteValue;
            node11.ConcreteValue = new AtomValue();
            node11.ConcreteValue.Type = AttributeTypes.Char;
            node11.ConcreteValue.CharLimit = 5;
            node11.ConcreteValue.StringValue = "str";
            // <=
            Expression node4 = new Expression();
            node4.Operator = Operator.LessThanOrEqualTo;
            node4.LeftOperand = node8;
            node4.RightOperand = node9;
            // <=
            Expression node5 = new Expression();
            node5.Operator = Operator.LessThanOrEqualTo;
            node5.LeftOperand = node10;
            node5.RightOperand = node11;
            // 6.6
            Expression node6 = new Expression();
            node6.Operator = Operator.AtomConcreteValue;
            node6.ConcreteValue = new AtomValue();
            node6.ConcreteValue.Type = AttributeTypes.Float;
            node6.ConcreteValue.FloatValue = 6.6;
            // c
            Expression node7 = new Expression();
            node7.Operator = Operator.AtomVariable;
            node7.AttributeName = "c";
            // and
            Expression node2 = new Expression();
            node2.Operator = Operator.And;
            node2.LeftOperand = node4;
            node2.RightOperand = node5;
            // >
            Expression node3 = new Expression();
            node3.Operator = Operator.MoreThan;
            node3.LeftOperand = node6;
            node3.RightOperand = node7;
            // and
            Expression node1 = new Expression();
            node1.Operator = Operator.And;
            node1.LeftOperand = node2;
            node1.RightOperand = node3;

            return node1;
        }

        static void TestPagerSwapping()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 4);

            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            pager.ExtendNumberOfPages();
            // pager.ExtendNumberOfPages();

            MemoryPage page1 = pager.ReadPage(1);
            page1.IsPinned = true;
            MemoryPage page2 = pager.ReadPage(2);
            page2.Data[0] = 2;
            MemoryPage page3 = pager.ReadPage(3);
            MemoryPage page4 = pager.ReadPage(4);
            MemoryPage page5 = pager.ReadPage(5);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == false);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == false);
            MemoryPage page6 = pager.ReadPage(6);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == true);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == false);
            Debug.Assert(page6.IsSwappedOut == false);
            page4.Data[0] = 4;
            page4[1] = 44;
            MemoryPage page7 = pager.ReadPage(7);
            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.IsSwappedOut == true);
            Debug.Assert(page3.IsSwappedOut == true);
            Debug.Assert(page4.IsSwappedOut == false);
            Debug.Assert(page5.IsSwappedOut == true);
            Debug.Assert(page6.IsSwappedOut == false);
            Debug.Assert(page7.IsSwappedOut == false);

            // not enough pages
            bool error = false;
            try
            {
                MemoryPage page8 = pager.ReadPage(8);
            }
            catch (Exception)
            {
                error = true;
            }
            Debug.Assert(error == true);

            Debug.Assert(page1.IsSwappedOut == false);
            Debug.Assert(page2.Data[0] == 2);
            Debug.Assert(page4.Data[0] == 4);
            Debug.Assert(page4[1] == 44);

            pager.Close();
        }

        static void TestPager()
        {
            string dbPath = "./testdbfile.minidb";
            File.Delete(dbPath);

            Pager pager = new Pager(dbPath, 4096, 4);

            Debug.Assert(pager.PageCount == 1);

            pager.ExtendNumberOfPages();
            Debug.Assert(pager.PageCount == 2);

            MemoryPage page = pager.ReadPage(1);
            page.Data[2] = 114;
            page.Data[3] = 5;
            page.Data[4] = 14;

            pager.WritePage(page);
            MemoryPage page1 = pager.ReadPage(1);

            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            // it will save all dirty pages and remove all pages from recording
            // after that, any MemoryPage returned from this `pager` will no longer legal to use
            pager.Close();

            pager.Open(dbPath);
            // another page also with page number 1
            page1 = pager.ReadPage(1);

            // test if preventing buffer duplication
            Debug.Assert(page.Data[2] == page1.Data[2]);
            Debug.Assert(page.Data[3] == page1.Data[3]);
            Debug.Assert(page.Data[4] == page1.Data[4]);

            // test if the two page pointing to the same buffer
            Debug.Assert(page1.Data[100] == 0);
            page.Data[100] = 1;
            Debug.Assert(page1.Data[100] == 1);

            pager.Close();
        }


        static void AssertAtomValue(AtomValue v1, AtomValue v2)
        {
            Debug.Assert(v1.Type == v2.Type);
            Debug.Assert(v1.CharLimit == v2.CharLimit);
            Debug.Assert(v1.FloatValue == v2.FloatValue);
            Debug.Assert(v1.IntegerValue == v2.IntegerValue);
            Debug.Assert(v1.StringValue == v2.StringValue);
        }
    }
}
