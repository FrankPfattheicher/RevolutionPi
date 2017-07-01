using System;

namespace IctBaden.RevolutionPi
{
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