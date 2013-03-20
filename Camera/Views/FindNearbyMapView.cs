using System;
using Camera.Views.Interfaces;
using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public class FindNearbyMapView : MKMapView, IFindNearbyMapView
    {
        UIView _topBarView;

        UIImageView _logoImageView;

        UIButton _backButton;
        public event EventHandler<EventArgs> BackButtonPressed;

        public FindNearbyMapView()
            : base()
        {

            InitViews();
            WireEvents();
        }

        public override void LayoutSubviews()
        {

            base.LayoutSubviews();
            //var height = this.Bounds.Height;
            var width = this.Bounds.Width;
            _topBarView.Frame = new System.Drawing.RectangleF(0, 0, width, 51);
            _logoImageView.Frame = new System.Drawing.RectangleF((width/2) - (94/2), 3, 94, 44);
            _backButton.Frame = new System.Drawing.RectangleF(15, 15, 25, 22);
        }

        void InitViews()
        {
            _topBarView = new UIView {
                BackgroundColor = UIColor.FromRGB(17, 186, 188)

            };
            _logoImageView = new UIImageView {
                Image = UIImage.FromFile(@"logo_small.png")
            };
            _backButton = new UIButton {

            };
            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Topbar_Back.png"), UIControlState.Normal);
            _topBarView.Layer.ShadowOffset = new System.Drawing.SizeF(0, 2);
            _topBarView.Layer.ShadowRadius = 5;
            _topBarView.Layer.ShadowOpacity = 0.5f;
            //_topBarView.Layer.CornerRadius = 5;
            this.AddSubview(_topBarView);

            _topBarView.AddSubview(_logoImageView);
            _topBarView.AddSubview(_backButton);
        }

        void OnBackButtonPress(object sender, EventArgs e)
        {
            if (this.BackButtonPressed != null)
            {
                this.BackButtonPressed.Invoke(this, e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.UnwireEvents();
            _topBarView = null;
            _logoImageView = null;
            _backButton = null;

        }

        void WireEvents()
        {

            _backButton.TouchUpInside += OnBackButtonPress;
        }

        void UnwireEvents()
        {

            _backButton.TouchUpInside -= OnBackButtonPress;
        }
    }

}