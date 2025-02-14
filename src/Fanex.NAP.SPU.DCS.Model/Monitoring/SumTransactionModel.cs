namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Model
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.SPU.DCS.Features.Monitoring.Entities;

    public class SumTransactionModel
    {
        public SumTransactionModel()
        {
            SumSubscriberList = new List<SumSubscriberEntity>();
            SumDeviceTypeList = new List<SumDeviceTypeEntity>();
            AccountIPList = new List<AccountIPEntity>();
            AccountDeviceList = new List<AccountDeviceEntity>();
        }

        public UInt64 MaxTransID { get; set; }

        public DateTime DateScan { get; set; }

        public UInt64 AccountID { get; set; }

        public IEnumerable<SumSubscriberEntity> SumSubscriberList { get; set; }

        public IEnumerable<SumDeviceTypeEntity> SumDeviceTypeList { get; set; }

        public IEnumerable<AccountIPEntity> AccountIPList { get; set; }

        public IEnumerable<AccountDeviceEntity> AccountDeviceList { get; set; }
    }
}
