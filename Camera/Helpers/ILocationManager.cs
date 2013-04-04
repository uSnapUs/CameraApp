using System;
using System.Collections.Generic;

namespace Camera.Helpers
{
    public interface ILocationManager
    {
        void SubscribeToLocationUpdates(EventHandler<LocationEventArgs> onLocationUpdated);
        void UnsubscribeFromLocationUpdates(EventHandler<LocationEventArgs> onLocationUpdated);
        void LookupAddress(string searchText, Action<IEnumerable<AddressDetails>> onAddressLookup);
    }

    public struct AddressDetails
    {
        public string Description { get; set; }
        public Coordinate Coordinate { get; set; }
    }

    public class LocationEventArgs : EventArgs
    {
        public Coordinate Coordinate { get; set; }
    }
}