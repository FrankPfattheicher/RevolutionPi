using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using IctBaden.RevolutionPi;
using IctBaden.RevolutionPi.Configuration;
using IctBaden.RevolutionPi.Model;

namespace PiTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0 || args.Any(a => new Regex(@"^-[\?hH]$").IsMatch(a)))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                Console.WriteLine($"PiTest V{fileVersionInfo.FileVersion}");
                Console.WriteLine(" -s               Display system state");
                Console.WriteLine(" -d               Get device list");
                Console.WriteLine(" -v <varName>     Get variable info");
                Console.WriteLine(" -r <varName>     Read variable");
                Console.WriteLine(" -y <led> <color> Set system LED (1/2) to color");
                Console.WriteLine("                    r - red, g - green, o - orange, x - off");
                Environment.Exit(0);
            }

            var control = new PiControl();
            if (!control.Open())
            {
                Console.WriteLine("Could not open PiControl.");
                Environment.Exit(1);
            }

            var config = new PiConfiguration();
            if (!config.Open())
            {
                Console.WriteLine("Could not open PiConfiguration.");
                Environment.Exit(2);
            }

            var leds = new RevPiLeds(control, config);

            string name = (args.Length >= 2) ? args[1] : string.Empty;
            switch (args[0])
            {
                case "-x":
                    Console.WriteLine("Resetting driver");
                    control.Reset();
                    break;
                case "-d":
                    ListDevices(config);
                    break;
                case "-s":
                    ShowSystemState(config, control);
                    break;
                case "-v":
                    if (string.IsNullOrEmpty(name))
                    {
                        Console.WriteLine("Missing parameter <varName>.");
                        Environment.Exit(3);
                    }
                    ShowVarInfo(config, name);
                    break;
                case "-r":
                    if (string.IsNullOrEmpty(name))
                    {
                        Console.WriteLine("Missing parameter <varName>.");
                        Environment.Exit(3);
                    }
                    ReadVarValue(config, control, name);
                    break;
                case "-y":
                    int led;
                    int.TryParse((args.Length >= 2) ? args[1] : string.Empty, out led);
                    var color = (args.Length >= 3) ? args[2] : string.Empty;
                    if ((led < 1) || (led > 2) || string.IsNullOrEmpty(color))
                    {
                        Console.WriteLine("Missing parameter(s) <led> <color>.");
                        Environment.Exit(3);
                    }
                    SetLed(leds, led, color);
                    break;
            }

            control.Close();
            Environment.Exit(0);
        }

        private static void ShowSystemState(PiConfiguration config, PiControl control)
        {
            var variableInfo = config.GetVariable("RevPiStatus");
            var data = control.Read(variableInfo.Address, 1) ?? new byte[] { 0 };
            var status = (int)data[0];
            Console.Write($"RevPiStatus=0x{status:X2} ");
            foreach (int value in Enum.GetValues(typeof(RevPiStatus)))
            {
                if ((status & value) != 0)
                {
                    Console.Write($" {(RevPiStatus)value}");
                }
            }
            Console.WriteLine();
        }

        private static void SetLed(RevPiLeds leds, int led, string color)
        {
            LedColor ledColor;
            switch (color.Substring(0, 1).ToLower())
            {
                case "r":
                    ledColor = LedColor.Red;
                    break;
                case "g":
                    ledColor = LedColor.Green;
                    break;
                case "o":
                    ledColor = LedColor.Orange;
                    break;
                default:
                    ledColor = LedColor.Off;
                    break;
            }
            if (led == 1)
            {
                leds.SystemLedA1 = ledColor;
            }
            else
            {
                leds.SystemLedA2 = ledColor;
            }
        }

        private static void ShowVarInfo(PiConfiguration config, string name)
        {
            foreach (var device in config.Devices)
            {
                var varInfo = device.Variables.FirstOrDefault(v => v.Name == name);
                if (varInfo != null)
                {
                    Console.WriteLine($"{varInfo.Type.ToString().Substring(0, 1)}[{varInfo.Address:D4}]  {varInfo.Name} : {varInfo.LengthText}");
                    Console.WriteLine($"  @Device {device.Name}  [{device.Type}]");
                }
            }
        }

        private static void ReadVarValue(PiConfiguration config, PiControl control, string name)
        {
            var varInfo = config.Devices
                .SelectMany(d => d.Variables)
                .FirstOrDefault(v => v.Name == name);

            if (varInfo == null) return;

            var data = control.Read(varInfo.Address, varInfo.Length);
            if (data == null) return;

            var value = control.ConvertDataToValue(data, varInfo.Length);
            Console.WriteLine($"{varInfo.LengthText} {varInfo.Name} = {value} = 0x{value:X}");
        }

        private static void ListDevices(PiConfiguration config)
        {
            foreach (var device in config.Devices)
            {
                Console.WriteLine($"Device {device.Name}  [{device.Type}]");
                Console.WriteLine($"  Address: {device.Offset}, Type: {device.ProductType} (0x{device.ProductType:X2}) {RevPiProductNames.GetProductName(device.ProductType)}");
                foreach (var variable in device.Inputs)
                {
                    Console.WriteLine($"    I[{variable.Address:D4}]  {variable.Name} : {variable.LengthText}");
                }
                foreach (var variable in device.Outputs)
                {
                    Console.WriteLine($"    Q[{variable.Address:D4}]  {variable.Name} : {variable.LengthText}");
                }
                foreach (var variable in device.Mems)
                {
                    Console.WriteLine($"    M[{variable.Address:D4}]  {variable.Name} : {variable.LengthText}");
                }
            }
        }




    }
}
