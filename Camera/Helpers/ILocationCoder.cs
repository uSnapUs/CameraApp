using System;

namespace Camera.Helpers
{
    public interface ILocationCoder
    {
        void LookupLocation(string address,Action<ILocation[]> onComplete);
    }
}