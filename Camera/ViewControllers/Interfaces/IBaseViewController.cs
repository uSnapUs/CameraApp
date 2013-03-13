using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface IBaseViewController
    {
        event EventHandler<EventArgs> Load;
        event EventHandler<EventArgs> Unload;
        event EventHandler<EventArgs> Appear;
        event EventHandler<EventArgs> BeforeAppear;
    }
}