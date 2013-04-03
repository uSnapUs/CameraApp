using System;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;

namespace Camera.Helpers
{
    public class LocationDelegate : CLLocationManagerDelegate, ILocationManager
    {
        CLLocationManager _manager;

        NSTimer _updateLocationTimer;


        void Start()
        {
            _manager = new CLLocationManager();

            _manager.StartUpdatingLocation();
#pragma warning disable 612,618
            _manager.UpdatedLocation += LocationUpdate_Old;
#pragma warning restore 612,618
            _manager.LocationsUpdated += LocationsUpdated;
            _manager.Failed += Failed;
        }

        void LocationUpdate_Old(object sender, CLLocationUpdatedEventArgs e)
        {
            LocationsUpdated(sender,new CLLocationsUpdatedEventArgs(new[]{e.NewLocation}));
        }


        void Stop()
        {
            if (_updateLocationTimer != null)
            {
                _updateLocationTimer.Invalidate();
                _updateLocationTimer = null;
            }
            _onLocationUpdate = null;

            if (_manager != null)
            {
                _manager.StopUpdatingLocation();
                _manager = null;
            }

        }

        void Failed(object sender, NSErrorEventArgs failArgs)
        {
            Console.WriteLine(failArgs.Error);
        }

        void LocationsUpdated(object sender, CLLocationsUpdatedEventArgs updatedArgs)
        {

            if (_onLocationUpdate != null)
            {
                _onLocationUpdate.Invoke(this, new LocationEventArgs
                {
                    Coordinate = new Coordinate
                    {
                        Latitude = updatedArgs.Locations[0].Coordinate.Latitude,
                        Longitude = updatedArgs.Locations[0].Coordinate.Longitude,
                        Accuracy = updatedArgs.Locations[0].HorizontalAccuracy
                    }
                });
            }
        }
        EventHandler<LocationEventArgs> _onLocationUpdate;
        public event EventHandler<LocationEventArgs> LocationUpdate
        {
            add
            {
                if (_onLocationUpdate == null)
                {
                    Start();
                }
                _onLocationUpdate += value;

            }
            remove
            {
                if (_onLocationUpdate != null)
                {
                    // ReSharper disable DelegateSubtraction
                    _onLocationUpdate -= value;
                    // ReSharper restore DelegateSubtraction
                }
                if (_onLocationUpdate == null)
                {
                    Stop();
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);

        }

        public void SubscribeToLocationUpdates(EventHandler<LocationEventArgs> onLocationUpdated)
        {
            LocationUpdate += onLocationUpdated;
        }

        public void UnsubscribeFromLocationUpdates(EventHandler<LocationEventArgs> onLocationUpdated)
        {
            LocationUpdate -= onLocationUpdated;
        }
    }
}