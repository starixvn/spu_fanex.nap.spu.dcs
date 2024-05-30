using Fanex.NAP.SPU.DCS.Features.ELK;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.ElasticConfig;
using Fanex.NAP.SPU.DCS.JobService.Models.Responses;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IELKJobService
    {
        Task<bool> Execute(DateTime startTime, DateTime endTime);
    }

    public enum SportSetting
    {
        All,
        OnlySoccer,
        NotOnlySoccer
    }

    public enum SiteTypeSetting
    {
        All,
        Credit,
        Licensee
    }

    public class ELKJobService : IELKJobService
    {
        private readonly IELKServices elkService;
        private readonly IAppSettings appSettings;
        private readonly ILogService logService;

        public ELKJobService(IELKServices elkService, IAppSettings appSettings, ILogService logService)
        {
            this.elkService = elkService;
            this.appSettings = appSettings;
            this.logService = logService;
        }

        public async Task<bool> Execute(DateTime startTime, DateTime endTime)
        {
            try
            {
                foreach (SportSetting sport in Enum.GetValues(typeof(SportSetting)))
                {
                    foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
                    {
                        var elkRateReport = await GetELKData(startTime, endTime, sport, siteType);

                        if (elkRateReport != null && elkRateReport.Any())
                        {
                            var elkRateReportJson = System.Text.Json.JsonSerializer.Serialize(elkRateReport);

                            if (!string.IsNullOrWhiteSpace(elkRateReportJson))
                            {
                                elkService.PrepareStagingForParlayValidationStats(startTime, sport.ToString(), siteType.ToString(), elkRateReportJson, 1);

                                elkService.PrepareStagingForParlayValidationStats(startTime, sport.ToString(), siteType.ToString(), elkRateReportJson, 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private string BuildConfig(SportSetting sportSetting, SiteTypeSetting siteTypeSetting, DateTime startTime, DateTime endTime)
        {
            var mustRange = new Must
            {
                Range = new Models.ElasticConfig.Range
                {
                    Timestamp = new Timestamp
                    {
                        StartTime = startTime.ToString("yyyy-MM-dd") + "T04:00:00",
                        EndTime = endTime.ToString("yyyy-MM-dd") + "T04:00:00"
                    }
                }

            };

            var mustMatch = new Must
            {
                Match = new Match
                {
                    ParlayStatisticsTopic = "MixParlay",
                    ParlayStatisticsSportType = null
                }

            };

            var mustTerms = new Must
            {
                Terms = new Terms
                {
                    ParlayStatisticsSubTicketsCustomerClass = new List<int> { 1, 2, 100, 111, 112, 113, 201, 202, 901, 999, 1001, 2001, 2101, 2201 },
                    Size = null,
                },
                Bool = null,

            };

            var mustBool = new Must
            {
                Bool = new Bool()
            };

            var mustTerm = new Must
            {
                Term = new Term()
            };

            var mustTermIsLive = new Must
            {
                Term = new Term {
                    ParlayStatisticsSubTicketsIsLive = true,
                    ParlayStatisticsIsLicensee = null
                }
            };

            var reportConfig = new RequestBody()
            {
                From = 0,
                Size = 0,
                Aggs = new Aggs
                {
                    GroupByErrorMessage = new GroupByErrorMessage
                    {
                        Terms = new Terms
                        {
                            Field = "ParlayStatistics.BettingErrorMessage",
                            Size = 100,
                        }
                    }
                },
                Query = new Query
                {
                    Bool = new Bool
                    {
                        Must = new List<Must>()
                    }
                }
            };

            if (sportSetting == SportSetting.All)
            {
                mustBool.Bool.Should = new List<Should> {
                        new Should
                        {
                            Match = new Match
                            {
                                ParlayStatisticsSportType = 1
                            }
                        },
                        new Should
                        {
                            Match = new Match
                            {
                                ParlayStatisticsSportType = 99
                            }
                        }
                    };

            }
            else if (sportSetting == SportSetting.OnlySoccer)
            {
                mustBool.Bool.Should = new List<Should> {
                        new Should
                        {
                            Match = new Match
                            {
                                ParlayStatisticsSportType = 1
                            }
                        }
                    };
            }
            else
            {
                mustBool.Bool.Should = new List<Should> {
                        new Should
                        {
                            Match = new Match
                            {
                                ParlayStatisticsSportType = 99
                            }
                        }
                    };
            }

            if (siteTypeSetting == SiteTypeSetting.Licensee)
            {
                mustTerm.Term = new Term
                {
                    ParlayStatisticsIsLicensee = true,
                    ParlayStatisticsSubTicketsIsLive = null
                };
            }
            else if (siteTypeSetting == SiteTypeSetting.Credit)
            {
                mustTerm.Term = new Term
                {
                    ParlayStatisticsIsLicensee = false,
                    ParlayStatisticsSubTicketsIsLive = null
                };
            }

            reportConfig.Query.Bool.Must.Add(mustRange);
            reportConfig.Query.Bool.Must.Add(mustMatch);
            reportConfig.Query.Bool.Must.Add(mustBool);            
            reportConfig.Query.Bool.Must.Add(mustTermIsLive);

            if(siteTypeSetting != SiteTypeSetting.All)
            {
                reportConfig.Query.Bool.Must.Add(mustTerm);
            }
            

            reportConfig.Query.Bool.Must.Add(mustTerms);

            reportConfig.Query.Bool.MustNot = new List<MustNot> {
                new MustNot
                {
                    Term = new Term{
                        ParlayStatisticsSubTicketsIsLive = false,
                        ParlayStatisticsIsLicensee = null
                    }
                },
                new MustNot
                {
                    Script = new Script
                    {
                        Script1 = new Script
                        {
                            Source = "params.excluded.contains(doc['ParlayStatistics.SubTickets.CustomerClass'].value)",
                            Params = new Params
                            {
                                Excluded = new List<int> { 1, 2, 100, 111, 112, 113, 201, 202, 901, 999, 1001, 2001, 2101, 2201 }
                            },
                            Script1 = null
                        }
                    }
                }
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(reportConfig, settings);
        }

        private async Task<IEnumerable<SuccessRateModel>> GetELKData(DateTime startTime, DateTime endTime, SportSetting sportSetting, SiteTypeSetting siteTypeSetting)
        {
            var config = BuildConfig(sportSetting, siteTypeSetting, startTime, endTime);

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(appSettings.ELKUrl),
                DefaultRequestHeaders =
                {
                     Authorization = new AuthenticationHeaderValue("Basic", "ZWxhc3RpYzpwMGFseEA5ZGxj")
                }
            };

            var httpResponseMessage = await httpClient.PostAsync("logs-bettingresult-prod/_search", new StringContent(config, null, "application/json"));

            var elasticResponse = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ElasticResponse>();

            var successRate = elasticResponse
                .TryGetBuckets()
                .Select(x => new SuccessRateModel()
                {
                    BettingErrorMessage = x.key,
                    Count = x.doc_count,
                });

            return successRate;

        }
    }
}
