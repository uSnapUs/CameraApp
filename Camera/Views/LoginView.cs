using System;
using System.Drawing;
using Camera.Model;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public sealed class LoginView:UIView
    {
        readonly string _loginReason;

        UIImageView _logo;
        float _yOffset;
        UILabel _loginInstructionLabel;
        UIButton _facebookButton;
        public event EventHandler<EventArgs> FacebookLoginPressed;

        void OnFacebookLoginPressed(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = FacebookLoginPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        public LoginView(string loginReason):base(UIScreen.MainScreen.Bounds)
        {
            _loginReason = loginReason;

            _yOffset = UIScreen.MainScreen.Bounds.Height + (UIScreen.MainScreen.Bounds.Height / 2);
            Center = new PointF(UIScreen.MainScreen.Bounds.Width/2, _yOffset);
            //Layer.CornerRadius = 5f;
            BackgroundColor = UIColor.FromRGBA(0,0,0,0.9f);
            InitComponents();
            WireEvents();
        }

        void WireEvents()
        {
            _facebookButton.TouchUpInside += OnFacebookLoginPressed;
        }

        

        void InitComponents()
        {
            _loginInstructionLabel = new DropShadowLabel {
                Text = String.Format("Before {0}\nquickly login below!", _loginReason),
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.White,
                ShadowColor = UIColor.FromRGBA(0,0,0,40),
                Lines = 0,
                Font = UIFont.FromName("ProximaNova-Bold",15),
               TextAlignment = UITextAlignment.Center
                
            };
            _logo = new UIImageView(UIImage.FromFile("logo.png"));
            _facebookButton = new UIButton {

            };
            _facebookButton.SetImage(UIImage.FromFile("Button_Facebook.png"),UIControlState.Normal);
            AddSubview(_loginInstructionLabel);
            AddSubview(_logo);
            AddSubview(_facebookButton);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = Bounds.Width;
            
            _logo.Frame = new RectangleF((width-271)/2,81,271,122);
            _loginInstructionLabel.Frame=new RectangleF((width/2)-(230/2),81+122,230,50);
            _facebookButton.Frame = new RectangleF((width/2)-((577/2)/2),81+122+100,577/2,126/2);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnwireEvents();
        }
        
        void UnwireEvents()
        {
            _facebookButton.TouchUpInside -= OnFacebookLoginPressed;
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