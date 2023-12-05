namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.DCSExtra;
    using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
    using Fanex.NAP.SPU.DCS.JobService.Models;
    using Fanex.NAP.SPU.DCS.JobService.Models.Dto;
    using Fanex.NAP.SPU.DCS.JobService.Ultilities;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Sentry;

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
                var stopTime = DateTimeOffset.Now.AddMinutes(-stopMinutes);
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

                    return Task.FromResult<IEnumerable<RawTransactionModel>>(null);
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
                var deviceIDList = string.Join(",", batch.Select(x => x));
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

                    return Task.FromResult<IEnumerable<DeviceInfoModel>>(null);
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
                var deviceIDList = string.Join(",", batch.Select(x => x));
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

                    return Task.FromResult<IEnumerable<DeviceFingerprintModel>>(null);
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

        private static string MappingFlaggedCode(int? flagged)
        {
            if (flagged == null) return Flagged.Human.ToString();

            var mapping = new Dictionary<int, string>
            {
                { 5, Flagged.EmptyToken.ToString() },
                { 6, Flagged.EmptyAnalysis.ToString() },
                { 7, Flagged.EmptyDevice.ToString() },
                { 8, Flagged.InvalidAnalysis.ToString() },
                { 9, Flagged.InvalidToken.ToString() },
                { 10, Flagged.InvalidDevice.ToString() },
                { 212, Flagged.Human.ToString() },
                { 215, Flagged.EmptyDevice.ToString() },
                { 216, Flagged.InvalidDevice.ToString() }
            };

            if (mapping.TryGetValue(flagged.Value, out var result))
            {
                return result;
            }

            if (flagged % 10 == 3) return Flagged.Human.ToString();
            if (flagged % 10 == 4) return Flagged.Headless.ToString();

            return Flagged.Human.ToString();
        }

        public async Task<bool> InitialTransIpInfo(int batchSize, int numberOfBatch)
        {
            try
            {
                if (!ValidateBatchParameters(batchSize, numberOfBatch))
                {
                    sentryClient.CaptureException(new Exception("BatchSize is invalid!"));
                    return false;
                }

                var initialDataConfig = settingDCSExtra.GetSection("InitialData");
                var lastTransID_ParameterID = initialDataConfig.GetValue<int>("LastTransID_ParameterID");

                sentryClient.CaptureMessage("Start InitialTransIpInfo!");

                var trans = await dcsExtraService.GetTransactionInitialDataAsync(batchSize * numberOfBatch, lastTransID_ParameterID);

                sentryClient.CaptureMessage("GetTransactionInitialDataAsync Finished!");
                if (trans != null && trans.Any())
                {
                    var listIpID = trans.Where(t => t.IPID.HasValue)
                                            .Select(t => t.IPID)
                                            .Distinct()
                                            .ToList();

                    await ProcessTransactionByIps(batchSize, lastTransID_ParameterID, trans, listIpID);
                }

            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }

            return true;
        }

        private async Task ProcessTransactionByIps(int batchSize, int lastTransID_ParameterID, IEnumerable<TransactionInitialDataModel> trans, List<decimal?> listIpID)
        {
            if (listIpID != null && listIpID.Any())
            {
                var ipIDs = string.Join(",", listIpID);

                var locationByIP = await dcsExtraService.GetLocationByIPList(ipIDs);
                var ipInfo = HashIPInfoCode(locationByIP);

                sentryClient.CaptureMessage("GetLocationByIPList Finished!");
                if (ipInfo != null && ipInfo.Any())
                {
                    await InsertIPInfo(ipInfo);
                    sentryClient.CaptureMessage("InsertIPInfo Finished!");

                    // UPDATE Trans
                    var mappingTransID = MergeTransactionInitial(trans, ipInfo);
                    await UpdateTransactionInitialData(mappingTransID, batchSize);

                    sentryClient.CaptureMessage("UpdateTransactionInitialData Finished!");
                    // UPDATE Parameter
                    var lastTransID = mappingTransID.Min(x => x.TransID);

                    await dcsExtraService.UpdateParameterValueAsync(lastTransID_ParameterID, lastTransID.ToString());

                    sentryClient.CaptureMessage("UpdateParameterValueAsync Finished!");
                }
            }
        }

        private static bool ValidateBatchParameters(int batchSize, int numberOfBatch) => batchSize > 0 && numberOfBatch > 0;

        private static IEnumerable<LocationByIPModel> HashIPInfoCode(IEnumerable<LocationByIPModel> ipInfo)
        {
            foreach (var item in ipInfo)
            {
                item.IPInfoCode = Cipher.Hashing2Int(string.Join(";", item.CountryCode, item.Region, item.City, item.ISP));
            }

            return ipInfo;
        }

        private Task InsertIPInfo(IEnumerable<LocationByIPModel> ipInfo)
        {
            var ipInfoList = ipInfo.Where(t => !string.IsNullOrEmpty(t.CountryCode))
                                    .GroupBy(x => x.IPInfoCode)
                                    .Select(group => new IPInfoModel
                                    {
                                        IPInfoCode = group.Key,
                                        CountryCode = group.First().CountryCode,
                                        Country = group.First().Country,
                                        Region = group.First().Region,
                                        City = group.First().City,
                                        ISP = group.First().ISP
                                    })
                                    .ToList();

            if (ipInfoList != null && ipInfoList.Any())
            {
                var inputJs = JsonConvert.SerializeObject(ipInfoList);
                return dcsExtraService.InsertListIPInfoAsync(inputJs);
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
                            IPInfoCode = ip.IPInfoCode
                        }).ToList();
            }
            return new List<TransactionInitialDataDto>();
        }

        private async Task UpdateTransactionInitialData(IEnumerable<TransactionInitialDataDto> mappingTransID, int batchSize)
        {
            if (mappingTransID != null && mappingTransID.Any())
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
