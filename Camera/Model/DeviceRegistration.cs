
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
        public string ServerId { get; set; }
        public string FacebookId { get; set; }

        public DeviceRegistrationDto ToDto()
        {
            return new DeviceRegistrationDto {
                email = Email,
                id = ServerId,
                name = Name,
                facebook_id = FacebookId,
                guid = Guid
            };
        }
    }
    // ReSharper disable InconsistentNaming
    public class DeviceRegistrationDto
    {
        protected bool Equals(DeviceRegistrationDto other)
        {
            return string.Equals(id, other.id) && string.Equals(name, other.name) && string.Equals(email, other.email) && string.Equals(guid, other.guid) && string.Equals(facebook_id, other.facebook_id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeviceRegistrationDto) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (email != null ? email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (guid != null ? guid.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (facebook_id != null ? facebook_id.GetHashCode() : 0);
                return hashCode;
            }
        }

        public string id { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string guid { get; set; }
        public string facebook_id { get; set; }

        internal DeviceRegistration ToModel()
        {
            return new DeviceRegistration() {
                Email = email,
                FacebookId = facebook_id,
                Guid = guid,
                ServerId = id,
                Name = name
            };
        }
    }
    // ReSharper restore InconsistentNaming
}