using System;

namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Model
{
    public class ReportLoginTransactionByBrowserModel
    {
        public DateTime TransDate { get; set; }

        public ulong BrowserID { get; set; }

        public string BrowserName { get; set; }

        public ulong OSID { get; set; }

        public string OSName { get; set; }

        public ulong DeviceTypeID { get; set; }

        public string DeviceType { get; set; }

        public int TotalTrans { get; set; }

        public int TotalInvalidBrowers { get; set; }

        public int TotalBotTrans { get; set; }
    }
}