using System;
using Camera.ViewControllers.Interfaces;

namespace Camera.Supervisors
{
    public abstract class BaseViewControllerSupervisor:IDisposable
    {
        IBaseViewController _viewController;

        protected BaseViewControllerSupervisor(IBaseViewController viewController)
        {
            _viewController = viewController;
            _viewController.Appear += ViewControllerAppear;
            _viewController.BeforeAppear += ViewControllerBeforeAppear;
            _viewController.Load += ViewControllerLoad;
            _viewController.Unload += ViewControllerUnload;
        }

        protected void ViewControllerUnload(object sender, EventArgs e)
        {
            this.Dispose();
        }

        protected void ViewControllerLoad(object sender, EventArgs e)
        {
            
        }

        protected void ViewControllerBeforeAppear(object sender, EventArgs e)
        {
            
        }

        protected void ViewControllerAppear(object sender, EventArgs e)
        {
                
        }

        protected virtual void UnwireEvents()
        {
            _viewController.Appear -= ViewControllerAppear;
            _viewController.BeforeAppear -= ViewControllerBeforeAppear;
            _viewController.Load -= ViewControllerLoad;
            _viewController.Unload -= ViewControllerUnload;
        }


        public virtual void Dispose()
        {
            UnwireEvents();
            _viewController = null;
        }
    }
}