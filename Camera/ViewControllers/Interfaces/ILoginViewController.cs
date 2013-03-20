using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface ILoginViewController:IBaseViewController
    {
        event EventHandler<EventArgs> FacebookLoginPress;
    }
}