using MiniSQL.IndexManager.Models;

namespace MiniSQL.IndexManager.Controllers
{
    public partial class BTreeController
    {
        public BTreeNode OccupyNewTableNode()
        {
            BTreeNode newNode = GetNewNode(PageTypes.LeafTablePage);
            return newNode;
        }
    }
}
