using System;

namespace Fanex.NAP.SPU.DCS.JobService.Models.Dto
{
    public class TransactionInitialDataDto
    {
        public ulong TransID { get; set; }

        public string CountryCode { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string ISP { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
