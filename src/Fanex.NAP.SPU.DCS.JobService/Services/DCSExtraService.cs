using Fanex.NAP.SPU.DCS.Features.DCSExtra;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.Dto;
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
        Task<bool> MigrateDCSExtra();
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

        public async Task<bool> MigrateDCSExtra()
        {
            try
            {
                var parameterID = settingDCSExtra.GetValue<int>("ParameterID");
                var rangeMinutes = settingDCSExtra.GetValue<int>("RangeMinutes");
                var stopMinutes = settingDCSExtra.GetValue<int>("StopMinutes");

                var parameterValue = dcsExtraService.GetParameterValue(parameterID);
                var lastTransTime = DateTime.ParseExact(parameterValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var toTransTime = lastTransTime.AddMinutes(rangeMinutes);
                var stopTime = DateTime.Now.AddMinutes(-stopMinutes);
                if (toTransTime <= stopTime)
                {
                    var mergedList = await GetMergedListAsync(lastTransTime, toTransTime);
                    if (mergedList != null && mergedList.Any())
                    {
                        var rawTransJson = JsonConvert.SerializeObject(mergedList);
                        await dcsExtraService.InsertTransformRawTransAsync(rawTransJson);

                        await dcsExtraService.UpdateParameterValueAsync(parameterID, toTransTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }


        private async Task<IEnumerable<RawTransactionDto>> GetMergedListAsync(DateTime lastTransTime, DateTime toTransTime)
        {
            var tasks = new List<Task<IEnumerable<RawTransactionModel>>>();

            var dcsExtraConfig = settingDCSExtra.GetSection("Config").GetChildren();
            foreach (var item in dcsExtraConfig)
            {
                tasks.Add(dcsExtraService.GetRawTransactionAsync(item["Site"], item["SettingKey"], lastTransTime, toTransTime));
            }

            var listRawTrans = (await Task.WhenAll(tasks)).Where(result => result != null).SelectMany(result => result);
            if (listRawTrans != null && listRawTrans.Any())
            {
                var deviceIDs = listRawTrans
                                .Where(x => x.DeviceID != null)
                                .Select(x => x.DeviceID)
                                .Distinct();

                var deviceIDList = string.Join(",", deviceIDs.Select(x => x.ToString()));
                var deviceInfo = TransformDeviceInfo(dcsExtraService.GetDeviceInfo(deviceIDList));

                var deviceFingerprint = Enumerable.Empty<DeviceFingerprintDto>();
                if (deviceInfo != null && deviceInfo.Any())
                {
                    var deviceNos = deviceInfo
                                    .Where(x => x.DeviceNo != null)
                                    .Select(x => x.DeviceNo)
                                    .Distinct();

                    var deviceNoList = string.Join(",", deviceNos.Select(x => x.ToString()));
                    deviceFingerprint = TransformDeviceFingerprint(dcsExtraService.GetDeviceFingerprint(deviceNoList));
                }

                return MergedList(listRawTrans, deviceInfo, deviceFingerprint);
            }
            return Enumerable.Empty<RawTransactionDto>();
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
    }
}
