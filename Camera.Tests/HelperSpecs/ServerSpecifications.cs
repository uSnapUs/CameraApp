// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Camera.Exceptions;
using Camera.Helpers;
using Camera.Model;
using Machine.Fakes;
using Machine.Fakes.Adapters.Moq;
using Machine.Specifications;
using RestSharp;

namespace Camera.Tests.HelperSpecs
{
    namespace ServerSpecifications
    {
        [Subject(typeof(Server))]
        public abstract class ServerSpecification : WithFakes<MoqFakeEngine>
        {

            protected static Server _sut;


            Establish context = () =>
            {
                _baseUrl = "http://192.168.0.110:3000/";
                _restClientFactory = An<IRestClientFactory>();
                _restClient = An<IRestClient>();
                _devicesRestRequest = An<IRestRequest>();
                _eventsRestRequest = An<IRestRequest>();
                _devicesRestResponse = An<IRestResponse<DeviceRegistration>>();
                _eventsRestResponse = An<IRestResponse<Event>>();
                _devicesRestResponse.StatusCode = HttpStatusCode.OK;
                _eventsRestResponse.StatusCode = HttpStatusCode.OK;
                _restClientFactory.WhenToldTo(factory => factory.CreateClient(_baseUrl)).Return(_restClient);
                
                _restClientFactory.WhenToldTo(factory => factory.CreateRestRequest("devices", Method.POST)).Return(_devicesRestRequest);
                _devicesRestRequest.WhenToldTo(request => request.Parameters).Return(_params);
                _restClient.WhenToldTo(client => client.Execute<DeviceRegistration>(_devicesRestRequest)).Return(_devicesRestResponse);

                _restClientFactory.WhenToldTo(factory => factory.CreateRestRequest("events", Method.POST)).Return(_eventsRestRequest);
                _eventsRestRequest.WhenToldTo(request => request.Parameters).Return(_params);
                _restClient.WhenToldTo(client => client.Execute<Event>(_eventsRestRequest)).Return(_eventsRestResponse);

                _sut = new Server(new NullLogger())
                {

                    RestClientFactory = _restClientFactory,
                    BaseUrl = _baseUrl
                };
            };

            protected static IRestClientFactory _restClientFactory;
            protected static IRestClient _restClient;
            protected static IRestRequest _devicesRestRequest;
            protected static IRestResponse<DeviceRegistration> _devicesRestResponse;
            static string _baseUrl;
            protected static List<Parameter> _params = new List<Parameter>();
            static IRestRequest _eventsRestRequest;
            protected static IRestResponse<Event> _eventsRestResponse;
        }
        [Subject(typeof(Server))]
        [Tags("Integration")]
        public abstract class IntegrationServerSpecification : WithFakes<MoqFakeEngine>
        {
            protected static Server _sut;


            Establish context = () =>
            {
                _baseUrl = "http://api.stage.isnap.us";
                
                _sut = new Server(new NullLogger())
                {
                    RestClientFactory = new RestClientFactory(),
                    BaseUrl = _baseUrl
                };
            };

            Cleanup cleanup = () => ResetDevices(_baseUrl);
            static void ResetDevices(string baseUrl)
            {
                var uri = new Uri(baseUrl);
                var request = WebRequest.Create(new Uri(uri, "/devices"));
                request.Method = "DELETE";
                var response = request.GetResponse();
                response.Dispose();
            }

            protected static IRestClientFactory _restClientFactory;
            protected static IRestClient _restClient;
            protected static IRestRequest _restRequest;
            protected static IRestResponse<DeviceRegistration> _restResponse;
            static string _baseUrl;
            protected static List<Parameter> _params = new List<Parameter>();
        }
        public class when_registering_initial_device : ServerSpecification
        {
            Establish context = () => _devicesRestResponse.WhenToldTo(response => response.Data).Return(_serverDeviceRegistration);
            Because of = () => _result = _sut.RegisterDevice(_deviceRegistration);

            It should_return_correct_device = () => _result.ShouldEqual(_serverDeviceRegistration);

            It should_send_a_device_to_the_server = () => _devicesRestRequest.WasToldTo(request => request.AddBody(_deviceRegistration));
            
            static DeviceRegistration _deviceRegistration = new DeviceRegistration
            {
                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",

            };

            static DeviceRegistration _serverDeviceRegistration = new DeviceRegistration
            {
                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                ServerId = "TESTID",
                Token = "TOKEN"

            };

            static DeviceRegistration _result;
        }
        public class when_creating_a_new_event:ServerSpecification
        {
            Establish context = () => _eventsRestResponse.WhenToldTo(response=>response.Data).Return(_serverEvent);
            Because of = () => _result = _sut.CreateEvent(_event);
            It should_retrn_correct_event = () => _result.ShouldEqual(_serverEvent);
            static Event _serverEvent = new Event {Name = "server name"};
            static Event _event = new Event() {Name = "original name"};
            static Event _result;
        }

        public class integration_when_registering_initial_device : IntegrationServerSpecification
        {
            Because of = () => _result = _sut.RegisterDevice(_deviceRegistration);

            It should_return_correct_device = () => _result.Guid.ShouldEqual(_deviceRegistration.Guid);
            It should_return_a_token = () => _result.Token.ShouldNotBeNull(); 
            static DeviceRegistration _deviceRegistration = new DeviceRegistration
            {

                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                Name = "test device"

            };



            static DeviceRegistration _result;
        }

        public class integration_when_updating_device_registration : IntegrationServerSpecification
        {
            Establish context = () =>
                {
                    _initialDevice = _sut.RegisterDevice(_initialDevice);
                    _deviceRegistration.ServerId = _initialDevice.ServerId;
                    _sut.SetDeviceCredentials(_initialDevice.Guid, _initialDevice.Token);
                };
            Because of = () => _result = _sut.RegisterDevice(_deviceRegistration);

            It should_return_correct_device = () => _result.Guid.ShouldEqual(_deviceRegistration.Guid);
            It should_return_correct_facebook_id = () =>_result.FacebookId.ShouldEqual(_deviceRegistration.FacebookId);
            It should_return_correct_server_id = () =>_result.ServerId.ShouldEqual(_deviceRegistration.ServerId) ;
            static DeviceRegistration _deviceRegistration = new DeviceRegistration
            {

                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                Name = "test device",
                FacebookId = "facebook_id"
            };



            static DeviceRegistration _result;
            static DeviceRegistration _initialDevice = new DeviceRegistration {
                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                Name = "initial device"
            };
        }

        public class integration_when_registering_an_invalid_device : IntegrationServerSpecification
        {
            Because of = () => _exception = Catch.Exception(() => _sut.RegisterDevice(_deviceRegistration));

            It should_throw_an_error = () => _exception.ShouldNotBeNull();
            It should_throw_a_validation_exception = () => _exception.ShouldBeOfType<ApiException>();

            static DeviceRegistration _deviceRegistration = new DeviceRegistration
            {

                Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",

            };

            static Exception _exception;
        }

        public class integration_when_creating_event : IntegrationServerSpecification
        {
            Establish context = () =>
                {
                    _deviceRegistration = _sut.RegisterDevice(new DeviceRegistration {

                        Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                        Name = "test device",
                        FacebookId = "facebook_id"
                        //Email = "owen@iouk.com"
                    });
                    _sut.SetDeviceCredentials(_deviceRegistration.Guid, _deviceRegistration.Token);
                };
            Because of = () => _result = _sut.CreateEvent(_event);

            It should_return_correct_event = () => _result.Name.ShouldEqual(_event.Name);
            It should_return_a_code = () => _result.Code.ShouldNotBeNull();
            static Event _event = new Event {
                Name = "Name",
                Address = "An Address",
                Location = new Point {Latitude = 0.1, Longitude = 0.1},
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                IsPublic = true,
                Code = null

            };

            static DeviceRegistration _deviceRegistration;
            static Event _result;
        }
        public class integration_when_asking_for_an_existing_event_by_code : IntegrationServerSpecification
        {
            Establish context = () =>
            {
                _deviceRegistration = _sut.RegisterDevice(new DeviceRegistration
                {

                    Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                    Name = "test device",
                    FacebookId = "facebook_id"
                });
                _sut.SetDeviceCredentials(_deviceRegistration.Guid, _deviceRegistration.Token);
                _existing_event = _sut.CreateEvent(_event);
            };
            Because of = () => _result = _sut.FindEvent(_existing_event.Code);

            It should_return_correct_event = () => _result.Name.ShouldEqual(_event.Name);
            It should_return_a_code = () => _result.Code.ShouldNotBeNull();
            static Event _event = new Event
            {
                Name = "Name",
                Address = "An Address",
                Location = new Point {Latitude = 0.1, Longitude = 0.1 },
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                IsPublic = true,
                Code = null

            };

            static DeviceRegistration _deviceRegistration;
            static Event _result;
            static Event _existing_event;
        }
        public class integration_when_asking_for_a_existing_events_by_location : IntegrationServerSpecification
        {
            Establish context = () =>
            {
                _deviceRegistration = _sut.RegisterDevice(new DeviceRegistration
                {

                    Guid = "0F0F187A-9AD5-461A-BB56-810BFEF41553",
                    Name = "test device",
                    FacebookId = "facebook_id"
                });
                _sut.SetDeviceCredentials(_deviceRegistration.Guid, _deviceRegistration.Token);
                _existing_event1 = _sut.CreateEvent(_event1);
                _existing_event2 = _sut.CreateEvent(_event2);
            };
            Because of = () => _result = _sut.FindEventsByLocation(new Coordinate(){Latitude = 0.1,Longitude = 0.1});

            It should_return_correct_events = () => _result.Select(ev=>ev.Name).ShouldContain(_event1.Name,_event2.Name);
            It should_return_correct_codes = () => _result.Select(ev => ev.Code).ShouldContain(_existing_event1.Code, _existing_event2.Code);
            static Event _event1 = new Event
            {
                Name = "Name",
                Address = "An Address",
                Location = new Point { Latitude = 0.11, Longitude = 0.1 },
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                IsPublic = true,
                Code = null

            };
            static Event _event2 = new Event
            {
                Name = "Name2",
                Address = "An Address2",
                Location = new Point { Latitude = 0.10, Longitude = 0.1 },
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                IsPublic = true,
                Code = null

            };

            static DeviceRegistration _deviceRegistration;
            static Event[] _result;
            static Event _existing_event1;
            static Event _existing_event2;
        }
    }
}