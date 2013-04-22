using System;
using System.IO;
using Camera.Helpers;
using Camera.Messages;
using Camera.ViewControllers.Interfaces;
using TinyMessenger;

namespace Camera.Supervisors
{
    public class EventDashboardViewControllerSupervisor:BaseViewControllerSupervisor
    {
        IEventDashboardViewController _eventDashboardViewController;
        TinyMessageSubscriptionToken _photoProgressSubscription;
        TinyMessageSubscriptionToken _photoDoneSubscription;
        TinyMessageSubscriptionToken _subscription;

        public EventDashboardViewControllerSupervisor(IEventDashboardViewController viewController) : base(viewController)
        {
            _eventDashboardViewController = viewController;
            _eventDashboardViewController.BackButtonPressed += EventDashboardViewControllerOnBackButtonPressed;
            _eventDashboardViewController.CameraButtonPressed += EventDashboardViewControllerOnCameraButtonPressed;
            _eventDashboardViewController.ImageSelected += EventDashboardViewControllerOnImageSelected;
        }
        
        void EventDashboardViewControllerOnImageSelected(object sender, ImageEventArgs imageEventArgs)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(directory, Guid.NewGuid().ToString("N"));
            File.WriteAllBytes(filePath, imageEventArgs.Image);
            _eventDashboardViewController.StartUploadMessage();
            if (_photoProgressSubscription == null)
            {
                _photoProgressSubscription = StateManager.Current.MessageHub.Subscribe<UploadProgressMessage>(OnPhotoProgress);
            }
            if (_photoDoneSubscription == null)
            {
                _photoDoneSubscription = StateManager.Current.MessageHub.Subscribe<UploaderDoneMessage>(OnPhotoDone);
            }
            StateManager.Current.UploadPhoto(_eventDashboardViewController.Event,filePath);
        }
        protected override void ViewControllerAppear(object sender, EventArgs e)
        {
            base.ViewControllerAppear(sender,e);
            _subscription = StateManager.Current.MessageHub.Subscribe<EventPhotoListUpdatedMessage>(OnPhotosUpdated);
            _eventDashboardViewController.ShowUpdatingMessage();
            _eventDashboardViewController.Photos =
                StateManager.Current.GetEventPhotos(_eventDashboardViewController.Event);
            StateManager.Current.StartUpdatingPhotosForEvent(_eventDashboardViewController.Event);
        }

        void OnPhotosUpdated(EventPhotoListUpdatedMessage eventPhotoListUpdatedMessage)
        {
            _eventDashboardViewController.Photos =
              StateManager.Current.GetEventPhotos(_eventDashboardViewController.Event);
        }

        void OnPhotoDone(UploaderDoneMessage uploaderDoneMessage)
        {
            _eventDashboardViewController.ClearUploadMessage(true);
        }

        void OnPhotoProgress(UploadProgressMessage uploadProgressMessage)
        {
            _eventDashboardViewController.ProgressUploadMessage(uploadProgressMessage.BytsSent/((float)uploadProgressMessage.TotalBytes));
        }

        void EventDashboardViewControllerOnCameraButtonPressed(object sender, EventArgs eventArgs)
        {
            _eventDashboardViewController.PresentImagePickerView();
        }

        void EventDashboardViewControllerOnBackButtonPressed(object sender, EventArgs eventArgs)
        {
            _eventDashboardViewController.PresentLandingView();
        }
        public override void Dispose()
        {
            base.Dispose();
            StateManager.Current.StopUpdatingPhotosForEvent();
            if (_subscription != null)
            {
                StateManager.Current.MessageHub.Unsubscribe<EventPhotoListUpdatedMessage>(_subscription);
            }
            _eventDashboardViewController.BackButtonPressed -= EventDashboardViewControllerOnBackButtonPressed;
            _eventDashboardViewController.CameraButtonPressed -= EventDashboardViewControllerOnCameraButtonPressed;
            _eventDashboardViewController.ImageSelected -= EventDashboardViewControllerOnImageSelected;
            _eventDashboardViewController = null;
        }
    }
}