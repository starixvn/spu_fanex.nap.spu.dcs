namespace Fanex.NAP.SPU.DCS.JobService.Models
{
    public enum NotifyType
    {
        MonitorTransaction
    }

    public enum Flagged
    {
        Human,
        EmptyToken,
        InvalidToken,
        EmptyAnalysis,
        InvalidAnalysis,
        EmptyDevice,
        InvalidDevice,
        EmptyUserAgent,
        Headless,
    }
}
