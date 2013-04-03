using System;
using System.Drawing;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace Camera.Helpers
{
    public static class MapHelper
    {
        public static void SetCenterCoordinate(this MKMapView mapToCenter, CLLocationCoordinate2D centerCoordinate, int zoomLevel, bool animated)
        {
            // clamp large numbers to 28
            zoomLevel = Math.Min(zoomLevel, 28);

            // use the zoom level to compute the region
            MKCoordinateSpan span = CoordinateSpanWithMapView(mapToCenter, centerCoordinate, zoomLevel);
            var region = new MKCoordinateRegion(centerCoordinate, span);

            // set the region like normal
            mapToCenter.SetRegion(region, animated);
        }

        const double MercatorOffset = 268435456;
        const double MercatorRadius = 85445659.44705395;

        static double LongitudeToPixelSpaceX(double longitude)
        {
            return Math.Round(MercatorOffset + MercatorRadius * longitude * Math.PI / 180.0);
        }

        static double LatitudeToPixelSpaceY(double latitude)
        {
            return Math.Round(MercatorOffset - MercatorRadius * Math.Log((1 + Math.Sin(latitude * Math.PI / 180.0)) / (1 - Math.Sin(latitude * Math.PI / 180.0))) / 2.0);
        }

        static double PixelSpaceXToLongitude(double pixelX)
        {
            return ((Math.Round(pixelX) - MercatorOffset) / MercatorRadius) * 180.0 / Math.PI;
        }

        static double PixelSpaceYToLatitude(double pixelY)
        {
            return (Math.PI / 2.0 - 2.0 * Math.Tan(Math.Exp((Math.Round(pixelY) - MercatorOffset) / MercatorRadius))) * 180.0 / Math.PI;
        }

        static MKCoordinateSpan CoordinateSpanWithMapView(MKMapView mapView, CLLocationCoordinate2D centerCoordinate, int zoomLevel)
        {
            // convert center coordiate to pixel space
            double centerPixelX = LongitudeToPixelSpaceX(centerCoordinate.Longitude);
            double centerPixelY = LatitudeToPixelSpaceY(centerCoordinate.Latitude);

            // determine the scale value from the zoom level
            int zoomExponent = 20 - zoomLevel;
            double zoomScale = Math.Pow(2, zoomExponent);

            // scale the map’s size in pixel space
            SizeF mapSizeInPixels = mapView.Bounds.Size;
            double scaledMapWidth = mapSizeInPixels.Width * zoomScale;
            double scaledMapHeight = mapSizeInPixels.Height;

            // figure out the position of the top-left pixel
            double topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
            double topLeftPixelY = centerPixelY - (scaledMapHeight / 2);

            // find delta between left and right longitudes
            double minLng = PixelSpaceXToLongitude(topLeftPixelX);
            double maxLng = PixelSpaceXToLongitude(topLeftPixelX + scaledMapWidth);
            double longitudeDelta = maxLng - minLng;

            // find delta between top and bottom latitudes
            double minLat = PixelSpaceYToLatitude(topLeftPixelY);
            double maxLat = PixelSpaceYToLatitude(topLeftPixelY + scaledMapHeight);
            double latitudeDelta = -1 * (maxLat - minLat);

            // create and return the lat/lng span
            var span = new MKCoordinateSpan(latitudeDelta, longitudeDelta);

            return span;
        }
    }
}