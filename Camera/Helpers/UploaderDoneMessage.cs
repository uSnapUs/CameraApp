using TinyMessenger;

namespace Camera.Helpers
{
    internal class UploaderDoneMessage : ITinyMessage
    {
        public Server.PhotoUploader PhotoUploader { get; set; }
        public object Sender { get; set; }
    }
}