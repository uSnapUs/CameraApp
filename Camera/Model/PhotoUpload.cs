using System;

namespace Camera.Model
{
    public class PhotoUpload
    {

        public PhotoUpload()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Event Event { get; set; }

        public string Path { get; set; }

       
    }
}