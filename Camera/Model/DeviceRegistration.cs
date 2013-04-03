
using Newtonsoft.Json;

namespace Camera.Model
{
    public class DeviceRegistration
    {
      

        protected bool Equals(DeviceRegistration other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Email, other.Email) && string.Equals(Guid, other.Guid) && string.Equals(ServerId, other.ServerId) && string.Equals(FacebookId, other.FacebookId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeviceRegistration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Guid != null ? Guid.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ServerId != null ? ServerId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FacebookId != null ? FacebookId.GetHashCode() : 0);
                return hashCode;
            }
        }

        [PrimaryKey, AutoIncrement]
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string ServerId { get; set; }
        [JsonProperty(PropertyName = "facebook_id")]
        public string FacebookId { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        

       
    }

   

   
}