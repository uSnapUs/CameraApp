using System;
using Camera.ViewControllers.Interfaces;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    public abstract class BaseViewController:UIViewController,IBaseViewController
    {

        protected abstract void EnsureSupervised();

        public override void ViewDidLoad()
        {
            EnsureSupervised();
            base.ViewDidLoad();
            OnLoad();
        }
        public override void ViewDidAppear(bool animated)
        {
            EnsureSupervised();
            base.ViewDidAppear(animated);
            OnAppear();
        }
        public override void ViewDidDisappear(bool animated)
        {
           // OnUnload();
            base.ViewDidDisappear(animated);
        }
        public event EventHandler<EventArgs> Load;

        protected virtual void OnLoad()
        {
            EventHandler<EventArgs> handler = Load;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Unload;

        protected virtual void OnUnload()
        {
            EventHandler<EventArgs> handler = Unload;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Appear;

        protected virtual void OnAppear()
        {
            EventHandler<EventArgs> handler = Appear;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> BeforeAppear;

        protected virtual void OnBeforeAppear()
        {
            EventHandler<EventArgs> handler = BeforeAppear;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        protected override void Dispose(bool disposing)
        {
            OnUnload();
            base.Dispose(disposing);
        }
    }
}