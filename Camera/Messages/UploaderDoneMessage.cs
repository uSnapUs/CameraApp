using System;
using Camera.Model;
using TinyMessenger;

namespace Camera.Messages
{
    public class UploaderDoneMessage : ITinyMessage
    {
        public object Sender { get; set; }

        public Guid PhotoId { get; set; }

        public string Response { get; set; }
    }
}