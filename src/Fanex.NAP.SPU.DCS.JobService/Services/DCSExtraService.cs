using Fanex.NAP.SPU.DCS.Features.DCSExtra;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.Dto;
using Fanex.NAP.SPU.DCS.JobService.Ultilities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IDCSExtraService
    {
        Task<bool> MigrateDCSExtra(int rangeMinutes, int stopMinutes, int batchSizeGetData);

        Task<bool> InitialTransIpInfo(int batchSize, int numberOfBatch);
    }

    public class DCSExtraService : IDCSExtraService
    {
        private readonly IDCSExtraServices dcsExtraService;
        private readonly ISentryClient sentryClient;
        private readonly IConfigurationSection settingDCSExtra;

        public DCSExtraService(IDCSExtraServices dcsExtraService, ISentryClient sentryClient, IConfiguration configuration)
        {
            this.dcsExtraService = dcsExtraService;
            this.sentryClient = sentryClient;
            this.settingDCSExtra = configuration.GetSection("DCSExtra");
        }

        public async Task<bool> MigrateDCSExtra(int rangeMinutes, int stopMinutes, int batchSizeGetData)
        {
            try
            {
                var parameterID = settingDCSExtra.GetValue<int>("ParameterID");
                var parameterValue = dcsExtraService.GetParameterValue(parameterID);
                var lastTransTime = DateTime.ParseExact(parameterValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var toTransTime = lastTransTime.AddMinutes(rangeMinutes);
                var stopTime = DateTime.Now.AddMinutes(-stopMinutes);
                if (stopMinutes == 0 || toTransTime <= stopTime)
                {
                    var mergedList = await GetMergedListAsync(lastTransTime, toTransTime, batchSizeGetData);
                    if (mergedList != null && mergedList.Any())
                    {
                        var rawTransJson = JsonConvert.SerializeObject(mergedList);
                        await dcsExtraService.InsertTransformRawTransAsync(rawTransJson);
                    }

                    await dcsExtraService.UpdateParameterValueAsync(parameterID, toTransTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }


        private async Task<IEnumerable<RawTransactionDto>> GetMergedListAsync(DateTime lastTransTime, DateTime toTransTime, int batchSizeGetData)
        {
            var tasks = new List<Task<IEnumerable<RawTransactionModel>>>();

            var dcsExtraConfig = settingDCSExtra.GetSection("Config").GetChildren();
            foreach (var item in dcsExtraConfig)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        return dcsExtraService.GetRawTransactionAsync(item["Site"], item["SettingKey"], lastTransTime, toTransTime);
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                    }

                    return null;
                }));
            }

            var listRawTrans = (await Task.WhenAll(tasks)).Where(result => result != null).SelectMany(result => result);
            if (listRawTrans != null && listRawTrans.Any())
            {
                var deviceIDs = listRawTrans
                                .Where(x => x.DeviceID != null)
                                .Select(x => x.DeviceID)
                                .Distinct();

                var deviceInfo = TransformDeviceInfo(await GetDeviceInfoBatchedAsync(deviceIDs, batchSizeGetData));

                var deviceFingerprint = Enumerable.Empty<DeviceFingerprintDto>();
                if (deviceInfo != null && deviceInfo.Any())
                {
                    var deviceNos = deviceInfo
                                    .Where(x => x.DeviceNo != null)
                                    .Select(x => x.DeviceNo)
                                    .Distinct();

                    deviceFingerprint = TransformDeviceFingerprint(await GetDeviceFingerprintBatchedAsync(deviceNos, batchSizeGetData));
                }

                return MergedList(listRawTrans, deviceInfo, deviceFingerprint);
            }
            return Enumerable.Empty<RawTransactionDto>();
        }

        public async Task<IEnumerable<DeviceInfoModel>> GetDeviceInfoBatchedAsync(IEnumerable<long?> deviceIDs, int batchSize)
        {
            var batches = deviceIDs.Chunk2(batchSize);

            var tasks = new List<Task<IEnumerable<DeviceInfoModel>>>();

            foreach (var batch in batches)
            {
                var deviceIDList = string.Join(",", batch.Select(x => x.ToString()));
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        return dcsExtraService.GetDeviceInfoAsync(deviceIDList);
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                    }

                    return null;
                }));
            }

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(x => x);
        }

        public async Task<IEnumerable<DeviceFingerprintModel>> GetDeviceFingerprintBatchedAsync(IEnumerable<string> deviceNos, int batchSize)
        {
            var batches = deviceNos.Chunk2(batchSize);

            var tasks = new List<Task<IEnumerable<DeviceFingerprintModel>>>();

            foreach (var batch in batches)
            {
                var deviceIDList = string.Join(",", batch.Select(x => x.ToString()));
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        return dcsExtraService.GetDeviceFingerprintAsync(deviceIDList);
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                    }

                    return null;
                }));
            }

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(x => x);
        }

        private IEnumerable<DeviceInfoDto> TransformDeviceInfo(IEnumerable<DeviceInfoModel> model)
        {
            return model.Select(x => new DeviceInfoDto
            {
                DeviceID = x.DeviceID,
                DeviceNo = x.DeviceNo
            });
        }

        private IEnumerable<DeviceFingerprintDto> TransformDeviceFingerprint(IEnumerable<DeviceFingerprintModel> model)
        {
            return model.Select(x => new DeviceFingerprintDto
            {
                DeviceNo = x.DeviceNo,
                FingerprintCode = x.Code
            });
        }

        private IEnumerable<RawTransactionDto> MergedList(IEnumerable<RawTransactionModel> listRawTrans, IEnumerable<DeviceInfoDto> deviceInfo, IEnumerable<DeviceFingerprintDto> deviceFingerprint)
        {
            return listRawTrans
                    .GroupJoin(deviceInfo, a => a.DeviceID, b => b.DeviceID, (a, bGroup) => new { a, bGroup })
                    .SelectMany(ab => ab.bGroup.DefaultIfEmpty(), (a, b) => new { a.a, b })
                    .GroupJoin(deviceFingerprint, ab => ab.b?.DeviceNo ?? null, c => c.DeviceNo, (ab, cGroup) => new { ab.a, cGroup })
                    .SelectMany(abc => abc.cGroup.DefaultIfEmpty(), (ab, c) => new RawTransactionDto
                    {
                        LoginName = ab.a.LoginName,
                        SubscriberName = ab.a.SubscriberName,
                        TransTime = ab.a.TransTime,
                        UserAgent = ab.a.UserAgent,
                        IP = ab.a.IP,
                        PluginID = ab.a.PluginID,
                        URL = ab.a.URL,
                        Action = ab.a.Action,
                        ActionResult = ab.a.ActionResult,
                        FlaggedCode = MappingFlaggedCode(ab.a.Flagged),
                        DeviceID = ab.a.DeviceID,
                        DeviceCode = c?.DeviceNo ?? null,
                        FingerprintCode = c?.FingerprintCode ?? null
                    })
                    .OrderBy(x => x.TransTime);
        }

        private string MappingFlaggedCode(int? flagged)
        {
            if (flagged == null) return Flagged.Human.ToString();

            if (flagged == 5) return Flagged.EmptyToken.ToString();
            if (flagged == 6) return Flagged.EmptyAnalysis.ToString();
            if (flagged == 7) return Flagged.EmptyDevice.ToString();
            if (flagged == 8) return Flagged.InvalidAnalysis.ToString();
            if (flagged == 9) return Flagged.InvalidToken.ToString();
            if (flagged == 10) return Flagged.InvalidDevice.ToString();
            if (flagged % 10 == 3) return Flagged.Human.ToString();
            if (flagged == 212) return Flagged.Human.ToString();
            if (flagged % 10 == 4) return Flagged.Headless.ToString();
            if (flagged == 215) return Flagged.EmptyDevice.ToString();
            if (flagged == 216) return Flagged.InvalidDevice.ToString();

            return Flagged.Human.ToString();
        }

        public async Task<bool> InitialTransIpInfo(int batchSize, int numberOfBatch)
        {
            try
            {
                var initialDataConfig = settingDCSExtra.GetSection("InitialData");
                var lastTransID_ParameterID = initialDataConfig.GetValue<int>("LastTransID_ParameterID");
                var lastCreatedDate_ParameterID = initialDataConfig.GetValue<int>("LastCreatedDate_ParameterID");


                if (batchSize > 0 && numberOfBatch > 0)
                {
                    var trans = await dcsExtraService.GetTransactionInitialDataAsync(batchSize * numberOfBatch);

                    if (trans != null && trans.Any())
                    {
                        var listIpID = trans.Where(t => t.IPID.HasValue)
                                                .Select(t => t.IPID)
                                                .Distinct()
                                                .ToList();

                        if (listIpID != null && listIpID.Any())
                        {
                            var ipIDs = string.Join(",", listIpID);

                            var ipInfo = await dcsExtraService.GetLocationByIPList(ipIDs);
                            if (ipInfo != null && ipInfo.Any())
                            {
                                var tasksInsert = new List<Task>
                                {
                                    InsertCountry(ipInfo),
                                    InsertCity(ipInfo),
                                    InsertISP(ipInfo),
                                    InsertRegion(ipInfo)
                                };

                                await Task.WhenAll(tasksInsert.ToArray());

                                // UPDATE Trans
                                var mappingTransID = MergeTransactionInitial(trans, ipInfo);
                                await UpdateTransactionInitialData(mappingTransID, batchSize);

                                // UPDATE Parameter
                                var lastTransID = mappingTransID.Max(x => x.TransID);
                                var lastCreatedDate = mappingTransID.Max(x => x.CreatedDate);

                                await dcsExtraService.UpdateParameterValueAsync(lastTransID_ParameterID, lastTransID.ToString());
                                await dcsExtraService.UpdateParameterValueAsync(lastCreatedDate_ParameterID, lastCreatedDate.ToString("yyyy-MM-dd"));
                            }
                        }
                    }
                }
                else
                {
                    sentryClient.CaptureException(new Exception("BatchSize is invalid!"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }

            return true;
        }

        private Task InsertCountry(IEnumerable<LocationByIPModel> ipInfo)
        {
            var countryList = ipInfo.Where(t => !string.IsNullOrEmpty(t.CountryCode))
                                    .GroupBy(t => new { t.Country, t.CountryCode })
                                    .Select(group => new CountryModel { Country = group.Key.Country, CountryCode = group.Key.CountryCode })
                                    .ToList();

            if (countryList != null && countryList.Any())
            {
                var countryInputJs = JsonConvert.SerializeObject(countryList);
                return dcsExtraService.InsertListCountryAsync(countryInputJs);
            }

            return Task.CompletedTask;
        }

        private Task InsertCity(IEnumerable<LocationByIPModel> ipInfo)
        {
            var cityList = ipInfo.Where(t => !string.IsNullOrEmpty(t.City))
                                .Select(t => t.City)
                                .Distinct()
                                .ToList();

            if (cityList != null && cityList.Any())
            {
                var cityInputJs = string.Join(",", cityList);
                return dcsExtraService.InsertListCityAsync(cityInputJs);
            }

            return Task.CompletedTask;
        }

        private Task InsertISP(IEnumerable<LocationByIPModel> ipInfo)
        {
            var ispList = ipInfo.Where(t => !string.IsNullOrEmpty(t.ISP))
                                .Select(t => t.ISP)
                                .Distinct()
                                .ToList();

            if (ispList != null && ispList.Any())
            {
                var ispInputJs = string.Join(",", ispList);
                return dcsExtraService.InsertListISPAsync(ispInputJs);
            }

            return Task.CompletedTask;
        }

        private Task InsertRegion(IEnumerable<LocationByIPModel> ipInfo)
        {
            var regionList = ipInfo.Where(t => !string.IsNullOrEmpty(t.Region))
                                .Select(t => t.Region)
                                .Distinct()
                                .ToList();

            if (regionList != null && regionList.Any())
            {
                var regionInputJs = string.Join(",", regionList);
                return dcsExtraService.InsertListRegionAsync(regionInputJs);
            }

            return Task.CompletedTask;
        }

        private IEnumerable<TransactionInitialDataDto> MergeTransactionInitial(IEnumerable<TransactionInitialDataModel> trans, IEnumerable<LocationByIPModel> ipInfo)
        {
            if (trans != null && trans.Any() && ipInfo != null && ipInfo.Any())
            {
                return (from t in trans
                        join ip in ipInfo on t.IPID equals ip.IP
                        select new TransactionInitialDataDto
                        {
                            TransID = t.TransID,
                            CreatedDate = t.CreatedDate,
                            CountryCode = ip.CountryCode,
                            Region = ip.Region,
                            City = ip.City,
                            ISP = ip.ISP
                        }).ToList();
            }
            return new List<TransactionInitialDataDto>();
        }

        private async Task UpdateTransactionInitialData(IEnumerable<TransactionInitialDataDto> mappingTransID, int batchSize)
        {
            if(mappingTransID != null && mappingTransID.Any())
            {
                var batches = mappingTransID.Chunk2(batchSize);
                var tasksUpdate = new List<Task>();

                foreach (var batch in batches)
                {
                    var transJs = JsonConvert.SerializeObject(batch);
                    tasksUpdate.Add(dcsExtraService.UpdateTransactionInitialDataAsync(transJs));
                }

                await Task.WhenAll(tasksUpdate.ToArray());
            }
        }
    }
}
