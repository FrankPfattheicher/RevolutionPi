using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace IctBaden.RevolutionPi
{
    [DebuggerDisplay("{Name}")]
    public class VariableInfo
    {
        public int Index { get; set; }
        public string  Name { get; set; }
        public long Value { get; set; }
        public byte BitOffset { get; set; }              // 0-7 bit position, >= 8 whole byte
        public ushort Length { get; set; }               // length of the variable in bits. Possible values are 1, 8, 16 and 32
        public ushort Address { get; set; }              // Address of the byte in the process image
        public bool Export { get; set; }
        //  "0000",
        public string Comment { get; set; }
        //  ""

        public VariableInfo(int index, IList<JToken> json)
        {
            Index = index;
            Name = json[0].Value<string>();
            Value = json[1].Value<long>();
            Length = json[2].Value<ushort>();
            Address = json[3].Value<ushort>();
            Export = json[4].Value<bool>();

            Comment = json[6].Value<string>();
        }
    }
}
