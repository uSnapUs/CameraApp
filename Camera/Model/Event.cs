using System;
using Camera.Helpers;

namespace Camera.Model
{
    public class Event
    {
        public override string ToString()
        {
            return string.Format("Name: {0}, Address: {1}, Location: {2}, IsPublic: {3}, StartDate: {4}, EndDate: {5}", Name, Address, Location, IsPublic, StartDate, EndDate);
        }

        protected bool Equals(Event other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Address, other.Address) && Location.Equals(other.Location) && IsPublic.Equals(other.IsPublic) && StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Event) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Location.GetHashCode();
                hashCode = (hashCode*397) ^ IsPublic.GetHashCode();
                hashCode = (hashCode*397) ^ StartDate.GetHashCode();
                hashCode = (hashCode*397) ^ EndDate.GetHashCode();
                return hashCode;
            }
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public Coordinate Location { get; set; }

        public bool IsPublic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}