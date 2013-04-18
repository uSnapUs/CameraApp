using System;
using Newtonsoft.Json;

namespace Camera.Model
{
    public class Event
    {
        public override string ToString()
        {
            return string.Format("Name: {0}, Address: {1}, IsPublic: {2}, StartDate: {3}, EndDate: {4}, Code: {5}, Location: {6}", Name, Address, IsPublic, StartDate, EndDate, Code, Location);
        }

        protected bool Equals(Event other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Address, other.Address) && IsPublic.Equals(other.IsPublic) && StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate) && string.Equals(Code, other.Code) && Equals(Location, other.Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Event) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsPublic.GetHashCode();
                hashCode = (hashCode*397) ^ StartDate.GetHashCode();
                hashCode = (hashCode*397) ^ EndDate.GetHashCode();
                hashCode = (hashCode*397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Location != null ? Location.GetHashCode() : 0);
                return hashCode;
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "is_public")]
        public bool IsPublic { get; set; }
        [JsonProperty(PropertyName = "start_date")]
        public DateTime StartDate { get; set; }
        [JsonProperty(PropertyName = "end_date")]
        public DateTime EndDate { get; set; }
        [JsonProperty(PropertyName="code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }
        
    }
    public class Point
    {
        public override string ToString()
        {
            return string.Format("Type: {0}, Longitude: {1}, Latitude: {2}", Type, Longitude, Latitude);
        }

        protected bool Equals(Point other)
        {
            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Longitude.GetHashCode()*397) ^ Latitude.GetHashCode();
            }
        }

        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get { return "Point"; }
        }

        [JsonProperty(PropertyName = "coordinates")]
        public double[] Coordinates
        {
            get { return new[] {Longitude, Latitude}; }
            set
            {
                Longitude = value[0];
                Latitude = value[1];
            }
        }
        [JsonIgnore]
        public double Longitude { get; set; }
        [JsonIgnore]
        public double Latitude { get; set; }
    }
}