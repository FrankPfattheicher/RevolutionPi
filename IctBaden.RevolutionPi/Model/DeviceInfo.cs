using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IctBaden.RevolutionPi.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
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

// never assigned
#pragma warning  disable 0649
        [JsonProperty("inp")]
        private JObject _inp;
        [JsonProperty("out")]
        private JObject _out;
        [JsonProperty("mem")]
        private JObject _mem;
        [JsonProperty("extend")]
        private JObject _ext;

        private VariableInfo[] GetVarInfos(JToken obj)
        {
            return obj?.Children()
                .Select(token => new VariableInfo(this, VariableType.Input, int.Parse(token.First().Path), token.First.Children().ToList()))
                .ToArray()
                ?? new VariableInfo[0];
        }

        public VariableInfo[] Inputs => GetVarInfos(_inp);
        public VariableInfo[] Outputs => GetVarInfos(_out);
        public VariableInfo[] Mems => GetVarInfos(_mem);
        public VariableInfo[] Extends => GetVarInfos(_ext);

        public IEnumerable<VariableInfo> Variables
        {
            get
            {
                foreach (var variableInfo in Inputs)
                {
                    yield return variableInfo;
                }
                foreach (var variableInfo in Outputs)
                {
                    yield return variableInfo;
                }
                foreach (var variableInfo in Mems)
                {
                    yield return variableInfo;
                }
                foreach (var variableInfo in Extends)
                {
                    yield return variableInfo;
                }
            }
        }

    }

}
