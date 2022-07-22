using System;
using System.Runtime.Serialization;

namespace NHibernateCore
{
    [Serializable]
    public class OptimisticVersionException : Exception
    {
        public OptimisticVersionException()
        {
        }

        public OptimisticVersionException(string message)
            : base(message)
        {
        }

        public OptimisticVersionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected OptimisticVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
