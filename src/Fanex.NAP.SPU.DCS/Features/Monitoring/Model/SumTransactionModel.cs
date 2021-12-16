namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Model
{
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Entities;    
    using System.Collections.Generic;

    public class SumTransactionModel
    {
        public SumTransactionModel()
        {
            SumSubscriberList = new List<SumSubscriberEntity>();
            SumDeviceTypeList = new List<SumDeviceTypeEntity>();
            AccountIPList = new List<AccountIPEntity>();
            AccountDeviceList = new List<AccountDeviceEntity>();
        }

        public long MaxTransID { get; set; }

        public IEnumerable<SumSubscriberEntity> SumSubscriberList { get; set; }

        public IEnumerable<SumDeviceTypeEntity> SumDeviceTypeList { get; set; }

        public IEnumerable<AccountIPEntity> AccountIPList { get; set; }

        public IEnumerable<AccountDeviceEntity> AccountDeviceList { get; set; }
    }
}
