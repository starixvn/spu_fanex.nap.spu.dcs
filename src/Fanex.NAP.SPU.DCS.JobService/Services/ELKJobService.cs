using Fanex.NAP.SPU.DCS.Features.ELK;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.ElasticConfig;
using Fanex.NAP.SPU.DCS.JobService.Models.MixParlayElasticConfig;
using Fanex.NAP.SPU.DCS.JobService.Models.Responses;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
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

    [ExcludeFromCodeCoverage(Justification = "Excluded from code coverage")]
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

        private async Task ProccessForSoccerBasketball(DateTime startTime, DateTime endTime)
        {
            try
            {
                foreach (SportType sport in Enum.GetValues(typeof(SportType)))
                {
                    await ProcessSiteTypesForSport(startTime, endTime, sport);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
            }
        }

        private async Task ProcessSiteTypesForSport(DateTime startTime, DateTime endTime, SportType sport)
        {
            foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
            {
                await ProcessElkRateReport(startTime, endTime, sport, siteType);
            }
        }

        private async Task ProcessElkRateReport(DateTime startTime, DateTime endTime, SportType sport, SiteTypeSetting siteType)
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
                        await ProcessLeagueGroup(startTime, endTime, leagueGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
            }
        }

        private async Task ProcessLeagueGroup(DateTime startTime, DateTime endTime, LeagueGroupModel leagueGroup)
        {
            var tasks = Enum.GetValues(typeof(SportType))
                            .Cast<SportType>()
                            .Where(sport => leagueGroup.SportType.Equals(sport))
                            .Select(sport => ProcessSiteTypes(startTime, endTime, leagueGroup, sport));

            await Task.WhenAll(tasks);
        }

        private async Task ProcessSiteTypes(DateTime startTime, DateTime endTime, LeagueGroupModel leagueGroup, SportType sport)
        {
            foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
            {
                var elkRateReport = await GetELKDataForSoccerBasketball(startTime, endTime, sport, siteType, leagueGroup.LeagueGroupId);

                if (elkRateReport != null && elkRateReport.Any())
                {
                    var elkRateReportJson = System.Text.Json.JsonSerializer.Serialize(elkRateReport);

                    if (!string.IsNullOrWhiteSpace(elkRateReportJson))
                    {
                        await PrepareStagingForBetMessageDaily(startTime, sport, siteType, leagueGroup, elkRateReportJson);
                    }
                }
            }
        }

        private async Task PrepareStagingForBetMessageDaily(DateTime startTime, SportType sport, SiteTypeSetting siteType, LeagueGroupModel leagueGroup, string elkRateReportJson)
        {
            await Task.Run(() =>
            {
                elkService.PrepareStagingForBetMessageDaily(startTime, sport.ToString(), siteType.ToString(), leagueGroup.LeagueGroupId, elkRateReportJson, 1);
                elkService.PrepareStagingForBetMessageDaily(startTime, sport.ToString(), siteType.ToString(), leagueGroup.LeagueGroupId, elkRateReportJson, 2);
            });
        }

        private async Task ProccessForMixParlay(DateTime startTime, DateTime endTime)
        {
            try
            {
                foreach (SportSetting sport in Enum.GetValues(typeof(SportSetting)))
                {
                    await ProcessSiteTypesForMixParlay(startTime, endTime, sport);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
            }
        }

        private async Task ProcessSiteTypesForMixParlay(DateTime startTime, DateTime endTime, SportSetting sport)
        {
            foreach (SiteTypeSetting siteType in Enum.GetValues(typeof(SiteTypeSetting)))
            {
                await ProcessElkRateReportForMixParlay(startTime, endTime, sport, siteType);
            }
        }

        private async Task ProcessElkRateReportForMixParlay(DateTime startTime, DateTime endTime, SportSetting sport, SiteTypeSetting siteType)
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

        private string BuildConfigForSoccerBasketball(SportType sportType, SiteTypeSetting siteTypeSetting, DateTime startTime, DateTime endTime, int? leagueGroupId)
        {
            var reportConfig = InitializeReportConfig();
            AddMustConditions(reportConfig, startTime, endTime, siteTypeSetting, sportType, leagueGroupId);
            AddMustNotConditions(reportConfig);
            AddAggregations(reportConfig);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(reportConfig, settings);
        }

        private SoccerBasketballElasticConfigModel.RequestBody InitializeReportConfig()
        {
            return new SoccerBasketballElasticConfigModel.RequestBody
            {
                From = 0,
                Size = 0,
                Query = new SoccerBasketballElasticConfigModel.Query
                {
                    Bool = new SoccerBasketballElasticConfigModel.Bool
                    {
                        Must = new List<SoccerBasketballElasticConfigModel.Must>(),
                        MustNot = new List<SoccerBasketballElasticConfigModel.MustNot>()
                    }
                },
                Aggs = new SoccerBasketballElasticConfigModel.Aggs()
            };
        }

        private void AddMustConditions(
            SoccerBasketballElasticConfigModel.RequestBody reportConfig, 
            DateTime startTime, 
            DateTime endTime, 
            SiteTypeSetting siteTypeSetting, 
            SportType sportType, 
            int? leagueGroupId)
        {
            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
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
                    IsLive = true
                }
            });

            if (siteTypeSetting == SiteTypeSetting.Credit)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        IsLicensee = false
                    }
                });
            }
            if (siteTypeSetting == SiteTypeSetting.Licensee)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        IsLicensee = true
                    }
                });
            }

            reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
            {
                Terms = new SoccerBasketballElasticConfigModel.Terms
                {
                    CustomerClass = new List<int> { 1, 2, 100, 111, 112, 113, 201, 202, 901, 999, 1001, 2001, 2101, 2201 }
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
                    SportType = (int)sportType
                }
            });

            if (leagueGroupId != null && leagueGroupId != -1)
            {
                reportConfig.Query.Bool.Must.Add(new SoccerBasketballElasticConfigModel.Must
                {
                    Term = new SoccerBasketballElasticConfigModel.Term
                    {
                        LeagueGroupId = leagueGroupId
                    }
                });
            }
        }

        private void AddMustNotConditions(SoccerBasketballElasticConfigModel.RequestBody reportConfig)
        {
            reportConfig.Query.Bool.MustNot.Add(new SoccerBasketballElasticConfigModel.MustNot
            {
                Terms = new SoccerBasketballElasticConfigModel.Terms
                {
                    LeagueGroupId = new List<int> { 42, 74, 108, 113, 122, 151 }
                }
            });
        }

        private void AddAggregations(SoccerBasketballElasticConfigModel.RequestBody reportConfig)
        {
            reportConfig.Aggs.GroupByErrorMessage = new SoccerBasketballElasticConfigModel.GroupByErrorMessage
            {
                Terms = new SoccerBasketballElasticConfigModel.Terms
                {
                    Field = "BettingErrorMessage",
                    Size = 200
                }
            };
        }

        private string BuildConfigForMixParlay(SportSetting sportSetting, SiteTypeSetting siteTypeSetting, DateTime startTime, DateTime endTime)
        {
            var reportConfig = InitializeMixParlayReportConfig();
            AddMixParlayMustConditions(reportConfig, startTime, endTime, sportSetting, siteTypeSetting);
            AddMixParlayMustNotConditions(reportConfig);
            AddMixParlayAggregations(reportConfig);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(reportConfig, settings);
        }

        private RequestBody InitializeMixParlayReportConfig()
        {
            return new RequestBody
            {
                From = 0,
                Size = 0,
                Query = new Query
                {
                    Bool = new Bool
                    {
                        Must = new List<Must>()
                    }
                },
                Aggs = new Aggs
                {
                    GroupByErrorMessage = new GroupByErrorMessage
                    {
                        Terms = new Terms
                        {
                            Field = "ParlayStatistics.BettingErrorMessage",
                            Size = 100
                        }
                    }
                }
            };
        }

        private void AddMixParlayMustConditions(RequestBody reportConfig, DateTime startTime, DateTime endTime, SportSetting sportSetting, SiteTypeSetting siteTypeSetting)
        {
            AddRangeCondition(reportConfig, startTime, endTime);
            AddMatchCondition(reportConfig);
            AddCustomerClassCondition(reportConfig);
            AddSportTypeCondition(reportConfig, sportSetting);
            AddIsLiveCondition(reportConfig);
            AddSiteTypeCondition(reportConfig, siteTypeSetting);
        }

        private void AddRangeCondition(RequestBody reportConfig, DateTime startTime, DateTime endTime)
        {
            reportConfig.Query.Bool.Must.Add(new Must
            {
                Range = new Models.MixParlayElasticConfig.Range
                {
                    Timestamp = new Timestamp
                    {
                        StartTime = startTime.ToString("yyyy-MM-dd") + "T04:00:00",
                        EndTime = endTime.ToString("yyyy-MM-dd") + "T04:00:00"
                    }
                }
            });
        }

        private void AddMatchCondition(RequestBody reportConfig)
        {
            reportConfig.Query.Bool.Must.Add(new Must
            {
                Match = new Match
                {
                    ParlayStatisticsTopic = "MixParlay"
                }
            });
        }

        private void AddCustomerClassCondition(RequestBody reportConfig)
        {
            reportConfig.Query.Bool.Must.Add(new Must
            {
                Terms = new Terms
                {
                    ParlayStatisticsSubTicketsCustomerClass = new List<int>
            {
                1, 2, 100, 111, 112, 113, 201, 202, 901, 999, 1001, 2001, 2101, 2201
            }
                }
            });
        }

        private void AddSportTypeCondition(RequestBody reportConfig, SportSetting sportSetting)
        {
            var mustBool = new Must
            {
                Bool = new Bool()
            };

            if (sportSetting == SportSetting.All)
            {
                mustBool.Bool.Should = new List<Should>
        {
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
                mustBool.Bool.Should = new List<Should>
        {
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
                mustBool.Bool.Should = new List<Should>
        {
            new Should
            {
                Match = new Match
                {
                    ParlayStatisticsSportType = 99
                }
            }
        };
            }

            reportConfig.Query.Bool.Must.Add(mustBool);
        }

        private void AddIsLiveCondition(RequestBody reportConfig)
        {
            reportConfig.Query.Bool.Must.Add(new Must
            {
                Term = new Term
                {
                    ParlayStatisticsSubTicketsIsLive = true
                }
            });
        }

        private void AddSiteTypeCondition(RequestBody reportConfig, SiteTypeSetting siteTypeSetting)
        {
            if (siteTypeSetting == SiteTypeSetting.Licensee)
            {
                reportConfig.Query.Bool.Must.Add(new Must
                {
                    Term = new Term
                    {
                        ParlayStatisticsIsLicensee = true
                    }
                });
            }
            if (siteTypeSetting == SiteTypeSetting.Credit)
            {
                reportConfig.Query.Bool.Must.Add(new Must
                {
                    Term = new Term
                    {
                        ParlayStatisticsIsLicensee = false
                    }
                });
            }
        }

        private void AddMixParlayMustNotConditions(RequestBody reportConfig)
        {
            reportConfig.Query.Bool.MustNot = new List<MustNot>
            {
                new MustNot
                {
                    Term = new Term
                    {
                        ParlayStatisticsSubTicketsIsLive = false
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
                            }
                        }
                    }
                }
            };
        }

        private void AddMixParlayAggregations(RequestBody reportConfig)
        {
            reportConfig.Aggs.GroupByErrorMessage = new GroupByErrorMessage
            {
                Terms = new Terms
                {
                    Field = "ParlayStatistics.BettingErrorMessage",
                    Size = 100
                }
            };
        }

        private async Task<IEnumerable<SuccessRateModel>> GetELKDataForMixParlay(DateTime startTime, DateTime endTime, SportSetting sportSetting, SiteTypeSetting siteTypeSetting)
        {
            var config = BuildConfigForMixParlay(sportSetting, siteTypeSetting, startTime, endTime);

            var httpClient = new HttpClient
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
                .Select(x => new SuccessRateModel
                {
                    BettingErrorMessage = x.key,
                    Count = x.doc_count,
                });

            return successRate;

        }

        private async Task<IEnumerable<SuccessRateModel>> GetELKDataForSoccerBasketball(DateTime startTime, DateTime endTime, SportType sportType, SiteTypeSetting siteType, int? leagueGroupId = null)
        {
            var config = BuildConfigForSoccerBasketball(sportType, siteType, startTime, endTime, leagueGroupId);

            var httpClient = new HttpClient
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
                .Select(x => new SuccessRateModel
                {
                    BettingErrorMessage = x.key,
                    Count = x.doc_count,
                });

            return successRate;

        }


    }
}
