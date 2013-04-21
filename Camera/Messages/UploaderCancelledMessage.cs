using TinyMessenger;

namespace Camera.Messages
{
    public class UploaderCancelledMessage:ITinyMessage
    {
        public object Sender { get; set; }
    }
}