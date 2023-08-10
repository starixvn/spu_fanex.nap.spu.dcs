using System;

namespace Fanex.NAP.SPU.DCS.JobService.Models.Dto
{
    public class TransactionInitialDataDto
    {
        public ulong TransID { get; set; }

        public string CountryCode { get; set; }

        public string RegionName { get; set; }

        public string CityName { get; set; }

        public string ISPName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
