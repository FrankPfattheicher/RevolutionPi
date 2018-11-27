using System;
// ReSharper disable UnusedMember.Global

namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Bit flags of the system status byte 'RevPiStatus'
    /// </summary>
    [Flags]
    public enum RevPiStatus
    {
        Running = 0x01,
        ExtraModule = 0x02,
        MissingModule = 0x04,
        SizeMismatch = 0x08,
        LeftGateway = 0x10,
        RightGateway = 0x20
    }
}