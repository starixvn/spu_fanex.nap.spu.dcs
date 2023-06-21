using System;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model
{
    internal class AbnormalTransactionModel
    {
        public ulong TransID { get; set; }

        public string LoginName { get; set; }

        public DateTime TransTime { get; set; }

        public string UserAgent { get; set; }

        public string IP { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string Device { get; set; }
    }
}
