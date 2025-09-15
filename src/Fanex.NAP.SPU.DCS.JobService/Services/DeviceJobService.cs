namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.NAP.SPU.DCS.Features.Devices;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IDeviceJobService
    {
        Task<bool> Execute(int size, int numberOfBatch);

        Task<bool> DeviceEvolution(int batchSize);
    }

    public class DeviceJobService : IDeviceJobService
    {
        private readonly ILogService logService;
        private readonly IDeviceServices deviceService;
        private readonly HttpClient httpClient;
        private readonly IAppSettings appSettings;

        private const int delayTime = 100; // milliseconds

        private readonly int maxChangesRule5 = 2;
        private readonly int maxChangesRule6 = 1;

        public DeviceJobService(ILogService logService, IDeviceServices deviceService, IAppSettings appSettings, HttpClient httpClient)
        {
            this.logService = logService;
            this.deviceService = deviceService;
            this.appSettings = appSettings;
            this.httpClient = httpClient;

            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<bool> Execute(int size, int numberOfBatch)
        {
            try
            {
                if (size <= 0 || numberOfBatch <= 0)
                {
                    await logService.ErrorAsync(new Exception("TransformDevice BatchSize = 0!"));
                    return false;
                }

                var transformResult = await TransformDevice(size, numberOfBatch);

                if (transformResult)
                {
                    var updateResult = deviceService.UpdateMaxTransIDTransformDevice();
                    if (updateResult < 0)
                    {
                        await logService.ErrorAsync(new Exception($"UpdateMaxTransIDTransformDevice return value: {updateResult}!"));

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);

                return false;
            }

            return true;
        }

        public async Task<bool> DeviceEvolution(int batchSize)
        {
            try
            {
                if (batchSize <= 0)
                {
                    await logService.ErrorAsync(new Exception("DeviceEvolution BatchSize = 0!"));
                    return await Task.FromResult(false);
                }

                var resultDeviceEvolution = await deviceService.GetFPDeviceEvolutionAsync(batchSize);
                var dataEvolution = resultDeviceEvolution.Item1;
                var dataEvolutionAttribute = resultDeviceEvolution.Item2;

                if (dataEvolution != null && dataEvolution.Any())
                {
                    var listInsert = await CreateDeviceEvolutionList(dataEvolution, dataEvolutionAttribute);
                    if (listInsert.Any())
                    {
                        var deviceEvolutionJson = JsonSerializer.Serialize(listInsert);
                        await deviceService.InsertFPDeviceEvolutionAsync(deviceEvolutionJson);
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

        private async Task<List<InsertDeviceEvolutionEntity>> CreateDeviceEvolutionList(IEnumerable<DeviceEvolutionEntity> dataEvolution, IEnumerable<DeviceEvolutionEntity> dataEvolutionAttribute)
        {
            var listInsert = new List<InsertDeviceEvolutionEntity>();
            var listCandidate = new List<DeviceEvolutionEntity>();

            foreach (var itemEvolution in dataEvolution)
            {
                var itemEntity = new InsertDeviceEvolutionEntity
                {
                    FPDeviceFingerprintAccountID = itemEvolution.FPDeviceFingerprintAccountID,
                    EvolveByID = await FindMatchingCandidate(itemEvolution, listCandidate)
                };

                if (itemEntity.EvolveByID == 0 && !string.IsNullOrEmpty(itemEvolution.FPDeviceFingerprintAccountIDRef))
                {
                    itemEntity.EvolveByID = await FindMatchingReference(itemEvolution, dataEvolutionAttribute);
                }

                listInsert.Add(itemEntity);
                listCandidate.Add(itemEvolution);
            }

            return listInsert;
        }

        private async Task<ulong> FindMatchingCandidate(DeviceEvolutionEntity itemEvolution, List<DeviceEvolutionEntity> listCandidate)
        {
            var filteredByAccount = listCandidate
                .Where(candidate => candidate.AccountID == itemEvolution.AccountID)
                .OrderByDescending(candidate => candidate.FPDeviceFingerprintAccountID)
                .ToList();

            foreach (var destination in filteredByAccount)
            {
                if (await CompareRules(itemEvolution, destination))
                {
                    return destination.FPDeviceFingerprintAccountID;
                }
            }

            return 0;
        }

        private async Task<ulong> FindMatchingReference(DeviceEvolutionEntity itemEvolution, IEnumerable<DeviceEvolutionEntity> dataEvolutionAttribute)
        {
            var refIds = itemEvolution.FPDeviceFingerprintAccountIDRef
                                    .Split(',')
                                    .Select(ulong.Parse)
                                    .ToList();

            var matchingRecords = dataEvolutionAttribute
                                .Where(attrEntity => refIds.Contains(attrEntity.FPDeviceFingerprintAccountID))
                                .ToList();

            foreach (var destination in matchingRecords)
            {
                if (await CompareRules(itemEvolution, destination))
                {
                    return destination.FPDeviceFingerprintAccountID;
                }
            }

            return 0;
        }

        private async Task<bool> TransformDevice(int size, int numberOfBatch)
        {
            var stagingTransIds = await deviceService.GetStagingTransIdForDeviceAsync(size, numberOfBatch);
            if (stagingTransIds == null || !stagingTransIds.Any())
            {
                return false;
            }

            var groupTrans = stagingTransIds.GroupBy(x => x.BatchId);
            var tasks = new List<Task>();

            foreach (var trans in groupTrans)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await deviceService.TransformDeviceInsertAsync(trans);
                    }
                    catch (Exception ex)
                    {
                        await logService.ErrorAsync(ex);
                        throw;
                    }
                });

                await Task.Delay(delayTime);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());

            return true;
        }



        #region "Evolution"

#pragma warning disable S121 // Add curly braces around the nested statement(s) in this 'if' block.
#pragma warning disable S122 // Reformat the code to have only one statement per line.
        private async Task<bool> CompareRules(DeviceEvolutionEntity source, DeviceEvolutionEntity destination)
        {
            // Rule 2
            if (!CheckVersion(source.BrowserVersion, destination.BrowserVersion)) return false;

            // Rule 3
            if (!CheckForbiddenChanges(source, destination)) return false;

            // Rule 4
            if (!CheckFonts(source.Fonts, destination.Fonts)) return false;

            // Rule 5
            int allowChangeWithSimCounter = 0;
            (var result, allowChangeWithSimCounter) = await CheckAllowChangesWithSimilar(source, destination, allowChangeWithSimCounter);   
            if (!result) return false;

            // Rule 6
            int allowChangeCounter = 0;
            if (!CheckAllowChanges(source, destination, ref allowChangeCounter)) return false;

            // Rule 7
            if (allowChangeWithSimCounter != 0 && allowChangeCounter != 0) return false;

            return true;
        }
#pragma warning disable S121 // Add curly braces around the nested statement(s) in this 'if' block.
#pragma warning disable S122 // Reformat the code to have only one statement per line.

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
            if (string.IsNullOrEmpty(version)) return null;

            if (ValidateVersion(version))
            {
                return Version.Parse(version);
            }
            else
            {
                int dashIndex = version.IndexOf("-", StringComparison.Ordinal);
                if (dashIndex > -1)
                    version = version.Substring(0, dashIndex);

                int parenthesisIndex = version.IndexOf("(", StringComparison.Ordinal);
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

        private static bool CheckForbiddenChanges(DeviceEvolutionEntity source, DeviceEvolutionEntity destination)
        {
            var values = new List<ValuePair>
            {
                new() { Source = source.LocalStorage, Destination = destination.LocalStorage },
                new() { Source = source.DoNotTrack, Destination = destination.DoNotTrack },
                new() { Source = source.Cookies, Destination = destination.Cookies },
                new() { Source = source.Canvas, Destination = destination.Canvas }
            };

            return values.TrueForAll(value => Object.Equals(value.Source, value.Destination));
        }

        private static bool CheckFonts(string fontsSource, string fontsDestination)
        {
            if (string.IsNullOrEmpty(fontsSource) || string.IsNullOrEmpty(fontsDestination))
            {
                return fontsSource == fontsDestination;
            }

            if (fontsSource == "error" || fontsSource == "\"error\""
                || fontsDestination == "error" || fontsDestination == "\"error\"")
            {
                return false;
            }

            var arrSrc = JsonSerializer.Deserialize<string[]>(fontsSource);
            var arrDestination = JsonSerializer.Deserialize<string[]>(fontsDestination);

            return Array.TrueForAll(arrSrc, element => arrDestination.Contains(element))
                 || Array.TrueForAll(arrDestination, element => arrSrc.Contains(element));
        }

        private async Task<(bool, int)> CheckAllowChangesWithSimilar(DeviceEvolutionEntity source, DeviceEvolutionEntity destination, int initialCounter)
        {
            var values = new List<ValuePair>
            {
                new() { Source = source.UserAgent, Destination = destination.UserAgent },
                new() { Source = source.Vendor, Destination = destination.Vendor },
                new() { Source = source.GLRenderer, Destination = destination.GLRenderer },
                new() { Source = source.Plugins, Destination = destination.Plugins },
                new() { Source = source.Languages, Destination = destination.Languages },
                new() { Source = source.Accept, Destination = destination.Accept },
                new() { Source = source.Headers, Destination = destination.Headers }
            };

            int counter = initialCounter;

            try
            {
                foreach (var value in values.Where(v => !v.Source.Equals(v.Destination)))
                {
                    var ratioResult = await CheckRatio(value.Source.ToString(), value.Destination.ToString());

                    if (!ratioResult) return (false, counter);

                    counter++;

                    if (counter > maxChangesRule5) return (false, counter);
                }
            }
            catch (Exception ex)
            {
                await logService.ErrorAsync(ex);
                return (false, counter);
            }

            return (true, counter);
        }

        private async Task<bool> CheckRatio(string source, string destination)
        {
            const int minRatio = 75;
            if ((string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination)) && source != destination) return false;

            using StringContent content = new StringContent(JsonSerializer.Serialize(new { source, destination }), System.Text.Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await httpClient.PostAsync(new Uri(appSettings.RatioUrl), content);

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (decimal.TryParse(responseContent, out var ratio) && ratio < minRatio)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckAllowChanges(DeviceEvolutionEntity source, DeviceEvolutionEntity destination, ref int counter)
        {
            var values = new List<ValuePair>
            {
                new() { Source = source.Resolution, Destination = destination.Resolution },
                new() { Source = source.Timezone, Destination = destination.Timezone },
                new() { Source = source.Encoding, Destination = destination.Encoding }
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

        struct ValuePair : IEquatable<ValuePair>
        {
            public object Source;
            public object Destination;

            public bool Equals(ValuePair other) => Equals(Source, other.Source) && Equals(Destination, other.Destination);
        }

        #endregion "Evolution"
    }
}
