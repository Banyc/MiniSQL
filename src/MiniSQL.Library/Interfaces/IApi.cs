using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public interface IApi
    {
        // return empty list (not null) if nothing to return
        List<SelectResult> Query(string sql);
    }
}
