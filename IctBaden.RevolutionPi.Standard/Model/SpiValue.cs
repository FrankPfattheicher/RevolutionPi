using System.Runtime.InteropServices;

namespace IctBaden.RevolutionPi.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public class SpiValue
    {
        public ushort Address;           // Address of the byte in the process image
        public byte Bit;                 // 0-7 bit position, >= 8 whole byte
        public byte Value;               // Value: 0/1 for bit access, whole byte otherwise
    }
}
