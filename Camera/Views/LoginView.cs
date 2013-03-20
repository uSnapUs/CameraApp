using System;
using System.Drawing;
using Camera.Model;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public sealed class LoginView:UIView
    {

        UIImageView _logo;
        float _yOffset;

        public LoginView():base(UIScreen.MainScreen.Bounds)
        {
            
            _yOffset = UIScreen.MainScreen.Bounds.Height + (UIScreen.MainScreen.Bounds.Height / 2);
            Center = new PointF(UIScreen.MainScreen.Bounds.Width/2, _yOffset);
            //Layer.CornerRadius = 5f;
            BackgroundColor = UIColor.FromRGBA(0,0,0,0.7f);
            InitComponents();
            WireEvents();
        }

        void WireEvents()
        {
            
        }

        

        void InitComponents()
        {
            _logo = new UIImageView(UIImage.FromFile("logo.png"));
            AddSubview(_logo);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = Bounds.Width;
            
            _logo.Frame = new RectangleF((width-271)/2,81,271,122);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnwireEvents();
        }
        
        void UnwireEvents()
        {
        }

        public void TransitionIn()
        {
            Animate(0.5,() =>
                {
                    _yOffset = (UIScreen.MainScreen.Bounds.Height / 2);
                    Center =new PointF(UIScreen.MainScreen.Bounds.Width/2, _yOffset);
                });
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public User User { get; set; }
    }
}