namespace VariableServer.Model
{
    public class VarReadInfo
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        public string LengthText { get; set; }
        public ushort Length { get; set; }
        /// <summary>
        /// Variable address within containing device
        /// </summary>
        public ushort Address { get; set; }
        public VarDeviceInfo Device { get; set; }
        public int[] Data { get; set; }
        public object Value { get; set; }
    }
}