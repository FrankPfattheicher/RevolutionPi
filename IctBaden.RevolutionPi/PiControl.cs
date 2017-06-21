using System;
using System.Runtime.InteropServices;

namespace IctBaden.RevolutionPi
{
    public class PiControl
    {
        /// <summary>
        /// Linux device name full path
        /// </summary>
        public string PiControlDeviceName = "/dev/piControl0";

        private int piControlHandle = -1;

        public bool IsOpen => piControlHandle >= 0;

        public bool Open()
        {
            if (!IsOpen)
            {
                piControlHandle = Interop.open(PiControlDeviceName, Interop.O_RDWR);
            }
            return IsOpen;
        }

        public void Close()
        {
            if (!IsOpen) return;

            Interop.close(piControlHandle);
            piControlHandle = -1;
        }

        public bool Reset()
        {
            if (!Open()) return false;

            return Interop.ioctl_void(piControlHandle, Interop.KB_RESET) >= 0;
        }

        public byte[] Read(int offset, int length)
        {
            if (!IsOpen) return null;

            if (Interop.lseek(piControlHandle, offset, Interop.SEEK_SET) < 0)
            {
                return null;
            }

            var data = new byte[length];
            var bytesRead = Interop.read(piControlHandle, data, length);
            return bytesRead != length ? null : data;
        }

        public int Write(int offset, byte[] data)
        {
            if (!IsOpen) return 0;

            if (Interop.lseek(piControlHandle, offset, Interop.SEEK_SET) < 0)
            {
                return 0;
            }

            var bytesWritten = Interop.write(piControlHandle, data, data.Length);
            return bytesWritten;
        }


        public SpiVariable GetVariable(string name)
        {
            var variable = new SpiVariable();

            return variable;
        }

        public LedColor SystemLedA1 
        {
            get
            {
                var led = Read(6, 1);
                return (LedColor)(led[0] & 0x03);
            }
            set
            {
                var led = Read(6, 1);

                var data = (byte)((led[0] & ~0x03) | (byte)value);
                Write(6, new[] { data });
            }
        }

        public LedColor SystemLedA2
        {
            get
            {
                var led = Read(6, 1);
                return (LedColor)((led[0] & 0x0C) >> 2);
            }
            set
            {
                var led = Read(6, 1);

                var data = (byte)((led[0] & ~0x0C) | ((byte)value << 2));
                Write(6, new[] { data });
            }
        }
    }
}
