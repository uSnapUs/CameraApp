// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Camera.Supervisors;
using Camera.Tests.Helpers;
using Camera.ViewControllers.Interfaces;
using Machine.Fakes;
using Machine.Fakes.Adapters.Moq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Camera.Tests.ControllerSpecifications
{
    namespace EventDashboardViewControllerSupervisorSpecifications
    {
        [Subject(typeof(EventDashboardViewControllerSupervisor))]
        public abstract class EventDashboardViewControllerSupervisorSpecification:WithFakes<MoqFakeEngine>
        {
            static protected EventDashboardViewControllerSupervisor _sut;

            Establish context = () => _sut = new EventDashboardViewControllerSupervisor( (_mockViewController = new Mock<IEventDashboardViewController>()).Object);
            protected static Mock<IEventDashboardViewController> _mockViewController;
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

        public void PresentLandingView()
        {
            
        }

        public void PresentImagePickerView()
        {
            
        }
    }
}