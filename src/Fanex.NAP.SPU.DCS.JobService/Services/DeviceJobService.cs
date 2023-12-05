namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Sentry;

    public interface IDeviceJobService
    {
        bool IsReadyDbExtra { get; set; }

        Task<bool> Execute(int size, int numberOfBatch);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDeviceServices deviceService;
        private readonly IAppSettings appSettings;

        public bool IsReadyDbExtra { get; set; }

        private const int delayTime = 100; // milliseconds

        private readonly int maxChangesRule5 = 2;
        private readonly int maxChangesRule6 = 1;

        public DeviceJobService(ISentryClient sentryClient, IDeviceServices deviceService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.deviceService = deviceService;
            this.appSettings = appSettings;

            this.deviceService.IsReadyDbExtra = IsReadyDbExtra && appSettings.IsReadyDbExtra;
        }

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {
                    await TransformDevice(size, numberOfBatch);
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private async Task TransformDevice(int size, int numberOfBatch)
        {
            var stagingTransIds = await deviceService.GetStagingTransIdForDeviceAsync(size, numberOfBatch);

            if (stagingTransIds != null && stagingTransIds.Any())
            {
                var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);

                var taskDevices = TaskDeviceIdentity(groupTrans);
                var taskRecovers = TaskRecoveringDeviceByFPAndCookie(groupTrans);

                await Task.WhenAll(taskDevices, taskRecovers);

                var identityDevices = (await taskDevices).Where(result => result != null);
                var evolutionDevices = (await taskRecovers).Where(result => result != null);

                if (identityDevices.Any())
                {
                    await InsertTransfromTransaction(identityDevices, evolutionDevices);
                }
            }
        }

        private async Task<IEnumerable<IdentityDevice>> TaskDeviceIdentity(IEnumerable<IGrouping<int, StagingTransId>> groupTrans)
        {
            var taskDevices = new List<Task<IEnumerable<IdentityDevice>>>();

            foreach (var trans in groupTrans)
            {
                var taskDevice = Task.Run(() =>
                {
                    try
                    {
                        return deviceService.TransformDeviceInsertAsync(trans);
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                taskDevices.Add(taskDevice);
            }

            return (await Task.WhenAll(taskDevices)).Where(result => result != null).SelectMany(result => result);
        }

        private async Task<IEnumerable<TransactionRecover>> TaskRecoveringDeviceByFPAndCookie(IEnumerable<IGrouping<int, StagingTransId>> groupTrans)
        {
            var taskRecovers = new List<Task<IEnumerable<TransactionRecover>>>();

            foreach (var trans in groupTrans)
            {
                var taskRecover = Task.Run(() =>
                {
                    try
                    {
                        return ProcessEvolution(string.Join(",", trans.Select(x => x.TransId)));
                    }
                    catch (Exception ex)
                    {
                        sentryClient.CaptureException(ex);
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                taskRecovers.Add(taskRecover);
            }


            var result = (await Task.WhenAll(taskRecovers)).Where(result => result != null).SelectMany(result => result);
            return result;
        }

        private async Task<IEnumerable<TransactionRecover>> ProcessEvolution(string transIds)
        {
            var (transactionRecovers, transactionEvolutions) = await deviceService.GetIdentityDeviceRecoverAsync(transIds);

            var evolutionDevices = await EvolutionDeviceAsync(transactionEvolutions);

            return evolutionDevices.Concat(transactionRecovers);
        }


        private async Task<int> InsertTransfromTransaction(IEnumerable<IdentityDevice> identityDevices, IEnumerable<TransactionRecover> evolutionDevices)
        {
            var transformTransactions = identityDevices.Select(identityDevice => new TransformTransactionModel
            {
                TransID = identityDevice.TransID,
                DeviceID = identityDevice.DeviceID,
                FPDeviceID = evolutionDevices.FirstOrDefault(e => e.TransID == identityDevice.TransID)?.FPDeviceID,
                RecoverType = evolutionDevices.FirstOrDefault(e => e.TransID == identityDevice.TransID)?.RecoverType,
                DeviceCodeID = identityDevice.DeviceCodeID,
                FirstDeviceCode = identityDevice.FirstDeviceCode,
                DeviceStatus = identityDevice.DeviceStatus,
                DeviceFingerprintID = identityDevice.DeviceFingerprintID
            });

            return await deviceService.TransformTransaction07InsertAsync(transformTransactions);
        }

        private async Task<IEnumerable<TransactionRecover>> EvolutionDeviceAsync(IEnumerable<TransactionEvolution> transactionEvolutions)
        {
            if (transactionEvolutions.Any())
            {
                var (newDeviceTransIds, evolutionModel) = await ClassifyRecoverTransAsync(transactionEvolutions);

                // NewDevice
                if (newDeviceTransIds != null && newDeviceTransIds.Any())
                {
                    var transIds = string.Join(",", newDeviceTransIds);
                    return await deviceService.IdentityDeviceInsertAsync(transIds);
                }

                // FPS Evolution
                if (evolutionModel != null && evolutionModel.Any())
                {
                    var identityEvolutions = evolutionModel.Select(x =>
                                            new IdentifyEvolutionModel
                                            {
                                                TransID = x.ID,
                                                RecoverRawFingerPrintID = x.RecoverFingerPrintID
                                            });

                    return await deviceService.IdentityEvolutionInsertAsync(identityEvolutions);
                }
            }

            return new List<TransactionRecover>();
        }

        private async Task<(IEnumerable<ulong>, IEnumerable<TransactionEvolutionModel>)> ClassifyRecoverTransAsync(IEnumerable<TransactionEvolution> trans)
        {
            var newDeviceTransIds = new List<ulong>();
            var transRecover = new List<TransactionEvolutionModel>();

            foreach (var tran in trans)
            {
                var recoverFingerPrintId = await GetRecoverFingerPrintIdByEvolutionAsync(tran);

                if (recoverFingerPrintId > 0)
                {
                    var model = new TransactionEvolutionModel
                    {
                        ID = tran.TransID,
                        RecoverFingerPrintID = recoverFingerPrintId
                    };

                    transRecover.Add(model);
                }
                else
                {
                    newDeviceTransIds.Add(tran.TransID);
                }
            }

            return (newDeviceTransIds, transRecover);
        }

        private async Task<ulong> GetRecoverFingerPrintIdByEvolutionAsync(TransactionEvolution source)
        {
            if (source != null && !string.IsNullOrEmpty(source.RawFingerPrintIDs) && !string.IsNullOrEmpty(source.BrowserFamilyCode))
            {
                var evolutionAttributes = await deviceService.GetIdentityEvolutionAttributesAsync(source.RawFingerPrintIDs, source.BrowserFamilyCode);

                TransactionEvolutionAttributes result = evolutionAttributes.FirstOrDefault(destination => CompareRules(source, destination));

                if (result != null)
                {
                    return result.RawFingerPrintID;
                }
            }

            return 0;
        }

#pragma warning disable S121 // Add curly braces around the nested statement(s) in this 'if' block.
        private bool CompareRules(TransactionEvolutionAttributes source, TransactionEvolutionAttributes destination)
        {
            // Rule 2
            if (!CheckVersion(source.BrowserVersion, destination.BrowserVersion)) return false;

            // Rule 3
            if (!CheckForbiddenChanges(source, destination)) return false;

            // Rule 4
            if (!CheckFonts(source.Fonts, destination.Fonts)) return false;

            // Rule 5
            int allowChangeWithSimCounter = 0;
            if (!CheckAllowChangesWithSimilar(source, destination, ref allowChangeWithSimCounter)) return false;

            // Rule 6
            int allowChangeCounter = 0;
            if (!CheckAllowChanges(source, destination, ref allowChangeCounter)) return false;

            // Rule 7
            if (allowChangeWithSimCounter != 0 && allowChangeCounter != 0) return false;

            return true;
        }
#pragma warning disable S121 // Add curly braces around the nested statement(s) in this 'if' block.

        #region "Evolution"

        private static bool CheckVersion(string browserVersionSrc, string browserVersionDestination)
        {
            var versionSrc = GetVersion(browserVersionSrc);
            var versionDestination = GetVersion(browserVersionDestination);

            return versionSrc != null &&
                   versionDestination != null &&
                   versionSrc.CompareTo(versionDestination) >= 0;
        }

        private static Version GetVersion(string version)
        {
            if (ValidateVersion(version))
            {
                return Version.Parse(version);
            }
            else
            {
                int dashIndex = version.IndexOf("-");
                if (dashIndex > -1)
                    version = version.Substring(0, dashIndex);

                int parenthesisIndex = version.IndexOf("(");
                if (parenthesisIndex > -1)
                    version = version.Substring(0, parenthesisIndex);

                version += ".0.0.0";
                string[] versionArray = version.Split(".");
                if (versionArray.Length > 4)
                {
                    version = string.Concat(versionArray[0], ".", versionArray[1], ".", versionArray[2], ".", versionArray[3]);
                }

                if (ValidateVersion(version))
                {
                    return Version.Parse(version);
                }
            }
            return null;
        }

        static bool ValidateVersion(string version) => Version.TryParse(version, out _);

        private bool CheckForbiddenChanges(TransactionEvolutionAttributes source, TransactionEvolutionAttributes destination)
        {
            var values = new List<ValuePair>
            {
                new ValuePair { Source = source.LocalStorage, Destination = destination.LocalStorage },
                new ValuePair { Source = source.DoNotTrack, Destination = destination.DoNotTrack },
                new ValuePair { Source = source.Cookies, Destination = destination.Cookies },
                new ValuePair { Source = source.Canvas, Destination = destination.Canvas }
            };

            return values.TrueForAll(value => value.Source.Equals(value.Destination));
        }

        private static bool CheckFonts(string fontsSource, string fontsDestination)
        {
            if (string.IsNullOrEmpty(fontsSource) || string.IsNullOrEmpty(fontsDestination))
            {
                return fontsSource == fontsDestination;
            }

            var arrSrc = JsonSerializer.Deserialize<string[]>(fontsSource);
            var arrDestination = JsonSerializer.Deserialize<string[]>(fontsDestination);

            return Array.TrueForAll(arrSrc, element => arrDestination.Contains(element))
                 || Array.TrueForAll(arrDestination, element => arrSrc.Contains(element));
        }

        private bool CheckAllowChangesWithSimilar(TransactionEvolutionAttributes source, TransactionEvolutionAttributes destination, ref int counter)
        {
            var values = new List<ValuePair>
            {
                new ValuePair { Source = source.UserAgent, Destination = destination.UserAgent },
                new ValuePair { Source = source.Vendor, Destination = destination.Vendor },
                new ValuePair { Source = source.GLRenderer, Destination = destination.GLRenderer },
                new ValuePair { Source = source.Plugins, Destination = destination.Plugins },
                new ValuePair { Source = source.Languages, Destination = destination.Languages }
            };

            try
            {
                counter = 0;
                foreach (var value in values)
                {
                    if (!value.Source.Equals(value.Destination))
                    {
                        var ratioResult = CheckRatio(value.Source.ToString(), value.Destination.ToString()).Result;

                        if (!ratioResult) return false;

                        counter++;
                    }

                    if (counter > maxChangesRule5)
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

        private async Task<bool> CheckRatio(string source, string destination)
        {
            if ((string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination)) && source != destination) return false;

            var content = new StringContent(JsonSerializer.Serialize(new { source, destination }), System.Text.Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var response = await client.PostAsync(new System.Uri(appSettings.RatioUrl), content);

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (decimal.TryParse(responseContent, out var ratio) && ratio < 75)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckAllowChanges(TransactionEvolutionAttributes source, TransactionEvolutionAttributes destination, ref int counter)
        {
            var values = new List<ValuePair>
            {
                new ValuePair { Source = source.Resolution, Destination = destination.Resolution },
                new ValuePair { Source = source.Timezone, Destination = destination.Timezone },
                new ValuePair { Source = source.GLRenderer, Destination = destination.GLRenderer }
            };

            counter = 0;
            foreach (var value in values)
            {
                if (!value.Source.Equals(value.Destination))
                {
                    counter++;
                }

                if (counter > maxChangesRule6) return false;
            }

            return true;
        }

        struct ValuePair
        {
            public object Source;
            public object Destination;
        }

        #endregion "Evolution"
    }
}
