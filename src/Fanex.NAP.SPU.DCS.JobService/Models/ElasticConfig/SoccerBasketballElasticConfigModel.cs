using Newtonsoft.Json;

namespace Fanex.NAP.SPU.DCS.JobService.Models.ElasticConfig
{
    public class SoccerBasketballElasticConfigModel
    {
       
        public class Aggs
        {
            [JsonProperty("group_by_error_message", NullValueHandling = NullValueHandling.Ignore)]
            public GroupByErrorMessage GroupByErrorMessage { get; set; }
        }

        public class Bool
        {
            [JsonProperty("must", NullValueHandling = NullValueHandling.Ignore)]
            public List<Must> Must { get; set; }

            [JsonProperty("must_not", NullValueHandling = NullValueHandling.Ignore)]
            public List<MustNot> MustNot { get; set; }
        }

        public class GroupByErrorMessage
        {
            [JsonProperty("terms", NullValueHandling = NullValueHandling.Ignore)]
            public Terms Terms { get; set; }
        }

        public class Match
        {
            [JsonProperty("Topic", NullValueHandling = NullValueHandling.Ignore)]
            public string Topic { get; set; }
        }

        public class Must
        {
            [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
            public Range Range { get; set; }

            [JsonProperty("term", NullValueHandling = NullValueHandling.Ignore)]
            public Term Term { get; set; }

            [JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
            public Match Match { get; set; }

            [JsonProperty("terms", NullValueHandling = NullValueHandling.Ignore)]
            public Terms Terms { get; set; }
        }

        public class MustNot
        {
            [JsonProperty("terms", NullValueHandling = NullValueHandling.Ignore)]
            public Terms Terms { get; set; }
        }

        public class Query
        {
            [JsonProperty("bool", NullValueHandling = NullValueHandling.Ignore)]
            public Bool Bool { get; set; }
        }

        public class Range
        {
            [JsonProperty("@timestamp", NullValueHandling = NullValueHandling.Ignore)]
            public Timestamp Timestamp { get; set; }
        }

        public class RequestBody
        {
            [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
            public int From { get; set; }

            [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
            public int Size { get; set; }

            [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
            public Query Query { get; set; }

            [JsonProperty("aggs", NullValueHandling = NullValueHandling.Ignore)]
            public Aggs Aggs { get; set; }
        }

        public class Term
        {
            [JsonProperty("IsLive", NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsLive { get; set; }

            [JsonProperty("IsLicensee", NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsLicensee { get; set; }

            [JsonProperty("SportType", NullValueHandling = NullValueHandling.Ignore)]
            public int? SportType { get; set; }            
        }

        public class Terms
        {
            [JsonProperty("LeagueGroupId", NullValueHandling = NullValueHandling.Ignore)]
            public List<int> LeagueGroupId { get; set; }

            [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
            public string Field { get; set; }

            [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
            public int? Size { get; set; }

            [JsonProperty("CustomerClass", NullValueHandling = NullValueHandling.Ignore)]
            public List<int> CustomerClass { get; set; }
        }

        public class Timestamp
        {
            [JsonProperty("gte", NullValueHandling = NullValueHandling.Ignore)]
            public string StartTime { get; set; }

            [JsonProperty("lt", NullValueHandling = NullValueHandling.Ignore)]
            public string EndTime { get; set; }
        }
    }
}
