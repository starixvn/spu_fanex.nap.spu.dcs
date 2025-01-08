namespace Fanex.NAP.SPU.DCS.Features.MBDevice.Entities
{
    public class MBTransactionEntity
    {
        public ulong TransID { get; set; }

        public ulong SameDeviceID { get; set; }

        public int RecoverType { get; set; }

        public int BatchID { get; set; }
    }
}
