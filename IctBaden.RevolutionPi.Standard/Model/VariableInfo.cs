using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace IctBaden.RevolutionPi.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class VariableInfo
    {
        public VariableType Type { get; private set; }
        public int Index { get; private set; }
        public string  Name { get; set; }
        public object DefaultValue { get; set; }
        public byte BitOffset { get; set; }              // 0-7 bit position, >= 8 whole byte
        public ushort Length { get; set; }               // length of the variable in bits. Possible values are 1, 8, 16 and 32
        public ushort Address { get; set; }              // Address of the byte in the process image
        public bool Export { get; set; }
        //  "0000",
        public string Unknown { get; set; }               //"0001"
        public string Comment { get; set; }

        public DeviceInfo Device { get; set; }

        public string LengthText
        {
            get
            {
                switch (Length)
                {
                    case 1: return "BIT";
                    case 8: return "BYTE";
                    case 16: return "WORD";
                    case 32: return "DWORD";
                }
                return $"[{Length}bits]";
            }
        }

        public VariableInfo(DeviceInfo device, VariableType type, int index, IList<JToken> json)
        {
            Device = device;
            Type = type;
            Index = index;
            Name = json[0].Value<string>();
            DefaultValue = json[1].Value<object>();
            Length = json[2].Value<ushort>();
            Address = json[3].Value<ushort>();
            Export = json[4].Value<bool>();
            Unknown = json[5].Value<string>();
            Comment = json[6].Value<string>();
            try
            {
                BitOffset = json[7].Value<byte>();
            }
            catch
            {
                BitOffset = 0;
            }
        }
    }
}
