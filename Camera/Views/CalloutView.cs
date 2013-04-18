using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
using MonoTouch.ObjCRuntime;

namespace Camera.Views
{
    class CalloutView : UIView
    {
        public event EventHandler<EventArgs> CalloutButtonTap;

        protected virtual void OnCalloutButtonTap()
        {
            EventHandler<EventArgs> handler = CalloutButtonTap;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        const float CenterImageWidth = 32;
        const float CalloutHeight = 59;
        const float MinLeftImageWidth = 16;
        const float MinRightImageWidth = 16;
        static float _anchorX = 100;
        const float AnchorY = 60;
        const float CenterImageAnchorOffsetX = 15;
        const float MinAnchorX = MinLeftImageWidth + CenterImageAnchorOffsetX;
        const float ButtonWidth = 29;
        const float ButtonY = 5;
        const float LabelHeight = 48;

        static UIImage _calloutLeftImage;
        static UIImage _calloutCenterImage;
        static UIImage _calloutRightImage;

        static readonly RectangleF Initframe = new RectangleF(0, 0, 100, CalloutHeight);

        public UIImageView CalloutLeft;
        public UIImageView CalloutCenter;
        public UIImageView CalloutRight;
        public UIButton CalloutButton;
        public UILabel CalloutLabel;

        // The IntPtr and NSCoder constructors are required for controllers that need 
        // to be able to be created from a xib rather than from managed code

        public CalloutView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        [Export("initWithCoder:")]
        public CalloutView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        [Export("initWithFrame:")]
        public CalloutView(RectangleF frame)
            : base(frame)
        {
            Initialize();
        }

        public CalloutView(PointF pt)
            : base(Initframe)
        {
            SetAnchorPoint(pt);
            Initialize();
        }


        public CalloutView(string text, PointF pt)
            : base(Initframe)
        {
            SetAnchorPoint(pt);
            Initialize();
            Text = text;
        }

        public CalloutView(NSObjectFlag t)
            : base(t)
        {
        }

        public CalloutView(string text, PointF pt, NSObject target, Selector sel)
            : base(Initframe)
        {
            SetAnchorPoint(pt);
            Initialize();
            Text = text;
            AddButtonTarget(target, sel);
        }

        public static CalloutView AddCalloutView(UIView parent, string text, PointF pt, NSObject target, Selector sel)
        {
            var callout = new CalloutView(text, pt, target, sel);
            callout.ShowWithAnimation(parent);
            return callout;
        }

        public void ShowWithAnimation(UIView parent)
        {
            Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
            BeginAnimations(null);
            SetAnimationDelegate(this);
            SetAnimationWillStartSelector(new Selector("animationWillStart:context:"));
            SetAnimationDidStopSelector(new Selector("animationDidStop:finished:context:"));
            SetAnimationDuration(0.1f);
            parent.AddSubview(this);
            Transform = CGAffineTransform.MakeScale(1.2f, 1.2f);
            CommitAnimations();
        }

        [Export("animationWillStart:context:")]
        public void AnimationStarted(CAAnimation anim, IntPtr context)
        {
        }

        [Export("animationDidStop:finished:context:")]
        public void AnimationStopped(CAAnimation anim, bool finished, IntPtr context)
        {
            Transform = CGAffineTransform.MakeIdentity();
        }

        public string Text
        {
            get { return CalloutLabel.Text; }
            set
            {
                CalloutLabel.Text = value;
                SetNeedsLayout();
            }
        }

        void Initialize()
        {
            UserInteractionEnabled = true;
            BackgroundColor = UIColor.Clear;
            Opaque = false;

            if (_calloutLeftImage == null)
            {
                _calloutLeftImage = UIImage.FromFile("callout/callout-left.png").StretchableImage(15, 0);
            }

            if (_calloutCenterImage == null)
            {
                _calloutCenterImage = UIImage.FromFile("callout/callout-centre.png");
            }

            if (_calloutRightImage == null)
            {
                _calloutRightImage = UIImage.FromFile("callout/callout-right.png").StretchableImage(1, 0);
            }

            RectangleF frame = Frame;
            frame.Height = CalloutHeight;
            if (frame.Width < 100)
                frame.Width = 100;
            Console.WriteLine(frame.Width);
            Frame = frame;
            _anchorX = frame.Width/2;
            if (_anchorX < MinAnchorX)
                _anchorX = MinAnchorX;

            float leftWidth = _anchorX - CenterImageAnchorOffsetX;
            Console.WriteLine(leftWidth);
            float rightWidth = frame.Width - leftWidth - CenterImageWidth;

            CalloutLeft = new UIImageView(new RectangleF(0, 0, leftWidth, CalloutHeight)) {Image = _calloutLeftImage};
            AddSubview(CalloutLeft);

            CalloutCenter = new UIImageView(new RectangleF(leftWidth, 0, CenterImageWidth, CalloutHeight)) {
                Image = _calloutCenterImage
            };
            AddSubview(CalloutCenter);

            CalloutRight = new UIImageView(new RectangleF(leftWidth + CenterImageWidth, 0, rightWidth, CalloutHeight)) {
                Image = _calloutRightImage
            };
            AddSubview(CalloutRight);

            CalloutLabel = new UILabel(new RectangleF(MinLeftImageWidth, -2, frame.Width - MinLeftImageWidth - ButtonWidth - MinRightImageWidth - 2, LabelHeight)) {
                Font =UIFont.FromName("ProximaNova-Bold",20),
                TextColor = UIColor.FromRGB(12,128,129),
                BackgroundColor = UIColor.Clear
            };
            AddSubview(CalloutLabel);
            CalloutButton = UIButton.FromType(UIButtonType.DetailDisclosure);
            //CalloutButton = new UIButton(new RectangleF(frame.Width - ButtonWidth - MinRightImageWidth + 4,ButtonY,ButtonWidth,LabelHeight));
            //CalloutButton.SetImage(UIImage.FromFile("callout/callout-button.png"), UIControlState.Normal);
            //CalloutButton.ContentMode = UIViewContentMode.Center;
            RectangleF f = CalloutButton.Frame;
            f.X = frame.Width - ButtonWidth - MinRightImageWidth + 4;
            f.Y = ButtonY;
            CalloutButton.Frame = f;
            CalloutButton.AdjustsImageWhenHighlighted = false;
            //CalloutButton.TouchUpInside += CalloutButtonOnTouchUpInside;
            AddSubview(CalloutButton);

        }

        void CalloutButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            OnCalloutButtonTap();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            SizeF size = StringSize(CalloutLabel.Text, CalloutLabel.Font);
            RectangleF frame = Frame;
            frame.Width = size.Width + MinLeftImageWidth + ButtonWidth + MinRightImageWidth + 3;
            frame.Height = CalloutHeight;
            _anchorX = frame.Width / 2;
            if (_anchorX < MinAnchorX)
                _anchorX = MinAnchorX;
            Console.WriteLine(_anchorX);
            Console.WriteLine(frame.X);
            frame.X = -_anchorX;
            Frame = frame;

         

            float leftWidth = _anchorX - CenterImageAnchorOffsetX;
            float rightWidth = frame.Width - leftWidth - CenterImageWidth;

            CalloutLeft.Frame = new RectangleF(0, 0, leftWidth, CalloutHeight);
            CalloutCenter.Frame = new RectangleF(leftWidth, 0, CenterImageWidth, CalloutHeight);
            CalloutRight.Frame = new RectangleF(leftWidth + CenterImageWidth, 0, rightWidth, CalloutHeight);

            CalloutLabel.Frame = new RectangleF(MinLeftImageWidth, 0, size.Width, LabelHeight);

            RectangleF f = CalloutButton.Frame;
            f.X = frame.Width - ButtonWidth - MinRightImageWidth + 4;
            f.Y = ButtonY;
            CalloutButton.Frame = f;

        }

        public void SetAnchorPoint(PointF pt)
        {
            RectangleF frame = Frame;
            frame.X = pt.X - _anchorX;
            frame.Y = pt.Y - AnchorY;
            Frame = frame;
        }

        public void AddButtonTarget(NSObject target, Selector sel)
        {
            CalloutButton.AddTarget(target, sel, UIControlEvent.TouchUpInside);
        }


    }
}
