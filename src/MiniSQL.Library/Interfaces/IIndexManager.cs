using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    // TODO: review
    public interface IIndexManager
    {
        int Query(CreateStatement createStatement);
        void Drop(int rootPage);
        int Query(DeleteStatement deleteStatement, int rootPage);

    }
}
