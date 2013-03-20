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
    namespace FindNearbyViewControllerSupervisorSpecifications
    {
        [Subject(typeof (FindNearbyViewControllerSupervisor))]
        public abstract class FindNearbyViewControllerSupervisorSpecification:WithFakes<MoqFakeEngine>
        {
            protected static FindNearbyViewControllerSupervisor _sut;
            Establish context = () => _sut = new FindNearbyViewControllerSupervisor((_viewController = new Mock<IFindNearbyViewController>()).Object);
            protected static Mock<IFindNearbyViewController> _viewController;
        }

        public class on_back_press:FindNearbyViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(vc=>vc.BackButtonPressed+=null,(EventArgs)null);
            It should_move_back_to_landing_view = () => _viewController.Object.WasToldTo(vc=>vc.GoToLandingView()) ;
        }


        public class on_view_controller_unload : FindNearbyViewControllerSupervisorSpecification
        {

            Establish context = () => _sut = new FindNearbyViewControllerSupervisor(_stubView);
            Because of = () => _stubView.OnUnload();

            It should_no_longer_have_any_views_wired_up =
                () => EventHelpers.GetAllEventHandlers(_stubView).Count().ShouldEqual(0);

            static StubFindNearbyViewController _stubView = new StubFindNearbyViewController();
        }
    }
}

namespace Camera.Tests.ControllerSpecifications.FindNearbyViewControllerSupervisorSpecifications
{
    internal class StubFindNearbyViewController : IFindNearbyViewController
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
        public void GoToLandingView()
        {
            
        }
    }
}