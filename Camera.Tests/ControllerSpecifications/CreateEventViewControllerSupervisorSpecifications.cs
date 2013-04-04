// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
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
    namespace CreateEventViewControllerSupervisorSpecifications
    {
        [Subject(typeof(CreateEventViewControllerSupervisor))]
        public abstract class CreateEventViewControllerSupervisorSpecification:WithFakes<MoqFakeEngine>
        {
            static protected CreateEventViewControllerSupervisor _sut;

            Establish context = () =>
                {
                    StateManager.Current = (_stateManager = An<IStateManager>());
                    _stateManager.WhenToldTo(sm=>sm.LocationManager).Return((_locationManager=An<ILocationManager>()));
                    _sut =
                        new CreateEventViewControllerSupervisor(
                            (_mockViewController = new Mock<ICreateEventViewController>()).Object);
                };
            protected static Mock<ICreateEventViewController> _mockViewController;
            protected static ILocationManager _locationManager;
            static IServer _server;
            protected static IStateManager _stateManager;
        }

        public class when_back_button_is_pressed:CreateEventViewControllerSupervisorSpecification
        {
            Because of = () => _mockViewController.Raise(vc=>vc.BackPressed+=null,(EventArgs)null);
            It should_present_the_landing_view = () => _mockViewController.Object.WasToldTo(vc=>vc.PresentLandingView()) ;
        }

        public class when_location_lookup_shown:CreateEventViewControllerSupervisorSpecification
        {
            
            Because of = () => _mockViewController.Raise(vc=>vc.LocationLookupShown+=null,(EventArgs)null);
            It should_ask_location_manager_to_update_location = () => _locationManager.WasToldTo(lm=>lm.SubscribeToLocationUpdates(_sut.OnLocationUpdated));
        }
        public class when_location_search_invoked:CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () => _mockViewController.Object.WhenToldTo(vc=>vc.LocationSearchText).Return(_searchText);
            Because of = () => _mockViewController.Raise(vc => vc.LocationSearch+=null,(EventArgs)null);
            It should_ask_location_manager_to_looup_address = () => _locationManager.WasToldTo(lm=>lm.LookupAddress(_searchText,_sut.OnAddressLookup)) ;
            static string _searchText = "search";
        }
        public class when_location_search_invoked_with_no_text_in_search_box : CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () => _mockViewController.Object.WhenToldTo(vc => vc.LocationSearchText).Return(_searchText);
            Because of = () => _mockViewController.Raise(vc => vc.LocationSearch += null, (EventArgs)null);
            It should_not_ask_location_manager_to_looup_address = () => _locationManager.WasNotToldTo(lm => lm.LookupAddress(_searchText, _sut.OnAddressLookup));
            static string _searchText;
        }
        public class when_addresses_are_returned:CreateEventViewControllerSupervisorSpecification
        {
            Because of = () => _sut.OnAddressLookup(_addresses);
            It should_send_the_addresses_to_the_view_controller = () => _mockViewController.Object.WasToldTo(vc=>vc.AddAddressesToLocationMap(_addresses));
            static IEnumerable<AddressDetails> _addresses = new[] {
                new AddressDetails()
            };
        }
        public class when_create_pressed:CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () =>
                {
                    _mockViewController.Object.WhenToldTo(vc => vc.Name).Return(_name);
                    _mockViewController.Object.WhenToldTo(vc => vc.Location).Return(_location);
                    _mockViewController.Object.WhenToldTo(vc => vc.Date).Return(_date);
                    _mockViewController.Object.WhenToldTo(vc => vc.Public).Return(_public);
                    _stateManager.WhenToldTo(sm=>sm.CreateEvent(Moq.It.IsAny<Event>())).Return(_serverEvent);
                };
            Because of = () => _mockViewController.Raise(vc=>vc.Create+=null,(EventArgs)null);
            It should_send_correct_event_to_server = () => _stateManager.WasToldTo(server=>server.CreateEvent(new Event {
                Name = _name,
                Address = _location.Description,
                Location = _location.Coordinate,
                IsPublic = _public,
                StartDate = _date.Date,
                EndDate = _date.Date.AddDays(1),
            }));

            It should_move_to_the_event_view = () => _mockViewController.Object.WasToldTo(vc => vc.GoToEventDashboard(_serverEvent));
            static string _name = "Event Name";
            static AddressDetails _location = new AddressDetails {
                Coordinate = new Coordinate{Latitude = 1,Longitude = 1},
                Description = "Address description"

            };

            static DateTime _date = DateTime.Now;
            static bool _public = true;
            static Event _serverEvent = new Event();
        }
        public class when_create_pressed_without_specifying_name : CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () =>
            {
                _mockViewController.Object.WhenToldTo(vc => vc.Name).Return(_name);
                _mockViewController.Object.WhenToldTo(vc => vc.Location).Return(_location);
                _mockViewController.Object.WhenToldTo(vc => vc.Date).Return(_date);
                _mockViewController.Object.WhenToldTo(vc => vc.Public).Return(_public);
                _stateManager.WhenToldTo(sm => sm.CreateEvent(Moq.It.IsAny<Event>())).Return(_serverEvent);
            };
            Because of = () => _mockViewController.Raise(vc => vc.Create += null, (EventArgs)null);
            It should_not_call_create_event = () => _stateManager.WasNotToldTo(server => server.CreateEvent(Moq.It.IsAny<Event>()));

            It should_not_move_to_the_event_view = () => _mockViewController.Object.WasNotToldTo(vc => vc.GoToEventDashboard(_serverEvent));
            static string _name;
            static AddressDetails _location = new AddressDetails
            {
                Coordinate = new Coordinate { Latitude = 1, Longitude = 1 },
                Description = "Address description"

            };

            static DateTime _date = DateTime.Today;
            static bool _public = true;
            static Event _serverEvent = new Event();
        }
        public class when_create_pressed_without_location : CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () =>
            {
                _mockViewController.Object.WhenToldTo(vc => vc.Name).Return(_name);
                _mockViewController.Object.WhenToldTo(vc => vc.Location).Return(_location);
                _mockViewController.Object.WhenToldTo(vc => vc.Date).Return(_date);
                _mockViewController.Object.WhenToldTo(vc => vc.Public).Return(_public);
                _stateManager.WhenToldTo(sm => sm.CreateEvent(Moq.It.IsAny<Event>())).Return(_serverEvent);
            };
            Because of = () => _mockViewController.Raise(vc => vc.Create += null, (EventArgs)null);
            It should_not_call_create_event = () => _stateManager.WasNotToldTo(server => server.CreateEvent(Moq.It.IsAny<Event>()));

            It should_not_move_to_the_event_view = () => _mockViewController.Object.WasNotToldTo(vc => vc.GoToEventDashboard(_serverEvent));
            static string _name = "Event Name";
            static AddressDetails _location;

            static DateTime _date = DateTime.Today;
            static bool _public = true;
            static Event _serverEvent = new Event();
        }
        public class when_create_pressed_without_date : CreateEventViewControllerSupervisorSpecification
        {
            Establish context = () =>
            {
                _mockViewController.Object.WhenToldTo(vc => vc.Name).Return(_name);
                _mockViewController.Object.WhenToldTo(vc => vc.Location).Return(_location);
                _mockViewController.Object.WhenToldTo(vc => vc.Date).Return(_date);
                _mockViewController.Object.WhenToldTo(vc => vc.Public).Return(_public);
                _stateManager.WhenToldTo(sm => sm.CreateEvent(Moq.It.IsAny<Event>())).Return(_serverEvent);
            };
            Because of = () => _mockViewController.Raise(vc => vc.Create += null, (EventArgs)null);
            It should_not_call_create_event = () => _stateManager.WasNotToldTo(server => server.CreateEvent(Moq.It.IsAny<Event>()));

            It should_not_move_to_the_event_view = () => _mockViewController.Object.WasNotToldTo(vc => vc.GoToEventDashboard(_serverEvent));
            static string _name = "Event Name";
            static AddressDetails _location = new AddressDetails
            {
                Coordinate = new Coordinate { Latitude = 1, Longitude = 1 },
                Description = "Address description"

            };

            static DateTime _date;
            static bool _public = true;
            static Event _serverEvent = new Event();
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
    // ReSharper disable UnusedAutoPropertyAccessor.Local
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
        public event EventHandler<EventArgs> LocationLookupShown;
        public void SetLocation(Coordinate coordinate)
        {
            
        }

        public event EventHandler<EventArgs> LocationSearch;

        public string LocationSearchText { get; private set; }

        public string Name { get; private set; }
        public AddressDetails Location { get; private set; }
        public DateTime Date { get; private set; }
        public bool Public { get; private set; }
        public event EventHandler<EventArgs> Create;

        public void AddAddressesToLocationMap(IEnumerable<AddressDetails> addresses)
        {
            
        }

        public void GoToEventDashboard(Event serverEvent)
        {
            
        }
    }
}