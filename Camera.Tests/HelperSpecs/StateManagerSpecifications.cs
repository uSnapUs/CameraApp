// ReSharper disable InconsistentNaming
using System;
using System.IO;
using System.Linq;
using Camera.Helpers;
using Camera.Messages;
using Camera.Model;
using Machine.Fakes;
using Machine.Fakes.Adapters.Moq;
using Machine.Specifications;
using TinyMessenger;

namespace Camera.Tests.HelperSpecs
{
    namespace StateManagerSpecifications
    {

        [Subject(typeof (StateManager))]
        public abstract class StateManagerSpecification : WithFakes<MoqFakeEngine>
        {
            protected static StateManager _sut;


            Establish context = () =>
                {
                    _sut = new StateManager {
                        Server = (_server = An<IServer>()),
                        MessageHub = (_messageHub = An<ITinyMessengerHub>()),
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

            protected static IServer _server;
            protected static ITinyMessengerHub _messageHub;
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
                    _sut.CurrentDeviceRegistration = new DeviceRegistration {
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

            It should_send_the_registration_to_the_server =
                () => _sut.Server.WasToldTo(s => s.RegisterDevice(_sut.CurrentDeviceRegistration));

            It should_mark_user_as_authenticated = () => _sut.IsAuthenticated.ShouldBeTrue();
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

            It should_send_the_registration_to_the_server =
                () => _sut.Server.WasToldTo(s => s.RegisterDevice(_sut.CurrentDeviceRegistration));

            It should_mark_user_as_authenticated = () => _sut.IsAuthenticated.ShouldBeTrue();
            static string _name = "name";
            static string _email = "email@email.com";
            static string _guid = Guid.NewGuid().ToString("N");
            static string _facebookId = "fbid";
        }

        public class when_initiating_facebook_login : StateManagerSpecification
        {
            Establish context = () => FacebookSession.Current = An<IFacebookSession>();
            Because of = () => _sut.InitiateFacebookLogin();
            It should_initiate_facebook_login = () => FacebookSession.Current.WasToldTo(fb => fb.InitiateLogin());
        }

        public class when_registrating_device_without_facebook_id : StateManagerSpecification
        {
            Establish context = () =>
                {
                    _sut.CurrentDeviceRegistration = null;
                };

            Because of = () => _sut.DeviceName = _name;
            It should_update_current_registration_name = () => _sut.CurrentDeviceRegistration.Name.ShouldEqual(_name);
            It should_update_current_registration_email = () => _sut.CurrentDeviceRegistration.Email.ShouldBeNull();

            It should_update_current_facebook_id =
                () => _sut.CurrentDeviceRegistration.FacebookId.ShouldBeNull();

            It should_not_register_a_guid = () => _sut.CurrentDeviceRegistration.Guid.ShouldNotBeEmpty();

            It should_send_the_registration_to_the_server =
                () => _sut.Server.WasToldTo(s => s.RegisterDevice(_sut.CurrentDeviceRegistration));

            It should_mark_user_as_not_authenticated = () => _sut.IsAuthenticated.ShouldBeFalse();
            static string _name = "name";

        }

        public class when_creating_event : StateManagerSpecification
        {
            Establish context = () => _server.WhenToldTo(s => s.CreateEvent(_event)).Return(_returnedEvent);
            Because of = () => _result = _sut.CreateEvent(_event);
            It should_send_event_to_Server = () => _server.WasToldTo(s => s.CreateEvent(_event));
            It should_return_returned_event = () => _result.ShouldEqual(_returnedEvent);
            static Event _event = new Event {Name = "Event"};
            static Event _returnedEvent = new Event {Name = "Returned Event"};
            static Event _result;
        }

        public class when_uploading_photo : StateManagerSpecification
        {

            Establish context = () =>
                {
                    var assemblyPath = new FileInfo(typeof (when_uploading_photo).Assembly.Location).Directory;
                    _photoFilePath = assemblyPath + "/TestData/house.jpg";


                };

            Because of = () => _sut.UploadPhoto(_event, _photoFilePath);

            It should_upload_photo_to_server =
                () => _server.WasToldTo(s => s.PostPhoto(_event.Code, _photoFilePath, Moq.It.IsAny<Guid>()));

            It should_subscibe_to_photo_upload_done_events =
                () =>
                _messageHub.WasToldTo(messageHub => messageHub.Subscribe(Moq.It.IsAny<Action<UploaderDoneMessage>>()));

            It should_subscribe_to_photo_progress_events =
                () =>
                _messageHub.WasToldTo(messageHub => messageHub.Subscribe(Moq.It.IsAny<Action<UploadProgressMessage>>()));

            static string _photoFilePath;
            static Event _event = new Event {Code = "EventCode"};
        }
        public class when_updating_photos_for_an_event_with_no_photos:StateManagerSpecification
        {
            Establish context = () =>
                {
                    _existingEvent = new Event() {
                        Code="code"
                    };
                    StateManager.Db.Insert(_existingEvent);
                    _server.WhenToldTo(s=>s.GetPhotos(Moq.It.IsAny<Event>(),null)).Return(_photos);
                };
            Because of = () => _sut.UpdatePhotosForEvent(_existingEvent);
            It should_call_the_server_to_find_photos = () => _server.WasToldTo(s=>s.GetPhotos(_existingEvent,null));
            It should_save_photos = () => StateManager.Db.Table<Photo>().Count().ShouldEqual(2);
            It should_save_correct_event_for_photos = () => StateManager.Db.Table<Photo>().ToArray().Count(p => p.EventCode.Equals(_existingEvent.Code,StringComparison.OrdinalIgnoreCase)).ShouldEqual(2);
            It should_publish_update_event = () => _messageHub.WasToldTo(m => m.PublishAsync(Moq.It.Is<EventPhotoListUpdatedMessage>(message=>message.EventCode.Equals(_existingEvent.Code))));
            static Event _existingEvent;
            static Photo[] _photos = new[] {
                new Photo{ServerId = "serverId1"}, new Photo{ServerId = "serverId2"} 
            };
        }
    }
}
