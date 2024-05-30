using Newtonsoft.Json;

namespace Fanex.NAP.SPU.DCS.JobService.Models.ElasticConfig
{
    public class Aggs
    {
        [JsonProperty("group_by_error_message")]
        public GroupByErrorMessage GroupByErrorMessage { get; set; }
    }

    public class Bool
    {
        [JsonProperty("must")]
        public List<Must> Must { get; set; }

        [JsonProperty("must_not")]
        public List<MustNot> MustNot { get; set; }

        [JsonProperty("should")]
        public List<Should> Should { get; set; }
    }

    public class GroupByErrorMessage
    {
        [JsonProperty("terms")]
        public Terms Terms { get; set; }
    }

    public class Match
    {
        [JsonProperty("ParlayStatistics.Topic")]
        public string ParlayStatisticsTopic { get; set; }

        [JsonProperty("ParlayStatistics.SportType")]
        public int? ParlayStatisticsSportType { get; set; }
    }

    public class Must
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("match")]
        public Match Match { get; set; }

        [JsonProperty("bool")]
        public Bool Bool { get; set; }

        [JsonProperty("term")]
        public Term Term { get; set; }

        [JsonProperty("terms")]
        public Terms Terms { get; set; }
    }

    public class MustNot
    {
        [JsonProperty("term")]
        public Term Term { get; set; }

        [JsonProperty("script")]
        public Script Script { get; set; }
    }
    public class Script
    {
        [JsonProperty("script")]
        public Script Script1 { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("params")]
        public Params Params { get; set; }
    }
    public class Params
    {
        [JsonProperty("excluded")]
        public List<int> Excluded { get; set; }
    }

    public class ParlayStatisticsBettingErrorCode
    {
        [JsonProperty("gt")]
        public int Gt { get; set; }
    }

    public class Query
    {
        [JsonProperty("bool")]
        public Bool Bool { get; set; }
    }

    public class Range
    {
        [JsonProperty("@timestamp")]
        public Timestamp Timestamp { get; set; }

        [JsonProperty("ParlayStatistics.BettingErrorCode")]
        public ParlayStatisticsBettingErrorCode ParlayStatisticsBettingErrorCode { get; set; }
    }

    public class RequestBody
    {
        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("query")]
        public Query Query { get; set; }

        [JsonProperty("aggs")]
        public Aggs Aggs { get; set; }
    }

    public class Should
    {
        [JsonProperty("match")]
        public Match Match { get; set; }
    }

    public class Term
    {
        [JsonProperty("ParlayStatistics.SubTickets.IsLive")]
        public bool? ParlayStatisticsSubTicketsIsLive { get; set; }

        [JsonProperty("ParlayStatistics.IsLicensee")]
        public bool? ParlayStatisticsIsLicensee { get; set; }
    }

    public class Terms
    {
        [JsonProperty("ParlayStatistics.SubTickets.CustomerClass")]
        public List<int> ParlayStatisticsSubTicketsCustomerClass { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("size")]
        public int? Size { get; set; }
    }

    public class Timestamp
    {
        [JsonProperty("gte")]
        public string StartTime { get; set; }

        [JsonProperty("lte")]
        public string EndTime { get; set; }
    }




}
