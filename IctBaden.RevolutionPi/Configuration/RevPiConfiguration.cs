using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IctBaden.RevolutionPi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IctBaden.RevolutionPi.Configuration
{
    public class PiConfiguration
    {
        public string RevPiConfigFileName = "/etc/revpi/config.rsc";

        private JObject _config;

        public bool Open()
        {
            if (IsOpen) return true;
            if (!File.Exists(RevPiConfigFileName)) return false;

            try
            {
                var json = File.ReadAllText(RevPiConfigFileName);
                _config = JsonConvert.DeserializeObject<JObject>(json);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"RevPi.Configuration.Open failed: {ex.Message}");
            }
            return false;
        }

        public bool IsOpen => _config != null;


        private List<DeviceInfo> _devices = new List<DeviceInfo>();

        public List<DeviceInfo> Devices
        {
            get
            {
                if (_devices.Count == 0)
                {
                    Open();
                    try
                    {
                        _devices = _config["Devices"].Children()
                            .Select(jt => jt.ToObject<DeviceInfo>())
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"RevolutionPi.Configuration failed to parse devices: {ex.Message}");
                    }
                }
                return _devices;
            }
        }


        public VariableInfo GetVariable(string name)
        {
            return Devices.SelectMany(d => d.Inputs).FirstOrDefault(v => v.Name == name) ??
                   Devices.SelectMany(d => d.Outputs).FirstOrDefault(v => v.Name == name);
        }

    }
}
