using System;
using Mindscape.Raygun4Net;
using MonoTouch.UIKit;

namespace Camera
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception)
                    new RaygunClient("GRGMdF+o8A+aqw8Y6vcalg==").Send(e.ExceptionObject as Exception);
            };
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}