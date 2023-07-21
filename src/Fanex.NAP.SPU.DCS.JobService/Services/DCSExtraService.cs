using Fanex.NAP.SPU.DCS.Features.DCSExtra;
using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService.Models.Dto;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
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

        private readonly string[] subscriberNameS = { "CTMax", "Velki", "Velki1" };
        private readonly int rangeMinutes = 5;
        private readonly int parameterID = 1000;

        public DCSExtraService(IDCSExtraServices dcsExtraService, ISentryClient sentryClient)
        {
            this.dcsExtraService = dcsExtraService;
            this.sentryClient = sentryClient;
        }

        public async Task<bool> MigrateDCSExtra()
        {
            try
            {
                var parameterValue = dcsExtraService.GetParameterValue(parameterID);
                var lastTransTime = DateTime.ParseExact(parameterValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var toTransTime = lastTransTime.AddMinutes(rangeMinutes);
                var isError = false;

                var tasks = subscriberNameS.Select(subscriberName => dcsExtraService.GetRawTransactionAsync(subscriberName, lastTransTime, toTransTime)).ToList();

                if (!isError)
                {
                    var listRawTrans = (await Task.WhenAll(tasks)).Where(result => result != null).SelectMany(result => result).ToList();

                    if (listRawTrans != null && listRawTrans.Any())
                    {
                        var deviceInfo = Enumerable.Empty<DeviceInfoDto>();
                        var deviceFingerprint = Enumerable.Empty<DeviceFingerprintDto>();

                        var deviceIDs = listRawTrans.Select(x => x.DeviceID).Distinct();
                        var deviceIDList = string.Join(",", deviceIDs.Select(x => x.ToString()));
                        deviceInfo = TransformDevicInfo(dcsExtraService.GetDeviceInfo(deviceIDList));

                        if (deviceInfo != null && deviceInfo.Any())
                        {
                            var deviceNos = deviceInfo.Select(x => x.DeviceNo).Distinct();
                            var deviceNoList = string.Join(",", deviceNos.Select(x => x.ToString()));
                            deviceFingerprint = TransformDeviceFingerprint(dcsExtraService.GetDeviceFingerprint(deviceNoList));
                        }

                        List<RawTransactionDto> mergedList = listRawTrans
                        .GroupJoin(deviceInfo, a => a.DeviceID, b => b.DeviceID, (a, bGroup) => new { a, bGroup })
                        .SelectMany(ab => ab.bGroup.DefaultIfEmpty(), (a, b) => new { a.a, b })
                        .GroupJoin(deviceFingerprint, ab => ab.b?.DeviceNo ?? "", c => c.DeviceNo, (ab, cGroup) => new { ab.a, cGroup })
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
                            DeviceCode = c.DeviceNo,
                            FingerprintCode = c.FingerprintCode
                        })
                        .ToList();

                        if (mergedList != null && mergedList.Any())
                        {
                            var rawTransJson = JsonConvert.SerializeObject(mergedList);
                            await dcsExtraService.InsertTransformRawTransAsync(rawTransJson);

                            // 2019-05-07 00:14:11
                            await dcsExtraService.UpdateParameterValueAsync(parameterID, toTransTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }

        private IEnumerable<DeviceInfoDto> TransformDevicInfo(IEnumerable<DeviceInfoModel> model)
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

        private string MappingFlaggedCode(int flagged)
        {
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

            return string.Empty;
        }
    }
}
