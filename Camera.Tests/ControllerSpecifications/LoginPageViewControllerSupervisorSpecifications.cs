// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using Camera.Helpers;
using Camera.Model;
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
    namespace LoginPageViewControllerSupervisorSpecifications
    {
        [Subject(typeof (LandingPageViewControllerSupervisor))]
        public abstract class LoginViewControllerSupervisorSpecification : WithFakes<MoqFakeEngine>
        {
            protected static LoginViewControllerSupervisor _sut;
            protected static Mock<ILoginViewController> _viewController;
            Establish context = () =>
                {
                    StateManager.Current = An<IStateManager>();
                    _sut =
                        new LoginViewControllerSupervisor(
                            (_viewController = new Mock<ILoginViewController>()).Object);
                };
        }
        public class on_facebook_log_in:LoginViewControllerSupervisorSpecification
        {
            Because of = () => _viewController.Raise(vc=>vc.FacebookLoginPress+=null,(EventArgs)null);
            It should_initiate_a_facebook_login = () => StateManager.Current.WasToldTo(sm=>sm.InitiateFacebookLogin()) ;
        }
        public class on_view_controller_unload:LoginViewControllerSupervisorSpecification
        {
            Establish context = () => _sut = new LoginViewControllerSupervisor(_stubView);
            Because of = () => _stubView.OnUnload();
            It should_no_longer_have_any_views_wired_up = () => EventHelpers.GetAllEventHandlers(_stubView).Count().ShouldEqual(0);
            static StubLoginViewController _stubView = new StubLoginViewController();
        }
        
    }
}

namespace Camera.Tests.ControllerSpecifications.LoginPageViewControllerSupervisorSpecifications
{
    internal class StubLoginViewController : ILoginViewController
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
        public event EventHandler<EventArgs> FacebookLoginPress;
    }
}



