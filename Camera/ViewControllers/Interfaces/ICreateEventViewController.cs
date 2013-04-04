using System;
using System.Collections.Generic;
using Camera.Helpers;
using Camera.Model;

namespace Camera.ViewControllers.Interfaces
{
    public interface ICreateEventViewController:IBaseViewController
    {
        void PresentLandingView();
        event EventHandler<EventArgs> BackPressed;
        event EventHandler<EventArgs> LocationLookupShown;
        void SetLocation(Coordinate coordinate);
        event EventHandler<EventArgs> LocationSearch;
        string LocationSearchText { get; }
        string Name { get; }
        AddressDetails Location { get; }
        DateTime Date { get; }
        bool Public { get; }
        event EventHandler<EventArgs> Create;
        void AddAddressesToLocationMap(IEnumerable<AddressDetails> addresses);
        void GoToEventDashboard(Event serverEvent);
    }
}