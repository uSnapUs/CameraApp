using System.Drawing;
using Camera.Model;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SDWebImage;

namespace Camera.Views
{
    public sealed class EventPhotoCell:UITableViewCell
    {
        
        PhotoCellView _photoView;

        public EventPhotoCell(string reuseIdentifier, Photo photo)
            : base(UITableViewCellStyle.Default, reuseIdentifier)
        {
            _photoView = new PhotoCellView(photo);
            ContentView.Add(_photoView);


        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _photoView.Frame = ContentView.Bounds;
            _photoView.SetNeedsDisplay();

        }
        public void UpdatePhoto(Photo photo)
        {
            _photoView.Update(photo);
        }
    }

    internal class PhotoCellView:UIView
    {
        UIImageView _imageView;

        public PhotoCellView(Photo photo)
        {
            InitViews();
            Update(photo);
        }

        void InitViews()
        {
            _imageView = new UIImageView();
            AddSubview(_imageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var bounds = this.Bounds;
            _imageView.Frame = new RectangleF((bounds.Width/2)-100,15,200,200);
        }
        public void Update(Photo photo)
        {
            _imageView.SetImage(url: new NSUrl(photo.RootUrl + photo.ThumbnailPath));
            SetNeedsDisplay();
        }
    }
}