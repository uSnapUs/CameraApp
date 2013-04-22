using System;
using BigTed;
using Camera.Model;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.AssetsLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SDWebImage;

namespace Camera.ViewControllers
{
    [Register("EventDashboardViewController")]
    public sealed class EventDashboardViewController : BaseViewController,IEventDashboardViewController
    {
        EventDashboardView _eventDashboardView;
        EventDashboardViewControllerSupervisor _supervisor;
        readonly Event _event;
        readonly EventTableViewDelegate _tableViewDataSource;

        public EventDashboardViewController(Event serverEvent)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(false,true);
            _eventDashboardView = new EventDashboardView();
            _eventDashboardView.BackButtonPressed += EventDashboardBackButtonPressed;
            _eventDashboardView.CameraButtonPressed += EventDashboardViewOnCameraButtonPressed;
            _tableViewDataSource = new EventTableViewDelegate();
            _eventDashboardView.TableView.Source = _tableViewDataSource;
            
            _supervisor = new EventDashboardViewControllerSupervisor(this);
            _event = serverEvent;
            if (serverEvent != null)
            {
                _eventDashboardView.Title = serverEvent.Name;
            }
            View = _eventDashboardView;

        }

        public class EventDashboardViewDelegate:UITableViewDelegate
        {
        }

        void EventDashboardViewOnCameraButtonPressed(object sender, EventArgs eventArgs)
        {
            OnCameraButtonPressed();
        }

        void EventDashboardBackButtonPressed(object sender, EventArgs eventArgs)
        {
           OnBackButtonPressed();
        }


        protected override void EnsureSupervised()
        {
            if (_supervisor == null)
            {
                _supervisor = new EventDashboardViewControllerSupervisor(this);
            }
        }

        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> CameraButtonPressed;
        public event EventHandler<ImageEventArgs> ImageSelected;

        void OnImageSelected(ImageEventArgs e)
        {
            EventHandler<ImageEventArgs> handler = ImageSelected;
            if (handler != null) handler(this, e);
        }

        public Event Event { get { return _event; } }

        public Photo[] Photos
        {
            get { return _tableViewDataSource.Photos; }
            set
            {

                InvokeOnMainThread(delegate
                    {
                        _tableViewDataSource.Photos = value;
                        Console.WriteLine("Reloading data");
                        _eventDashboardView.TableView.ReloadData();
                        _eventDashboardView.SetNeedsDisplay();
                    });

            }
        }

        void OnCameraButtonPressed()
        {
            EventHandler<EventArgs> handler = CameraButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void OnBackButtonPressed()
        {
            EventHandler<EventArgs> handler = BackButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void PresentLandingView()
        {
            var landingPageViewController = new LandingPageViewController();
            PresentViewController(landingPageViewController,true,Dispose);

        }

        public void PresentImagePickerView()
        {
            var imagePickerViewController = new ImagePickerViewController();
            imagePickerViewController.ImageCaptured += ImagePickerViewControllerOnImageCaptured;
            imagePickerViewController.Cancel += ImagePickerViewControllerOnCancel;
            PresentViewController(imagePickerViewController,true,null);
        }

        public void ProgressUploadMessage(float percentageDone)
        {
            BTProgressHUD.Show("Uploading",percentageDone,BTProgressHUD.MaskType.Gradient);
        }

        public void StartUploadMessage()
        {
            BTProgressHUD.Show("Uploading");
        }

        public void ClearUploadMessage(bool uploadOk)
        {
            BTProgressHUD.ShowSuccessWithStatus("Upload done");
        }

        public void ShowUpdatingMessage()
        {
            BTProgressHUD.ShowToast("Loading photos",false);
        }

        void ImagePickerViewControllerOnCancel(object sender, EventArgs eventArgs)
        {
            DismissViewController(true,null);
            var imagePickerViewController = sender as ImagePickerViewController;
            if (imagePickerViewController != null)
            {
                imagePickerViewController.ImageCaptured -= ImagePickerViewControllerOnImageCaptured;
                imagePickerViewController.Cancel -= ImagePickerViewControllerOnCancel;
                imagePickerViewController.Dispose();
            }
        }

        void ImagePickerViewControllerOnImageCaptured(object sender, ImageCaptureEvent imageCaptureEvent)
        {
            var imagePickerViewController = sender as ImagePickerViewController;
            if (imagePickerViewController != null)
            {
                imagePickerViewController.ImageCaptured -= ImagePickerViewControllerOnImageCaptured;
                imagePickerViewController.Cancel -= ImagePickerViewControllerOnCancel;
                DismissViewController(true,imagePickerViewController.Dispose);
            }
          
            InvokeInBackground(() =>
                {
                    var library = new ALAssetsLibrary();
                    var alaOrentation = ALAssetOrientation.Down;
                    switch (imageCaptureEvent.Image.Orientation)
                    {
                        case UIImageOrientation.Down:
                            alaOrentation = ALAssetOrientation.Down;
                            break;
                        case UIImageOrientation.DownMirrored:
                            alaOrentation = ALAssetOrientation.DownMirrored;
                            break;
                        case UIImageOrientation.Left:
                            alaOrentation = ALAssetOrientation.Left;
                            break;
                        case UIImageOrientation.LeftMirrored:
                            alaOrentation = ALAssetOrientation.LeftMirrored;
                            break;
                        case UIImageOrientation.Right:
                            alaOrentation = ALAssetOrientation.Right;
                            break;
                        case UIImageOrientation.RightMirrored:
                            alaOrentation = ALAssetOrientation.RightMirrored;
                            break;
                        case UIImageOrientation.Up:
                            alaOrentation = ALAssetOrientation.Up;
                            break;
                        case UIImageOrientation.UpMirrored:
                            alaOrentation = ALAssetOrientation.UpMirrored;
                            break;
                    }

                    library.WriteImageToSavedPhotosAlbum(imageCaptureEvent.Image.CGImage, alaOrentation,
                                                         (url, error) =>
                                                             {
                                                                 if (error != null)
                                                                     return;
                                                                 AddAssetToAlbum(url);
                                                             });
                });
            InvokeInBackground(() => UploadImage(imageCaptureEvent.Image));

        }

        void UploadImage(UIImage image)
        {

            var data = image.AsPNG();
            byte[] dataBytes = new byte[data.Length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes,dataBytes,0,Convert.ToInt32(data.Length));
            OnImageSelected(new ImageEventArgs {
                Image = dataBytes
            });
        }

        void AddAssetToAlbum(NSUrl result)
        {
            var library = new ALAssetsLibrary();
            bool albumWasFound = false;
            
            library.Enumerate(ALAssetsGroupType.Album, (ALAssetsGroup group, ref bool stop) =>
                {
                   
                    if (group!=null&&group.Name == "uSnap.us")
                    {
                       library.AssetForUrl(result, asset => 
                               group.AddAsset(asset),
                               error => { });
                        albumWasFound = true;
                        stop = true;
                        return;
                    }
                    if (group == null && !albumWasFound)
                    {
                        library.AddAssetsGroupAlbum("uSnap.us", 
                            albumGroup => library.AssetForUrl(result, asset =>albumGroup.AddAsset(asset)
                                , error => { }),
                                error => { });
                    }


                }, error =>
                    {
                        
                    });
           
            
        }


        protected override void Dispose(bool disposing)
        {
            _eventDashboardView.BackButtonPressed -= EventDashboardBackButtonPressed;
            _eventDashboardView.CameraButtonPressed -= EventDashboardViewOnCameraButtonPressed;
            _eventDashboardView = null;
            _supervisor = null;
            base.Dispose(disposing);
            
        }

        
    }

    public abstract class MyTableViewController:UITableViewController,IBaseViewController
    {
        protected abstract void EnsureSupervised();

        public override void ViewDidLoad()
        {
            EnsureSupervised();
            base.ViewDidLoad();
            OnLoad();
        }
        public override void ViewDidAppear(bool animated)
        {
            EnsureSupervised();
            base.ViewDidAppear(animated);
            OnAppear();
        }
        public override void ViewDidDisappear(bool animated)
        {
            // OnUnload();
            base.ViewDidDisappear(animated);
        }
        public event EventHandler<EventArgs> Load;

        protected virtual void OnLoad()
        {
            EventHandler<EventArgs> handler = Load;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Unload;

        protected virtual void OnUnload()
        {
            EventHandler<EventArgs> handler = Unload;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Appear;

        protected virtual void OnAppear()
        {
            EventHandler<EventArgs> handler = Appear;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> BeforeAppear;

        protected virtual void OnBeforeAppear()
        {
            EventHandler<EventArgs> handler = BeforeAppear;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        protected override void Dispose(bool disposing)
        {
            OnUnload();
            base.Dispose(disposing);
        }
    }

    public class EventTableViewDelegate:UITableViewSource
    {
        public EventTableViewDelegate()
        {
            Photos = new Photo[]{};
        }
        const string CellId = "PhotoTableViewCell";
        public Photo[] Photos { get; set; }
        public override int NumberOfSections(UITableView tableView)
        {
            return 1;
        }
        public override int RowsInSection(UITableView tableView, int section)
        {
            Console.WriteLine("loading rows in section. {0}",Photos.Length);
            return Photos.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId) as EventPhotoCell;
            var photo = Photos[indexPath.Row];
            if (cell == null)
            {
                cell = new EventPhotoCell(CellId, photo);
            }
            else
            {
                cell.UpdatePhoto(photo);
            }

            return cell;

        
        }
        
    }
}