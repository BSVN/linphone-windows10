using System;

namespace BSN.Resa.Vns.Commons.Exceptions
{
    public class InterserviceCommunicationException : Exception
    {
        public InterserviceCommunicationException() : base() { }

        public InterserviceCommunicationException(string message) : base(message) { }

        public InterserviceCommunicationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
