using System;
using System.Threading;
using IctBaden.RevolutionPi;

namespace PiTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("PiTest");

            var control = new PiControl();
            control.Open();
            Console.WriteLine($"OpenControl = {control.IsOpen}");
            control.SystemLedA1 = LedColor.Off;
            control.SystemLedA2 = LedColor.Off;

            var config = new PiConfiguration();
            config.Open();
            Console.WriteLine($"OpenConfiguration = {config.IsOpen}");

            Console.WriteLine("");

            foreach (var device in config.Devices)
            {
                Console.WriteLine($"Device {device.Name}");
                foreach (var variable in device.Inputs)
                {
                    Console.WriteLine($"  Input {variable.Name}");
                }
                foreach (var variable in device.Outputs)
                {
                    Console.WriteLine($"  Output {variable.Name}");
                }
            }

            var status = control.Read(0, 1);
            Console.WriteLine($"System state = {status[0]}");

            Console.WriteLine("");

            control.SystemLedA1 = LedColor.Red;
            Console.WriteLine("Hit ENTER to start LED test");
            Console.ReadLine();

            Console.WriteLine("LED Test");
            control.SystemLedA1 = LedColor.Green;
            for (var led = 0; led < 10; led++)
            {
                control.SystemLedA2 = LedColor.Green;
                Thread.Sleep(300);
                control.SystemLedA2 = LedColor.Red;
                Thread.Sleep(300);
                control.SystemLedA2 = LedColor.Orange;
                Thread.Sleep(300);
            }
            control.SystemLedA1 = LedColor.Off;
            control.SystemLedA2 = LedColor.Off;

            Console.WriteLine("Done.");
            control.Close();
        }
    }
}
