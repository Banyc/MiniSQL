using System;
using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public interface IDatabaseController
    {
        // return empty list (not null) if nothing to return
        List<SelectResult> Query(string sql);
    }
}
