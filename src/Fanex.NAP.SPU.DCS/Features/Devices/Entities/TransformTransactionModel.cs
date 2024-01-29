namespace Fanex.NAP.SPU.DCS.Features.Devices.Entities
{
    public class TransformTransactionModel : IdentityDevice
    {
        public ulong? FPDeviceID { get; set; } = null;

        public int? RecoverType { get; set; } = null;
    }
}
