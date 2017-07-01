using System.Runtime.InteropServices;

namespace IctBaden.RevolutionPi.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpiVariable
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public string VarName;             // Variable name
        public ushort Address;             // Address of the byte in the process image
        public byte BitOffset;           // 0-7 bit position, >= 8 whole byte
        public ushort Length;              // length of the variable in bits. Possible values are 1, 8, 16 and 32
    }
}
