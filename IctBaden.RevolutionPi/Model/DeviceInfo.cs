using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace IctBaden.RevolutionPi
{
    [DebuggerDisplay("{Name}")]
    public class DeviceInfo
    {
        public string CatalogNr { get; set; }
        public Guid Guid { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public int ProductType { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string Bmk { get; set; }
        public int InpVariant { get; set; }
        public int OutVariant { get; set; }
        public string Comment { get; set; }
        public int Offset { get; set; }
        public JObject Inp;
        public JObject Out;
        public JObject Mem;

        public VariableInfo[] Inputs => Inp?.Children()
                    .Select(token => new VariableInfo(int.Parse(token.First().Path), token.First.Children().ToList()))
                    .ToArray();

        public VariableInfo[] Outputs => Out?.Children()
                    .Select(token => new VariableInfo(int.Parse(token.First().Path), token.First.Children().ToList()))
                    .ToArray();

    }

}
