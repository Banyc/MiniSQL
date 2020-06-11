using MiniSQL.BufferManager.Controllers;
using MiniSQL.BufferManager.Models;

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
    }
}
