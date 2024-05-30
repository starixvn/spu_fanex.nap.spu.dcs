namespace Fanex.NAP.SPU.DCS.JobService.Models.Responses;

public class ElasticResponse
{
    public int took { get; set; }
    public bool timed_out { get; set; }
    public _shards _shards { get; set; }
    public Hits hits { get; set; }
    public Aggregations aggregations { get; set; }

    public Buckets[] TryGetBuckets()
    {
        return aggregations?.group_by_error_message?.buckets ?? Array.Empty<Buckets>(); 
    }
}

public class _shards
{
    public int total { get; set; }
    public int successful { get; set; }
    public int skipped { get; set; }
    public int failed { get; set; }
}

public class Hits
{
    public Total total { get; set; }
    public object max_score { get; set; }
    public object[] hits { get; set; }
}

public class Total
{
    public int value { get; set; }
    public string relation { get; set; }
}

public class Aggregations
{
    public Group_by_error_message group_by_error_message { get; set; }
}

public class Group_by_error_message
{
    public int doc_count_error_upper_bound { get; set; }
    public int sum_other_doc_count { get; set; }
    public Buckets[] buckets { get; set; }
}

public class Buckets
{
    public string key { get; set; }
    public int doc_count { get; set; }
}