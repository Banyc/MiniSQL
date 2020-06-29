using System;

namespace MiniSQL.Library.Exceptions
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

    public class RepeatedKeyException : Exception
    {
        public RepeatedKeyException() { }
        public RepeatedKeyException(string message) : base(message) { }
    }
    
    public class TableOrIndexAlreadyExistsException : Exception
    {
        public TableOrIndexAlreadyExistsException()
        { }
        public TableOrIndexAlreadyExistsException(string message)
            : base(message) { }
        public TableOrIndexAlreadyExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class TableOrIndexNotExistsException : Exception
    {
        public TableOrIndexNotExistsException()
        { }
        public TableOrIndexNotExistsException(string message)
            : base(message) { }
        public TableOrIndexNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class AttributeNotExistsException : Exception
    {
        public AttributeNotExistsException()
        { }
        public AttributeNotExistsException(string message)
            : base(message) { }
        public AttributeNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class NumberOfAttributesNotMatchsException : Exception
    {
        public NumberOfAttributesNotMatchsException()
        { }
        public NumberOfAttributesNotMatchsException(string message)
            : base(message) { }
        public NumberOfAttributesNotMatchsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class TypeOfAttributeNotMatchsException : Exception
    {
        public TypeOfAttributeNotMatchsException()
        { }
        public TypeOfAttributeNotMatchsException(string message)
            : base(message) { }
        public TypeOfAttributeNotMatchsException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class KeyNotExistsException : Exception
    {
        public KeyNotExistsException()
        { }
        public KeyNotExistsException(string message)
            : base(message) { }
        public KeyNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }

}
