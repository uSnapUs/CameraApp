using TinyMessenger;

namespace Camera.Messages
{
    public class EventPhotoListUpdatedMessage:ITinyMessage
    {
        public object Sender { get; set; }
        public string EventCode { get; set; }

    }
}