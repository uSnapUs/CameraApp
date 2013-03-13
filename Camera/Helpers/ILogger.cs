using System;

namespace Camera.Helpers
{
    public interface ILogger
    {
        void Trace(string message);
        void Exception(Exception e);
    }
}