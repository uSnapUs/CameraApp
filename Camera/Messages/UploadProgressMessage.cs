using System;
using TinyMessenger;

namespace Camera.Messages
{
    public class UploadProgressMessage:ITinyMessage
    {
        public object Sender { get;  set; }

        public long BytsSent { get; set; }

        public Guid PhotoId { get; set; }

        public long TotalBytes { get; set; }
    }
}