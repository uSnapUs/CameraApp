using System;

namespace Camera.Helpers
{
    public class ConsoleLogger : ILogger
    {

        public void Trace(string message)
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine("{0} from {1} in {2}", message, method.Name, method.DeclaringType.Name);
        }
        public void Exception(Exception e)
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine("Exception '{0}' from {1} in {2}", e.Message, method.Name, method.DeclaringType.Name);
        }
    }
    public class RaygunLogger : ILogger
    {
        //readonly RaygunClient _client = new RaygunClient("3mYw/8cjL9ZOq04NeW0RCg==");
        public void Trace(string message)
        {
        }

        public void Exception(Exception e)
        {
            //_client.SendInBackground(e);
        }
    }
    public class NullLogger : ILogger
    {
        public void Trace(string message)
        {

        }

        public void Exception(Exception e)
        {

        }
    }
}