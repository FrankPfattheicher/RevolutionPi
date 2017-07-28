using IctBaden.RevolutionPi.Configuration;

namespace IctBaden.RevolutionPi
{
    /// <summary>
    /// Setting and querying the system LEDs A1 and A2.
    /// </summary>
    public class RevPiLeds
    {
        private readonly PiControl _control;
        private readonly int _ledAddress;

        public RevPiLeds(PiControl control, PiConfiguration config)
        {
            _control = control;

            var info = config.GetVariable("RevPiLED");
            _ledAddress = info?.Address ?? 0x06;
        }

        /// <summary>
        /// Current color of system LED A1
        /// </summary>
        public LedColor SystemLedA1
        {
            get
            {
                var led = _control.Read(_ledAddress, 1);
                return (LedColor)(led[0] & 0x03);
            }
            set
            {
                var oldLed = _control.Read(_ledAddress, 1);
                var newLed = (byte)((oldLed[0] & ~0x03) | (byte)value);
                _control.Write(_ledAddress, new[] { newLed });
            }
        }

        /// <summary>
        /// Current color of system LED A2
        /// </summary>
        public LedColor SystemLedA2
        {
            get
            {
                var led = _control.Read(_ledAddress, 1);
                return (LedColor)((led[0] & 0x0C) >> 2);
            }
            set
            {
                var oldLed = _control.Read(_ledAddress, 1);
                var newLed = (byte)((oldLed[0] & ~0x0C) | ((byte)value << 2));
                _control.Write(_ledAddress, new[] { newLed });
            }
        }

    }
}