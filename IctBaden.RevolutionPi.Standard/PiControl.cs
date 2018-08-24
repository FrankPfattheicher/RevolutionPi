using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using IctBaden.RevolutionPi.Model;

namespace IctBaden.RevolutionPi
{
    public class PiControl
    {
        /// <summary>
        /// Linux device name full path
        /// </summary>
        public string PiControlDeviceName = "/dev/piControl0";

        private FileStream _piControlFile;
        private int PiControlHandle => _piControlFile?.SafeFileHandle?.DangerousGetHandle().ToInt32() ?? 0;

        /// <summary>
        /// Opens the driver connection.
        /// </summary>
        /// <returns>True if connection successfully opened</returns>
        public bool Open()
        {
            if (IsOpen) return true;

            _piControlFile = File.Open(PiControlDeviceName, FileMode.Open, FileAccess.ReadWrite);
            if (IsOpen)
            {
                Trace.TraceInformation($"PiControl: Using {PiControlDeviceName}");
            }
            else
            {
                var err = Marshal.GetLastWin32Error();
                Trace.TraceError($"PiControl.Open: open {PiControlDeviceName} failed: errno = {err}, handle = {PiControlHandle}");
            }
            return IsOpen;
        }

        /// <summary>
        /// True if connection to the device driver established
        /// </summary>
        public bool IsOpen => _piControlFile != null;

        /// <summary>
        /// Closes the driver connection.
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;

            _piControlFile.Dispose();
            _piControlFile = null;
        }

        /// <summary>
        /// Resets the piControl driver process.
        /// </summary>
        /// <returns>True if reset is successful</returns>
        public bool Reset()
        {
            if (!Open()) return false;

            return Interop.ioctl_void(PiControlHandle, Interop.KB_RESET) >= 0;
        }

        /// <summary>
        /// Read data from the process image.
        /// </summary>
        /// <param name="offset">Position to read from</param>
        /// <param name="length">Byte count to read</param>
        /// <returns>Data read or null in case of failure</returns>
        public byte[] Read(int offset, int length)
        {
            if (!IsOpen) return null;

            try
            {
                _piControlFile.Seek(offset, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"PiControl.Read: Seek failed: {ex.Message}");
                return null;
            }

            var data = new byte[length];
            var bytesRead = _piControlFile.Read(data, 0, length);
            if (bytesRead < 0)
            {
                var err = Marshal.GetLastWin32Error();
                Trace.TraceError($"PiControl.Read: read 0x{offset:X2} failed: errno = {err}");
            }
            return bytesRead != length ? null : data;
        }

        /// <summary>
        /// Write data to the process image.
        /// </summary>
        /// <param name="offset">Position to write to</param>
        /// <param name="data">Data to be written</param>
        /// <returns>Bytes written</returns>
        public int Write(int offset, byte[] data)
        {
            if (!IsOpen) return 0;

            try
            {
                _piControlFile.Seek(offset, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"PiControl.Write: Seek failed: {ex.Message}");
                return 0;
            }

            _piControlFile.Write(data, 0, data.Length);
            _piControlFile.Flush();
            return data.Length;
        }

        /// <summary>
        /// Get the value of one bit in the process image.
        /// </summary>
        /// <param name="address">Address of the byte in the process image</param>
        /// <param name="bit">bit position (0-7)</param>
        /// <returns>Bit value</returns>
        public bool GetBitValue(ushort address, byte bit)
        {
            var bitValue = new SpiValue
            {
                Address = address,
                Bit = bit
            };

            if (!IsOpen) return false;

            if (Interop.ioctl_value(PiControlHandle, Interop.KB_GET_VALUE, bitValue) < 0)
            {
                Trace.TraceError("PiControl.SetBitValue: Failed to read bit value.");
                return false;
            }

            return bitValue.Value != 0;
        }

        /// <summary>
        /// Set the value of one bit in the process image.
        /// </summary>
        /// <param name="address">Address of the byte in the process image</param>
        /// <param name="bit">bit position (0-7)</param>
        /// <param name="value"></param>
        // ReSharper disable once UnusedMember.Global
        public void SetBitValue(ushort address, byte bit, bool value)
        {
            var bitValue = new SpiValue
            {
                Address = address,
                Bit = bit,
                Value = (byte)(value ? 1 : 0)
            };

            if (!IsOpen) return;

            if (Interop.ioctl_value(PiControlHandle, Interop.KB_SET_VALUE, bitValue) < 0)
            {
                Trace.TraceError("PiControl.SetBitValue: Failed to write bit value.");
            }
        }

        /// <summary>
        /// Converts given data to value
        /// </summary>
        /// <param name="data">Source data</param>
        /// <returns>Value of data</returns>
        public object ConvertDataToValue(byte[] data)
        {
            switch (data.Length)
            {
                case 8:
                    return data[0];
                case 16:
                    return (ushort)(data[0] + (data[1] * 0x100));
                case 32:
                    return data[0] +
                           (ulong)(data[1] * 0x100) +
                           (ulong)(data[2] * 0x10000) +
                           (ulong)(data[3] * 0x1000000);
                default:
                    return Encoding.ASCII.GetString(data);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public VarData ReadVariable(VariableInfo varInfo)
        {
            var deviceOffset = varInfo.Device.Offset;
            int byteLen;

            switch (varInfo.Length)
            {
                case 1: byteLen = 0; break;        // Bit
                case 8: byteLen = 1; break;
                case 16: byteLen = 2; break;
                case 32: byteLen = 4; break;
                default:                            // strings, z.B. IP-Adresse
                    byteLen = -varInfo.Length / 8;
                    break;
            }

            var varData = new VarData();

            if (byteLen > 0)
            {
                varData.Raw = Read(deviceOffset + varInfo.Address, byteLen);
            }
            else if (byteLen == 0)
            {
                var address = (ushort)(deviceOffset + varInfo.Address);
                varData.Raw = new[]
                {
                    (byte) (GetBitValue(address, varInfo.BitOffset) ? 1 : 0)
                };
            }
            else  // iByteLen < 0
            {
                varData.Raw = Read(deviceOffset + varInfo.Address, -byteLen);
            }

            if (varData.Raw == null) return null;

            varData.Value = ConvertDataToValue(varData.Raw);
            return varData;
        }
    }
}
