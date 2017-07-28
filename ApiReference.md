# API Reference
*RevolutionPi .NET Library*

For more information see http://www.mono-project.com/.

#
## class PiConfiguration
This class loads the RevPi configuration file used by the device driver.

    RevPiConfigFileName = "/etc/revpi/config.rsc";

#### bool Open()
Opens the configuration file (RevPiConfigFileName).    
Returns True if file could be opened and parsed.

#### bool IsOpen
Property signalling configuration is loaded.

#### List&lt;DeviceInfo&gt; Devices
List of loaded device informations.

#### VariableInfo GetVariable(string name)
Retrieve information about a configured variable by iot's name.
Returns variable info for the given variable or null if not found.

#
## class PiControl
This ist the interface to the Linux device driver (PiControlDeviceName).

    PiControlDeviceName = "/dev/piControl0";

#### bool Open()
Opens the driver connection.     
Returns True if connection successfully opened.

#### bool IsOpen
True if connection to the device driver established

#### void Close()
Closes the driver connection.

#### bool Reset()
Resets the piControl driver process.
Returns True if reset is successful.

#### byte[] Read(int offset, int length)
Read data from the process image.    

    offset : Position to read from    
    length : Byte count to read

Returns Data read or null in case of failure.

#### int Write(int offset, byte[] data)
Write data to the process image.

    offset : Position to write to
    data   : Data to be written

Bytes written.

#### bool GetBitValue(ushort address, byte bit)
Get the value of one bit in the process image.

    address : Address of the byte in the process image
    bit     : bit position (0-7)

Returns the bit value.

#### void SetBitValue(ushort address, byte bit, bool value)
Set the value of one bit in the process image.

    address : Address of the byte in the process image
    bit     : bit position (0-7)
    value   : Bit value to be written    


#### object ConvertDataToValue(byte[] data, int length)
Converts given data according it's length to
* bool   for length = 1
* byte   for length = 8
* ushort for length = 16

    data   : Source data
    length : Length of information

Returns value of data.

#
## class RevPiLeds
Class to simplify setting and querying the system LEDs A1 and A2.

#### Constructor RevPiLeds(PiControl control, PiConfiguration config)
Constructs an instance of the class. Requires control and configuration access.

#### LedColor SystemLedA1
#### LedColor SystemLedA2
Properties allowing querying and setting the color of the LEDs.

#### enum LedColor
Possible LED colors for system LEDS A1 and A2 are one of the following.

        Off
        Green
        Red
        Orange


#
## enum RevPiStatus
Bit flags of the system status byte 'RevPiStatus'

        Running = 0x01,
        ExtraModule = 0x02,
        MissingModule = 0x04,
        SizeMismatch = 0x08,
        LeftGateway = 0x10,
        RightGateway = 0x20

