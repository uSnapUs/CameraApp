using System;
using TinyMessenger;

namespace Camera.Messages
{
    public class UploaderErrorMessage:ITinyMessage
    {
        public object Sender { get; set; }

        public Exception Error { get; set; }
    }
}