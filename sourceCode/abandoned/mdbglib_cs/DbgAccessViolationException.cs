using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mdbglib
{
    public class DbgAccessViolationException : Exception
    {
        public DbgAccessViolationException() : base() { }
        public DbgAccessViolationException(string message) : base(message) { }
        public DbgAccessViolationException(string message, Exception innerException) : base(message, innerException) { }
        public DbgAccessViolationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
