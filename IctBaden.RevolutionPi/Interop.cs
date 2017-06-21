using System.Runtime.InteropServices;

namespace IctBaden.RevolutionPi
{
    internal class Interop
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private const int O_RDONLY = 00;
        private const int O_WRONLY = 01;
        internal const int O_RDWR = 02;

        internal const int SEEK_SET = 0;
        private const int SEEK_CUR = 1;
        private const int SEEK_END = 2;

        [DllImport("libc", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int open(string fileName, int mode);

        [DllImport("libc", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int close(int file);

        [DllImport("libc", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern long lseek(int file, int offset, int whence);

        [DllImport("libc", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int read(int file, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count);
        [DllImport("libc", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int write(int file, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int count);


        // see ioctl.h
        private const uint IOCPARM_MASK = 0x1fff;		/* parameter length, at most 13 bits */

        private const uint IOC_VOID = 0x20000000;   /* no parameters */
        private const uint IOC_OUT = 0x40000000;    /* copy out parameters */
        private const uint IOC_IN = 0x80000000; /* copy in parameters */
        private const uint IOC_INOUT = (IOC_IN | IOC_OUT);
        private const uint IOC_DIRMASK = 0xe0000000;    /* mask for IN/OUT/VOID */

        private static uint _IOC(uint inout, uint group, uint num, uint len) =>
            (inout | ((len & IOCPARM_MASK) << 16) | ((group) << 8) | (num));
        private static uint _IO(uint g, uint n) => _IOC(IOC_VOID, (g), (n), 0);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int ioctl_void(int file, uint cmd);
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int ioctl_data(int file, uint cmd, ref byte[] data);


        // piControl.h
        private const uint KB_IOC_MAGIC = 'K';
        internal static readonly uint KB_RESET = _IO(KB_IOC_MAGIC, 12);  // reset the piControl driver including the config file
        private static uint KB_GET_DEVICE_INFO_LIST = _IO(KB_IOC_MAGIC, 13); // get the device info of all detected devices
        private static uint KB_GET_DEVICE_INFO = _IO(KB_IOC_MAGIC, 14);  // get the device info of one device
        private static uint KB_GET_VALUE = _IO(KB_IOC_MAGIC, 15);  // get the value of one bit in the process image
        private static uint KB_SET_VALUE = _IO(KB_IOC_MAGIC, 16);  // set the value of one bit in the process image
        private static uint KB_FIND_VARIABLE = _IO(KB_IOC_MAGIC, 17);  // find a varible defined in piCtory
        private static uint KB_SET_EXPORTED_OUTPUTS = _IO(KB_IOC_MAGIC, 18);  // copy the exported outputs from a application process image to the real process image
        private static uint KB_UPDATE_DEVICE_FIRMWARE = _IO(KB_IOC_MAGIC, 19);  // try to update the firmware of connected devices
        private static uint KB_DIO_RESET_COUNTER = _IO(KB_IOC_MAGIC, 20);  // set a counter or endocder to 0
        private static uint KB_GET_LAST_MESSAGE = _IO(KB_IOC_MAGIC, 21);  // copy the last error message

        private static uint KB_WAIT_FOR_EVENT = _IO(KB_IOC_MAGIC, 50);  // wait for an event. This call is normally blocking
        private const uint KB_EVENT_RESET = 1;		// piControl was reset, reload configuration

        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming
    }
}
