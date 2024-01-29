namespace Fanex.NAP.SPU.DCS.Features.Devices.Entities
{
    public class IdentityDevice
    {
        public ulong TransID { get; set; }

        public ulong DeviceID { get; set; }

        public ulong DeviceCodeID { get; set; }

        public string FirstDeviceCode { get; set; }

        public short DeviceStatus { get; set; }

        public ulong DeviceFingerprintID { get; set; }
    }
}
