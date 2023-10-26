namespace Fanex.NAP.SPU.DCS.Features.DataTrace.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class UpdateSystemSettingParameterCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParameterID")]
        public int ID { get; set; }

        [SpParam(SqlParamName = "ip_ParameterValue")]
        public string Value { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_SystemSetting_UpdateParameterByID";
        }

        public override bool IsValid() => true;
    }
}
