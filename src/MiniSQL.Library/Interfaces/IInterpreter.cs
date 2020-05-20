using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public interface IInterpreter
    {
        Query GetQuery(string input);
    }
}
