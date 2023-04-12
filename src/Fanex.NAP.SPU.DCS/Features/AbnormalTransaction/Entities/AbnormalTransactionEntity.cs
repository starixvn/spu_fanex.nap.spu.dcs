using System;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities
{
    public class AbnormalTransactionEntity
    {
        public ulong AssociationID { get; set; }

        public ulong TransID { get; set; }

        public string LoginName { get; set; }

        public DateTime TransTime { get; set; }

        public string UserAgent { get; set; }

        public string IP { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string DeviceTypeName { get; set; }
    }
}
