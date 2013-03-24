using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public class DropShadowLabel:UILabel
    {
        public override void DrawText(System.Drawing.RectangleF rect)
        {
            var myShadowOffset = new SizeF(0, 2);
            var myColorValues = new[] {0f, 0f, 0f, .4f};
            var myContext = UIGraphics.GetCurrentContext();
            myContext.SaveState();

            var myColorSpace = CGColorSpace.CreateDeviceRGB();
            var myColor = new CGColor(myColorSpace, myColorValues);
            myContext.SetShadowWithColor(myShadowOffset,2,myColor);
            base.DrawText(rect);
            myColor.Dispose();
            myColorSpace.Dispose();
            myContext.RestoreState();
        }
    }
}