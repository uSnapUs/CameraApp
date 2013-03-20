using MonoTouch.UIKit;

namespace Camera.Views
{
    sealed class FindNearbyView : UIView
    {
        FindNearbyMapView _mapView;

        public FindNearbyView(FindNearbyMapView view)
        {
            Layer.CornerRadius = 5f;
            InitViews(view);
        }

        void InitViews(FindNearbyMapView view)
        {
            _mapView = view;
            this.AddSubview(_mapView);
        }
        public override void LayoutSubviews()
        {
            var height = this.Bounds.Height;
            var width = this.Bounds.Width;
            _mapView.Frame = new System.Drawing.RectangleF(0, 0, width, height);
        }
        protected override void Dispose(bool disposing)
        {
            _mapView = null;
            base.Dispose(disposing);
        }
    }
}