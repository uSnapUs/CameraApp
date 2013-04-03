// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Camera.Helpers;
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
    namespace CreateEventViewControllerSupervisorSpecifications
    {
        [Subject(typeof(CreateEventViewControllerSupervisor))]
        public abstract class CreateEventViewControllerSupervisorSpecification:WithFakes<MoqFakeEngine>
        {
            static protected CreateEventViewControllerSupervisor _sut;

            Establish context = () => _sut = new CreateEventViewControllerSupervisor((_mockViewController = new Mock<ICreateEventViewController>()).Object);
            protected static Mock<ICreateEventViewController> _mockViewController;
        }

        public class when_back_button_is_pressed:CreateEventViewControllerSupervisorSpecification
        {
            Because of = () => _mockViewController.Raise(vc=>vc.BackPressed+=null,(EventArgs)null);
            It should_present_the_landing_view = () => _mockViewController.Object.WasToldTo(vc=>vc.PresentLandingView()) ;
        }

        public class when_location_lookup_shown:CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () => StateManager.Current.LocationCoder = (_locationCoder = An<ILocationCoder>());
            Because of = () => _mockViewController.Raise(vc=>vc.LocationLookupShown+=null,(EventArgs)null);
            static ILocationCoder _locationCoder;
        }

        public class on_view_controller_unload : CreateEventViewControllerSupervisorSpecification
        {

            Establish context = () => _sut = new CreateEventViewControllerSupervisor(_stubView);
            Because of = () => _stubView.OnUnload();

            It should_no_longer_have_any_views_wired_up =
                () => EventHelpers.GetAllEventHandlers(_stubView).Count().ShouldEqual(0);

            static StubCreateEventViewController _stubView = new StubCreateEventViewController();
        }

    }
}

namespace Camera.Tests.ControllerSpecifications.CreateEventViewControllerSupervisorSpecifications
{
    internal class StubCreateEventViewController : ICreateEventViewController
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
        public void PresentLandingView()
        {
            
        }

        public event EventHandler<EventArgs> BackPressed;
        public event EventHandler<EventArgs> LocationEdited;
    }
}