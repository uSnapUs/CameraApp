using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface ILandingPageViewController : IBaseViewController
    {
        event EventHandler<EventArgs> FindButtonPressed;

    }
}