using Fanex.NAP.SPU.DCS.Features.ELK;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.ElasticConfig;
using Fanex.NAP.SPU.DCS.JobService.Models.MixParlayElasticConfig;
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

        Task<bool> Execute(DateTime startTime, DateTime endTime, DateTime betDate);
    }

    public enum SportType
    {
        Soccer = 1,
        Basketball = 2
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
                await Task.WhenAll(ProccessForMixParlay(startTime, endTime), ProccessForSoccerBasketball(startTime, endTime));
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private async Task ProccessForSoccerBasketball(DateTime startTime, DateTime endTime)
        {
            try
            {
                foreach (SportType sport in Enum.GetValues(typeof(SportType)))
                {
                    foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
                    {
                        var elkRateReport = await GetELKDataForSoccerBasketball(startTime, endTime, sport, siteType);

                        if (elkRateReport != null && elkRateReport.Any())
                        {
                            var elkRateReportJson = System.Text.Json.JsonSerializer.Serialize(elkRateReport);

                            if (!string.IsNullOrWhiteSpace(elkRateReportJson))
                            {
                                elkService.PrepareStagingForBetMessage(startTime, sport.ToString(), siteType.ToString(), elkRateReportJson, 1);

                                elkService.PrepareStagingForBetMessage(startTime, sport.ToString(), siteType.ToString(), elkRateReportJson, 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
            }
        }

        private IEnumerable<LeagueGroupModel> GetLeagueGroup(DateTime betDate)
        {
            var leagueGroupIds = elkService.GetLeagueGroupId(betDate);

            return leagueGroupIds.Select(x => new LeagueGroupModel { LeagueGroupId = x.LeagueGroupId, SportType = (SportType)x.SportType });

        }

        private async Task ProccessForSoccerBasketballWithLeagueGroup(DateTime startTime, DateTime endTime, DateTime betDate)
        {
            try
            {
                var leagueGroups = GetLeagueGroup(betDate);

                if (leagueGroups != null && leagueGroups.Any())
                {
                    foreach (var leagueGroup in leagueGroups)
                    {
                        foreach (SportType sport in Enum.GetValues(typeof(SportType)))
                        {                            
                            if(leagueGroup.SportType.Equals(sport))
                            {
                                foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
                                {
                                    var elkRateReport = await GetELKDataForSoccerBasketball(startTime, endTime, sport, siteType, leagueGroup.LeagueGroupId);

                                    if (elkRateReport != null && elkRateReport.Any())
                                    {
                                        var elkRateReportJson = System.Text.Json.JsonSerializer.Serialize(elkRateReport);

                                        if (!string.IsNullOrWhiteSpace(elkRateReportJson))
                                        {
                                            elkService.PrepareStagingForBetMessageDaily(startTime, sport.ToString(), siteType.ToString(), leagueGroup.LeagueGroupId, elkRateReportJson, 1);

                                            elkService.PrepareStagingForBetMessageDaily(startTime, sport.ToString(), siteType.ToString(), leagueGroup.LeagueGroupId, elkRateReportJson, 2);
                                        }
                                    }
                                }
                            }                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
            }
        }

        private async Task ProccessForMixParlay(DateTime startTime, DateTime endTime)
        {
            try
            {
                foreach (SportSetting sport in Enum.GetValues(typeof(SportSetting)))
                {
                    foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
                    {
                        var elkRateReport = await GetELKDataForMixParlay(startTime, endTime, sport, siteType);

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
            }
        }

        private string BuildConfigForSoccerBasketball(SportType sportType, SiteTypeSetting siteTypeSetting, DateTime startTime, DateTime endTime, int? leagueGroupId)
        {
            var reportConfig = new SoccerBasketballElasticConfigModel.RequestBody()
            {
                From = 0,
                Size = 0,
                Query = new SoccerBasketballElasticConfigModel.Query
                {
                    Bool = new SoccerBasketballElasticConfigModel.Bool
                    {
                        Must = new List<SoccerBasketballElasticConfigModel.Must>(),

                        MustNot = new List<SoccerBasketballElasticConfigModel.MustNot>
                        {
                            new SoccerBasketballElasticConfigModel.MustNot
                            {
                                Terms = new SoccerBasketballElasticConfigModel.Terms
                                {
                                    LeagueGroupId = new List<int> { 42, 74, 108, 113, 122, 151 },
                                    Size = null,
                                    CustomerClass = null,
                                    Field = null
                                }
                            }
                        }

                    }
                },
                Aggs = new SoccerBasketballElasticConfigModel.Aggs
                {
                    GroupByErrorMessage = new SoccerBasketballElasticConfigModel.GroupByErrorMessage
                    {
                        Terms = new SoccerBasketballElasticConfigModel.Terms
                        {
                            Field = "BettingErrorMessage",
                            Size = 200,
                            CustomerClass = null,
                            LeagueGroupId = null
                        }
                    }
                }
            };

            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Term = null,
                Match = null,
                Range = new SoccerBasketballElasticConfigModel.Range
                {
                    Timestamp = new SoccerBasketballElasticConfigModel.Timestamp
                    {
                        StartTime = startTime.ToString("yyyy-MM-dd") + "T04:00:00",
                        EndTime = endTime.ToString("yyyy-MM-dd") + "T04:00:00"
                    }
                }
            });

            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Term = new SoccerBasketballElasticConfigModel.Term
                {
                    IsLive = true,
                    IsLicensee = null,
                    SportType = null
                }
            });

            if (siteTypeSetting == SiteTypeSetting.Credit)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        IsLive = null,
                        IsLicensee = false,
                        SportType = null
                    }
                });
            }
            else if (siteTypeSetting == SiteTypeSetting.Licensee)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        IsLive = null,
                        IsLicensee = true,
                        SportType = null
                    }
                });
            }
            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Terms = new SoccerBasketballElasticConfigModel.Terms
                {
                    CustomerClass = new List<int> { 1, 2, 100, 111, 112, 113, 201, 202, 901, 999, 1001, 2001, 2101, 2201 },
                    Field = null,
                    LeagueGroupId = null,
                    Size = null
                }
            });
            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Match = new SoccerBasketballElasticConfigModel.Match
                {
                    Topic = "UnderOver"
                }
            });

            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Term = new SoccerBasketballElasticConfigModel.Term
                {
                    IsLive = null,
                    IsLicensee = null,
                    SportType = (int)sportType
                }
            });

            if (leagueGroupId != null)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        IsLive = null,
                        IsLicensee = null,
                        SportType = null,
                        LeagueGroupId = leagueGroupId
                    }
                });
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(reportConfig, settings);
        }


        private string BuildConfigForMixParlay(SportSetting sportSetting, SiteTypeSetting siteTypeSetting, DateTime startTime, DateTime endTime)
        {
            var mustRange = new Must
            {
                Range = new Models.MixParlayElasticConfig.Range
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
                Term = new Term
                {
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

            if (siteTypeSetting != SiteTypeSetting.All)
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

        private async Task<IEnumerable<SuccessRateModel>> GetELKDataForMixParlay(DateTime startTime, DateTime endTime, SportSetting sportSetting, SiteTypeSetting siteTypeSetting)
        {
            var config = BuildConfigForMixParlay(sportSetting, siteTypeSetting, startTime, endTime);

            HttpClientHandler handler = new HttpClientHandler
            {
                // Bypass SSL certificate validation for testing purposes
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(100),
                BaseAddress = new Uri(appSettings.ELKUrl),
                DefaultRequestHeaders =
                {
                     Authorization = new AuthenticationHeaderValue("Basic", "bG9nc3Rhc2gud3JpdGVyOmxvZ3N0YXNoLndyaXRlcg==")
                }
            };

            var httpResponseMessage = await httpClient.PostAsync("infinity-bettingresult-prod/_search", new StringContent(config, null, "application/json"));

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
        private async Task<IEnumerable<SuccessRateModel>> GetELKDataForSoccerBasketball(DateTime startTime, DateTime endTime, SportType sportType, SiteTypeSetting siteType, int? leagueGroupId = null)
        {
            var config = BuildConfigForSoccerBasketball(sportType, siteType, startTime, endTime, leagueGroupId);

            HttpClientHandler handler = new HttpClientHandler
            {
                // Bypass SSL certificate validation for testing purposes
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(100),
                BaseAddress = new Uri(appSettings.ELKUrl),
                DefaultRequestHeaders =
                {
                     Authorization = new AuthenticationHeaderValue("Basic", "bG9nc3Rhc2gud3JpdGVyOmxvZ3N0YXNoLndyaXRlcg==")
                }
            };

            var httpResponseMessage = await httpClient.PostAsync("infinity-bettingresult-prod/_search", new StringContent(config, null, "application/json"));

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

        public async Task<bool> Execute(DateTime startTime, DateTime endTime, DateTime betDate)
        {
            try
            {
                await ProccessForSoccerBasketballWithLeagueGroup(startTime, endTime, betDate);
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
