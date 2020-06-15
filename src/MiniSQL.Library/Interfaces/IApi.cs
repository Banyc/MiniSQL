using System;
using System.Collections.Generic;
using MiniSQL.Library.Models;

namespace MiniSQL.Library.Interfaces
{
    public class StatementPreCheckException : Exception
    {
        public StatementPreCheckException()
        { }
        public StatementPreCheckException(string message)
            : base(message) { }
        public StatementPreCheckException(string message, Exception inner)
            : base(message, inner) { }
    }
    
    public interface IApi
    {
        // return empty list (not null) if nothing to return
        List<SelectResult> Query(string sql);
    }
}
