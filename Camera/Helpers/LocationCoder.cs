using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBookUI;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;

namespace Camera.Helpers
{
    public class LocationCoder:ILocationCoder
    {
        readonly CLGeocoder _geoCoder;
        NSTimer _currentTimer;

        public LocationCoder()
        {
            _geoCoder = new CLGeocoder();
        }

        public void LookupLocation(string address,Action<ILocation[]> onComplete)
        {
            if (_geoCoder.Geocoding)
            {
                _geoCoder.CancelGeocode();
            }
            if (_currentTimer != null)
            {
                _currentTimer.Invalidate();
                _currentTimer = null;
            }
            if (!string.IsNullOrEmpty(address))
            {
                _currentTimer = NSTimer.CreateScheduledTimer(2, () =>
                    {
                        _geoCoder.GeocodeAddress(address,
                                                 (placemarks, error) =>
                                                 onComplete.Invoke(GotLocations(placemarks, error)));
                        _currentTimer = null;
                    });
            }

        }

        ILocation[] GotLocations(IEnumerable<CLPlacemark> placemarks, NSError error)
        {
            if (error!=null)
            {
                Console.WriteLine(error.LocalizedDescription);
            }
            if(placemarks==null)
                placemarks = new List<CLPlacemark>();
            return (from clPlacemark in placemarks
                    let address = ABAddressFormatting.ToString(clPlacemark.AddressDictionary, false)
                    select new Location {
                        Address = address.Replace(Environment.NewLine, ", "), Coordinate = new Coordinate {Latitude = clPlacemark.Location.Coordinate.Latitude, Longitude = clPlacemark.Location.Coordinate.Longitude}
                    }).Cast<ILocation>().ToArray();
        }
    }

    internal class Location : ILocation
    {
        public string Address { get; set; }

        public Coordinate Coordinate { get; set; }
    }


    public struct Coordinate
    {
        

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Accuracy { get; set; }
    }
}