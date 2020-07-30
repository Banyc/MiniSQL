using MiniSQL.BufferManager.Models;
using MiniSQL.IndexManager.Models;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
        public void RemoveTree(BTreeNode root)
        {
            if (root.PageType == PageTypes.LeafIndexPage || root.PageType == PageTypes.LeafTablePage)
            {
                DeleteNode(root);
                return;
            }
            if (root.PageType == PageTypes.InternalIndexPage)
            {
                foreach (BTreeCell cell in root)
                {
                    MemoryPage page = _pager.ReadPage((int)((InternalIndexCell)cell).ChildPage);
                    BTreeNode node = new BTreeNode(page);
                    RemoveTree(node);
                }
            }
            else  // (root.PageType == PageTypes.InternalTablePage)
            {
                foreach (BTreeCell cell in root)
                {
                    MemoryPage page = _pager.ReadPage((int)((InternalTableCell)cell).ChildPage);
                    BTreeNode node = new BTreeNode(page);
                    RemoveTree(node);
                }
            }
            MemoryPage rightPage = _pager.ReadPage((int)root.RightPage);
            BTreeNode rightNode = new BTreeNode(rightPage);
            RemoveTree(rightNode);
            // post-order traversal
            DeleteNode(root);
        }
    }
}
