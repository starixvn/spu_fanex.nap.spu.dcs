namespace Fanex.NAP.SPU.DCS.Features.Devices.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformTransaction07InsertCommand : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]

        public string transJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReadyDbExtra { get; set; }

        public override string GetSettingKey()
        {
            return IsReadyDbExtra ? "DCS_ET_Transform_Transaction07_Insert" : "DCS_DC_Transform_Transaction07_Insert";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(transJson);
    }
}
