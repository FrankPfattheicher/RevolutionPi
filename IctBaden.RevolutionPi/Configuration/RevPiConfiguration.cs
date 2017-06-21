using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IctBaden.RevolutionPi
{
    public class PiConfiguration
    {
        public string RevPiConfigFileName = "/etc/revpi/config.rsc";

        private JObject config;

        public bool Open()
        {
            if (IsOpen) return true;
            if (!File.Exists(RevPiConfigFileName)) return false;

            try
            {
                var json = File.ReadAllText(RevPiConfigFileName);
                config = JsonConvert.DeserializeObject<JObject>(json);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"RevPi.Configuration.Open failed: {ex.Message}");
            }
            return false;
        }

        public bool IsOpen => config != null;


        private DeviceInfo[] _devices;

        public DeviceInfo[] Devices
        {
            get
            {
                if (_devices == null)
                {
                    Open();
                    try
                    {
                        _devices = config["Devices"].Children()
                            .Select(jt => jt.ToObject<DeviceInfo>())
                            .ToArray();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"RevolutionPi.Configuration failed to parse devices: {ex.Message}");
                    }
                }
                return _devices;
            }
        }

    }
}
