// ReSharper disable InconsistentNaming
using System;
using System.IO;
using Camera.Helpers;
using Camera.Model;
using Machine.Fakes;
using Machine.Fakes.Adapters.Moq;
using Machine.Specifications;
using TinyMessenger;

namespace Camera.Tests.HelperSpecs
{
    namespace StateManagerSpecifications
    {

        [Subject(typeof(StateManager))]
        public abstract class StateManagerSpecification : WithFakes<MoqFakeEngine>
        {
            protected static StateManager _sut;


            Establish context = () =>

            {
                _sut = new StateManager
                {
                    Server = (_server = An<IServer>()),
                    MessageHub = An<ITinyMessengerHub>(),
                };
                _sut.Server.WhenToldTo(s => s.RegisterDevice(Moq.It.IsAny<DeviceRegistration>())).Return((
                    DeviceRegistration d) =>
                {
                    return d;
                });
            };

            Cleanup cleanup = () =>
            {
                _sut.Dispose();

                File.Delete(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                                 "usnapus.sqlite"));
            };

            static IServer _server;
        }

        public class when_calling_current : StateManagerSpecification
        {
            Because of = () => _return = StateManager.Current;
            It should_return_a_valid_state_manager = () => _return.ShouldNotBeNull();
            It should_return_a_singleton_instance = () => _return.ShouldBeTheSameAs(StateManager.Current);

            Cleanup cleanup = () => StateManager.Current.Dispose();
            static IStateManager _return;
        }


        public class when_registration_is_updated_with_existing_registration : StateManagerSpecification
        {
            Establish context = () =>
            {
                _sut.CurrentDeviceRegistration = new DeviceRegistration
                {
                    Name = null,
                    Email = null,
                    FacebookId = null,
                    Id = 1,
                    Guid = _guid
                };

            };
            Because of = () => _sut.UpdateDeviceRegistration(_name, _email, _facebookId);
            It should_update_current_registration_name = () => _sut.CurrentDeviceRegistration.Name.ShouldEqual(_name);
            It should_update_current_registration_email = () => _sut.CurrentDeviceRegistration.Email.ShouldEqual(_email);
            It should_not_change_the_guid_registered = () => _sut.CurrentDeviceRegistration.Guid.ShouldEqual(_guid);
            It should_update_current_facebook_id =
                () => _sut.CurrentDeviceRegistration.FacebookId.ShouldEqual(_facebookId);
            It should_send_the_registration_to_the_server = () => _sut.Server.WasToldTo(s => s.RegisterDevice(_sut.CurrentDeviceRegistration));
            static string _name = "name";
            static string _email = "email@email.com";
            static string _guid = Guid.NewGuid().ToString("N");
            static string _facebookId = "fbid";
        }
        public class when_registration_is_updated_with_no_existing_registration : StateManagerSpecification
        {
            Establish context = () =>
            {
                _sut.CurrentDeviceRegistration = null;
            };
            Because of = () => _sut.UpdateDeviceRegistration(_name, _email, _facebookId);
            It should_update_current_registration_name = () => _sut.CurrentDeviceRegistration.Name.ShouldEqual(_name);
            It should_update_current_registration_email = () => _sut.CurrentDeviceRegistration.Email.ShouldEqual(_email);

            It should_update_current_facebook_id =
                () => _sut.CurrentDeviceRegistration.FacebookId.ShouldEqual(_facebookId);
            It should_not_register_a_guid = () => _sut.CurrentDeviceRegistration.Guid.ShouldNotBeEmpty();
            It should_send_the_registration_to_the_server = () => _sut.Server.WasToldTo(s => s.RegisterDevice(_sut.CurrentDeviceRegistration));
            static string _name = "name";
            static string _email = "email@email.com";
            static string _guid = Guid.NewGuid().ToString("N");
            static string _facebookId = "fbid";
        }
        public class when_initiating_facebook_login:StateManagerSpecification
        {
            Establish context = () => FacebookSession.Current=An<IFacebookSession>();
            Because of = () => _sut.InitiateFacebookLogin();
            It should_initiate_facebook_login = () => FacebookSession.Current.WasToldTo(fb=>fb.InitiateLogin());
        }
    }
}
