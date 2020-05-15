using System;

namespace wpay.Library.Frameworks.PayQueue
{
    public class PayQueueException: Exception
    {
        public PayQueueException() {}

        public PayQueueException(string message) : base(message) {}

        public PayQueueException(string message, Exception inner) : base(message, inner) {}
    }
}