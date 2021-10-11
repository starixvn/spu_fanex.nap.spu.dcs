namespace Fanex.NAP.SPU.DCS.Features.Monitoring.Model
{  
    public class NotifyTransactionModel
    {
        public NotifyTransactionModel()
        {
            this.YesterdayLess50Percent = string.Empty;            
            this.ValidLess50Percent = string.Empty;
        }

        public string YesterdayLess50Percent { get; set; }        

        public string ValidLess50Percent { get; set; }

    }
}
