using Fanex.NAP.SPU.DCS.Features.DCSExtra.Model;
using Fanex.NAP.SPU.DCS.Features.Devices;
using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch, bool isTransformExtra);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDeviceServices deviceService;
        private readonly IAppSettings appSettings;

        private const int delayTime = 100; // milliseconds


        private readonly int maxChangesRule5 = 2;
        private readonly int maxChangesRule6 = 1;

        public DeviceJobService(ISentryClient sentryClient, IDeviceServices deviceService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.deviceService = deviceService;
            this.appSettings = appSettings;
        }

        public async Task<bool> Execute(int size, int numberOfBatch, bool isTransformExtra)
        {
            try
            {
                if (size > 0 && numberOfBatch > 0)
                {                    
                    JobRun(size, numberOfBatch, isTransformExtra);    
                }              
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private void JobRun(int size, int numberOfBatch, bool isTransformExtra)
        {
            if (isTransformExtra)
            {
                if(appSettings.IsReadyDbExtra)
                    TransformDevice(size, numberOfBatch, true).Wait();
            }
            else
            {
                TransformDevice(size, numberOfBatch, false).Wait();
            }          
        }

        private async Task TransformDevice(int size, int numberOfBatch, bool isReadyDbExtra)
        {
            var stagingTransIds = await deviceService.GetStagingTransIdForDeviceAsync(size, numberOfBatch, isReadyDbExtra);

            if (stagingTransIds != null && stagingTransIds.Any())
            {
                var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);

                var tasksIdentityDevice = new List<Task<IdentityDevice>>();
                var tasksIdentityDeviceRecover = new List<Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)>>();

                foreach (var trans in groupTrans)
                {
                    if (trans != null && trans.Any())
                    {
                        var taskIdentityDevice = TaskTransformDeviceInsert(isReadyDbExtra, trans);
                        var taskIdentityDeviceRecover = TaskIdentityDeviceRecover(string.Join(",", trans.Select(x => x.TransId)));

                        await Task.Delay(delayTime);

                        tasksIdentityDevice.Add(taskIdentityDevice);
                        tasksIdentityDeviceRecover.Add(taskIdentityDeviceRecover);
                    }
                }

                var identityDevices = (await Task.WhenAll(tasksIdentityDevice)).Where(result => result != null).Select(result => result);

                var identityDeviceRecovers = (await Task.WhenAll(tasksIdentityDeviceRecover))
                                            .Where(result => (result.Item1.Any() || result.Item2.Any()))
                                            .Select(result => result);

                var taskEvolutionDevices = identityDeviceRecovers.Select(x =>
                {
                    return Task.Run(() => EvolutionDeviceAsync(x.Item2));
                });

            }
        }

        private Task<IdentityDevice> TaskTransformDeviceInsert(bool isReadyDbExtra, IGrouping<int, StagingTransId> trans)
        {
            return Task.Run(() =>
            {
                try
                {
                    return deviceService.TransformDeviceInsertAsync(trans, isReadyDbExtra);
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);
                    throw;
                }
            });
        }

        private Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)> TaskIdentityDeviceRecover(string transId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return deviceService.GetIdentityDeviceRecoverAsync(transId);
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);
                    throw;
                }
            });
        }

        private async Task<IEnumerable<Transaction>> EvolutionDeviceAsync(IEnumerable<TransactionEvolution> transactionEvolutions)
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
                    var transJson = JsonConvert.SerializeObject(evolutionModel);
                    return await deviceService.IdentityDeviceInsertAsync(transJson);
                    //transactionService.EvolutionRecoverDevice(transJson);
                }
            }

            return new List<Transaction>();
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
                    var model = new TransactionEvolutionModel()
                    {
                        Id = tran.TransID,
                        RecoverFingerPrintId = recoverFingerPrintId
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

            ulong result = 0;

            if (source != null && !string.IsNullOrEmpty(source.RawFingerPrintIDs) && !string.IsNullOrEmpty(source.BrowserFamilyCode))
            {
                var evolutionAttributes = await deviceService.GetIdentityEvolutionAttributesAsync(source.RawFingerPrintIDs, source.BrowserFamilyCode);

                foreach (TransactionEvolutionAttributes destination in evolutionAttributes)
                {
                    if (CompareRules(source, destination))
                    {
                        result = destination.RawFingerPrintID;
                        break;
                    }
                }
            }

            return result;
        }

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


        #region "Evolution"

        private bool CheckVersion(string browserVersion1, string browserVersion2)
        {
            var version1 = GetVersion(browserVersion1);
            var version2 = GetVersion(browserVersion2);

            return version1 != null &&
                   version2 != null &&
                   version1.CompareTo(version2) >= 0;
        }

        private Version GetVersion(string version)
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

            return values.All(value => value.Source.Equals(value.Destination));
        }

        private bool CheckFonts(string fontsSource, string fontsDestination)
        {
            if (string.IsNullOrEmpty(fontsSource) || string.IsNullOrEmpty(fontsDestination)) return fontsSource == fontsDestination;

            var arr1 = JsonConvert.DeserializeObject<string[]>(fontsSource);
            var arr2 = JsonConvert.DeserializeObject<string[]>(fontsDestination);

            return arr1.All(arr2.Contains) || arr2.All(arr1.Contains);
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

            return true;

        }

        private async Task<bool> CheckRatio(string source, string destination)
        {
            if ((string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination)) && source != destination) return false;

            var content = new StringContent(JsonConvert.SerializeObject(new { source, destination }), System.Text.Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var response = await client.PostAsync(appSettings.RatioUrl, content);

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
