using System;
using Newtonsoft.Json;

namespace Camera.Model
{
    public class Photo
    {
        [JsonIgnore]
        [AutoIncrement,PrimaryKey]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public int Likes { get; set; }


        [JsonProperty(PropertyName = "_id")]
        public string ServerId { get; set; }

        [JsonProperty(PropertyName = "thumbnail_url")]
        public string ThumbnailPath { get; set; }
        [JsonProperty(PropertyName = "full_url")]
        public string FullPath { get; set; }
        [JsonProperty(PropertyName = "root_url")]
        public string RootUrl { get; set; }

        [JsonProperty(PropertyName = "creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonIgnore] 
        public string EventCode { get; set; }
    }
}