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
    namespace LandingPageViewControllerSupervisorSpecifications
    {
        [Subject(typeof (LandingPageViewControllerSupervisor))]
        public abstract class LandingPageViewControllerSupervisorSpecification : WithFakes<MoqFakeEngine>
        {
            protected static LandingPageViewControllerSupervisor _sut;
            protected static Mock<ILandingPageViewController> _viewController;
            Establish context = () =>
                {
                    StateManager.Current = An<IStateManager>();
                    _sut =
                        new LandingPageViewControllerSupervisor(
                            (_viewController = new Mock<ILandingPageViewController>()).Object);
                };
        }
        public class when_logged_in_on_my_events_press:LandingPageViewControllerSupervisorSpecification
        {
            Establish context = () => StateManager.Current.WhenToldTo(sm=>sm.IsAuthenticated).Return(true);
            Because of = () => _viewController.Raise(viewController=>viewController.MyEventsButtonPressed+=null,(EventArgs)null);
            It should_present_my_events_view = () => _viewController.Object.WasToldTo(vc=>vc.PresentMyEventsView()) ;
        }
        public class when_not_logged_in_on_my_events_press : LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController => viewController.MyEventsButtonPressed += null, (EventArgs)null);
            It should_not_present_my_events_view = () => _viewController.Object.WasNotToldTo(vc => vc.PresentMyEventsView());
            It should_present_login_view = () =>_viewController.Object.WasToldTo(vc=>vc.PresentLoginView("looking up your events")) ;
        }
        public class on_find_events_press:LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController => viewController.FindButtonPressed += null, (EventArgs)null);
            It should_present_find_events_view = () => _viewController.Object.WasToldTo(vc => vc.PresentFindEventsView());
        }
        public class when_logged_in_on_create_event_press : LandingPageViewControllerSupervisorSpecification
        {
            Establish context = () => StateManager.Current.WhenToldTo(sm => sm.IsAuthenticated).Return(true);
            Because of = () => _viewController.Raise(viewController => viewController.CreateButtonPressed += null, (EventArgs)null);
            It should_present_create_event_view = () => _viewController.Object.WasToldTo(vc => vc.PresentCreateView());
        }
        public class when_not_logged_in_on_create_event_press : LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController => viewController.CreateButtonPressed += null, (EventArgs)null);
            It should_not_present_create_event_view = () => _viewController.Object.WasNotToldTo(vc => vc.PresentCreateView());
            It should_present_login_view = () => _viewController.Object.WasToldTo(vc=>vc.PresentLoginView("creating an event"));
        }
        public class on_back_button_press:LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController=>viewController.BackButtonPressed+=null,(EventArgs)null);
            It should_hide_find_events_view = () => _viewController.Object.WasToldTo(vc=>vc.HideFindEventsView());
        }
        public class on_find_nearby_button_press : LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController => viewController.FindNearbyButtonPressed += null, (EventArgs)null);
            It should_show_find_nearby_view = () => _viewController.Object.WasToldTo(vc => vc.PresentFindNearbyView());
        }
        public class on_join_button_press_with_nothing_in_code : LandingPageViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(viewController => viewController.JoinButtonPressed += null, (EventArgs)null);
            It should_show_validation_message_on_screen = () => _viewController.Object.WasToldTo(vc => vc.ShowValidationMessage("Event code cannot be empty"));
        }
        public class on_join_button_press_with_a_code : LandingPageViewControllerSupervisorSpecification
        {
            Establish context = () => _viewController.Object.WhenToldTo(vc=>vc.EventCode).Return("Code");
            Because of = () => _viewController.Raise(viewController => viewController.JoinButtonPressed += null, (EventArgs)null);
            It should_navigate_to_event_dashboard = () => _viewController.Object.WasToldTo(vc => vc.PresentEventDashboard());
        }
        public class on_view_controller_unload:LandingPageViewControllerSupervisorSpecification
        {
            Establish context = () => _sut = new LandingPageViewControllerSupervisor(_stubView);
            Because of = () => _stubView.OnUnload();
            It should_no_longer_have_any_views_wired_up = () => EventHelpers.GetAllEventHandlers(_stubView).Count().ShouldEqual(0);
            static StubLandingViewController _stubView = new StubLandingViewController();
        }
        
    }
}

namespace Camera.Tests.ControllerSpecifications.LandingPageViewControllerSupervisorSpecifications
{
    internal class StubLandingViewController:ILandingPageViewController
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
        public event EventHandler<EventArgs> FindButtonPressed;
        public event EventHandler<EventArgs> CreateButtonPressed;
        public event EventHandler<EventArgs> MyEventsButtonPressed;
        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> FindNearbyButtonPressed;
        public event EventHandler<EventArgs> JoinButtonPressed;
        public string EventCode { get; set; }

        public void PresentMyEventsView()
        {
        }

        public void PresentFindEventsView()
        {
        }

        public void PresentCreateView()
        {
        }

        public void HideFindEventsView()
        {
            
        }

        public void PresentFindNearbyView()
        {
            
        }

        public void PresentLoginView(string loginReason)
        {
            
        }

        public void ShowValidationMessage(string validationMessage)
        {
            
        }

        public void PresentEventDashboard()
        {
            
        }
    }
}

