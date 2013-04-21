// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Linq;
using System.Text;
using Camera.Helpers;
using Camera.Model;
using Camera.Supervisors;
using Camera.Tests.Helpers;
using Camera.ViewControllers.Interfaces;
using Machine.Fakes;
using Machine.Fakes.Adapters.Moq;
using Machine.Specifications;
using Moq;
using TinyMessenger;
using It = Machine.Specifications.It;

namespace Camera.Tests.ControllerSpecifications
{
    namespace EventDashboardViewControllerSupervisorSpecifications
    {
        [Subject(typeof(EventDashboardViewControllerSupervisor))]
        public abstract class EventDashboardViewControllerSupervisorSpecification:WithFakes<MoqFakeEngine>
        {
            static protected EventDashboardViewControllerSupervisor _sut;

            Establish context = () =>
                {
                    _sut =
                        new EventDashboardViewControllerSupervisor(
                            (_mockViewController = new Mock<IEventDashboardViewController>()).Object);
                    StateManager.Current = (_stateManager = An<IStateManager>());
                    StateManager.Current.WhenToldTo(sm=>sm.MessageHub).Return((_messageHub=An<ITinyMessengerHub>()));
                };
            protected static Mock<IEventDashboardViewController> _mockViewController;
            protected static IStateManager _stateManager;
            static ITinyMessengerHub _messageHub;
        }
        
        public class on_back_pressed:EventDashboardViewControllerSupervisorSpecification
        {
            Because of = () => _mockViewController.Raise(vc=>vc.BackButtonPressed+=null,(EventArgs)null);
            It should_go_to_landing_page = () => _mockViewController.Object.WasToldTo(vc=>vc.PresentLandingView());
        }

        public class on_camera_pressed:EventDashboardViewControllerSupervisorSpecification
        {
            Because of = () => _mockViewController.Raise(vc=>vc.CameraButtonPressed+=null,(EventArgs)null);
            It should_go_to_camera_view = () => _mockViewController.Object.WasToldTo(vc=>vc.PresentImagePickerView());
        }

        public class on_view_controller_unload : EventDashboardViewControllerSupervisorSpecification
        {

            Establish context = () => _sut = new EventDashboardViewControllerSupervisor(_stubView);
            Because of = () => _stubView.OnUnload();

            It should_no_longer_have_any_views_wired_up =
                () => EventHelpers.GetAllEventHandlers(_stubView).Count().ShouldEqual(0);

            static StubEventDashboardViewController _stubView = new StubEventDashboardViewController();
        }
        public class on_view_controller_load:EventDashboardViewControllerSupervisorSpecification
        {
            Establish context = () =>
                {
                    _mockViewController.Object.WhenToldTo(vc => vc.Event).Return(_event);
                    _stateManager.WhenToldTo(sm => sm.GetEventPhotos(_event)).Return(_photos);

                };
            Because of = () => _mockViewController.Raise(vc=>vc.Appear+=null,(EventArgs)null);
            It should_start_updating_photos = () => _stateManager.WasToldTo(sm=>sm.StartUpdatingPhotosForEvent(_event));
            It should_populate_initial_events = () => _mockViewController.VerifySet(vc=>vc.Photos=_photos);
            static Event _event = new Event();
            static Photo[] _photos = new[] {
                new Photo(), 
            };
        }
        public class on_saving_photo:EventDashboardViewControllerSupervisorSpecification
        {
            Establish context = () =>
                {
                    _mockViewController.Object.WhenToldTo(vc=>vc.Event).Return(_event);
                    _imageBytes = Encoding.UTF8.GetBytes("Some data");
                    _stateManager.WhenToldTo(s => s.UploadPhoto(_event,Moq.It.IsAny<string>())).Callback((Event ev,string path) =>
                        { _imagePath = path; });
                };
            Because of = () => _mockViewController.Raise(vc=>vc.ImageSelected+=null,new ImageEventArgs{Image=_imageBytes});
            It should_set_image_path = () => _imagePath.ShouldNotBeNull();
            It should_create_an_upload_image = () => File.Exists(_imagePath).ShouldBeTrue();
            It should_save_correct_bytes = () => File.ReadAllBytes(_imagePath).ShouldEqual(_imageBytes);
            static byte[] _imageBytes;
            static string _imagePath;
            static Event _event= new Event();
        }
    }
}

namespace Camera.Tests.ControllerSpecifications.EventDashboardViewControllerSupervisorSpecifications
{
    internal class StubEventDashboardViewController : IEventDashboardViewController
    {
        public event EventHandler<EventArgs> Load;
        public event EventHandler<EventArgs> Unload;

        public virtual void OnUnload()
        {
            EventHandler<EventArgs> handler = Unload;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Appear;
        public event EventHandler<EventArgs> BeforeAppear;
        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> CameraButtonPressed;
        public event EventHandler<ImageEventArgs> ImageSelected;

        public Event Event { get { return null; } }
        public Photo[] Photos { get; set; }

        public void PresentLandingView()
        {
            
        }

        public void PresentImagePickerView()
        {
            
        }

        public void ProgressUploadMessage(float percentageDone)
        {
            
        }

        public void StartUploadMessage()
        {
            
        }

        public void ClearUploadMessage(bool uploadOk)
        {
            
        }

        public void ShowUpdatingMessage()
        {
            
        }
    }
}