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

        /// <summary>
        /// Opens the configuration file (RevPiConfigFileName)
        /// </summary>
        /// <returns>True if file could be opened and parsed</returns>
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

        /// <summary>
        /// Configuration is loaded.
        /// </summary>
        public bool IsOpen => _config != null;


        private List<DeviceInfo> _devices = new List<DeviceInfo>();

        /// <summary>
        /// List of loaded device informations.
        /// </summary>
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

        /// <summary>
        /// Retrieve information about a configured variable by iot's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Variable info for the given variable or null if not found.</returns>
        public VariableInfo GetVariable(string name)
        {
            return Devices.SelectMany(d => d.Inputs).FirstOrDefault(v => v.Name == name) ??
                   Devices.SelectMany(d => d.Outputs).FirstOrDefault(v => v.Name == name);
        }

    }
}
