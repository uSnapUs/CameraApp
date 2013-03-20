using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface ILandingPageViewController : IBaseViewController
    {
        event EventHandler<EventArgs> FindButtonPressed;
        event EventHandler<EventArgs> CreateButtonPressed;
        event EventHandler<EventArgs> MyEventsButtonPressed;
        event EventHandler<EventArgs> BackButtonPressed;
        event EventHandler<EventArgs> FindNearbyButtonPressed;
        event EventHandler<EventArgs> JoinButtonPressed;
        string EventCode { get; set; }
        void PresentMyEventsView();
        void PresentFindEventsView();
        void PresentCreateView();
        void HideFindEventsView();
        void PresentFindNearbyView();
        void PresentLoginView(string loginReason);
        void ShowValidationMessage(string validationMessage);
        void PresentEventDashboard();
    }
}