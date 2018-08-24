using System.Collections.Generic;

namespace IctBaden.RevolutionPi.Model
{
    /// <summary>
    /// List of known product types.
    /// </summary>
    public class RevPiProductNames
    {
        public static Dictionary<int, string> KnownProducts = new Dictionary<int, string>
        {
            { 95, "RevPi Core" },
            { 96, "RevPi DIO" },
            { 97, "RevPi DI" },
            { 98, "RevPi DO" },
            { 103, "RevPi AIO" },

            { 0x6001, "ModbusTCP Slave Adapter" },
            { 0x6002, "ModbusRTU Slave Adapter" },
            { 0x6003, "ModbusTCP Master Adapter" },
            { 0x6004, "ModbusRTU Master Adapter" },
            { 0x6005, "Profinet Controller" },
            { 0x6006, "Profinet Device" },

            { 100, "Gateway DMX" },
            { 71, "Gateway CANopen" },
            { 73, "Gateway DeviceNet" },
            { 74, "Gateway EtherCAT" },
            { 75, "Gateway EtherNet/IP" },
            { 93, "Gateway ModbusTCP" },
            { 76, "Gateway Powerlink" },
            { 77, "Gateway Profibus" },
            { 79, "Gateway Profinet IRT" },
            { 81, "Gateway SercosIII" }
        };

        /// <summary>
        /// Returns name of the product given its product type.
        /// </summary>
        /// <param name="productType">Product type number</param>
        /// <returns>Name of the product.</returns>
        public static string GetProductName(int productType)
        {
            if (KnownProducts.ContainsKey(productType))
            {
                return KnownProducts[productType];
            }

            return $"Unknown roduct Type ({productType})";
        }
    }
}
