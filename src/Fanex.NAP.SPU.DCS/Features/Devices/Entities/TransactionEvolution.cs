namespace Fanex.NAP.SPU.DCS.Features.Devices.Entities
{
    public class TransactionEvolution : TransactionEvolutionAttributes
    {
        public ulong TransID { get; set; }

        public string RawFingerPrintIDs { get; set; }
    }
}
