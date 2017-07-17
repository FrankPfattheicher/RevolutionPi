using System;
using System.Diagnostics;
using System.Threading;
using IctBaden.RevolutionPi;
using IctBaden.RevolutionPi.Configuration;
using Microsoft.Owin.Hosting;

namespace VariableServer
{
    internal class Program
    {
        internal static PiControl Control;
        internal static PiConfiguration Config;

        private static void Main(string[] args)
        {
            Control = new PiControl();
            Control.Open();
            Config = new PiConfiguration();
            Config.Open();

            var hostUrl = args.Length > 1 ? args[1] : "http://*:8000";
            var server = WebApp.Start<Startup>(hostUrl);
            Trace.TraceInformation($"RevPi Variable Server running on {hostUrl}");

            var terminate = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) => { terminate.Set(); };
            terminate.WaitOne();

            Trace.TraceInformation("Terminating RevPi Variable Server");
            server.Dispose();
        }
    }
}
