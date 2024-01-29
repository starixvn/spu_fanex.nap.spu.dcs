namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;

    public interface IDeviceServices
    {

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra);

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds, bool isReadyDbExtra);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode, bool isReadyDbExtra);

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId, bool isReadyDbExtra);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(IEnumerable<IdentifyEvolutionModel> models, bool isReadyDbExtra);

        Task<int> TransformTransaction07InsertAsync(IEnumerable<TransformTransactionModel> transformTransactionModels, bool isReadyDbExtra);
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra)
        {
            if (size != 0 && noOfBatch != 0)
            {
                return await deviceDataAcess.GetStagingTransIdForDeviceAsync(size, noOfBatch, isReadyDbExtra);
            }

            return new List<StagingTransId>();
        }

        public async Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra)
        {
            var transIdJson = JsonSerializer.Serialize(stagingTransId);
            return await deviceDataAcess.TransformDeviceInsertAsync(transIdJson, isReadyDbExtra);
        }

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds, bool isReadyDbExtra)
        {
            if (!string.IsNullOrEmpty(transIds))
            {
                return await deviceDataAcess.GetIdentityDeviceRecoverAsync(transIds, isReadyDbExtra);
            }

            return (new List<TransactionRecover>(), new List<TransactionEvolution>());
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode, bool isReadyDbExtra)
        {
            return await deviceDataAcess.GetIdentityEvolutionAttributesAsync(rawFingerPrintIDs, browserFamilyCode, isReadyDbExtra);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId, bool isReadyDbExtra)
        {
            return await deviceDataAcess.IdentityDeviceInsertAsync(transId, isReadyDbExtra);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(IEnumerable<IdentifyEvolutionModel> models, bool isReadyDbExtra)
        {
            var transJson = JsonSerializer.Serialize(models);
            return await deviceDataAcess.IdentityEvolutionInsertAsync(transJson, isReadyDbExtra);
        }

        public async Task<int> TransformTransaction07InsertAsync(IEnumerable<TransformTransactionModel> transformTransactionModels, bool isReadyDbExtra)
        {
            if (transformTransactionModels != null && transformTransactionModels.Any())
            {
                var transJson = JsonSerializer.Serialize(transformTransactionModels);
                return await deviceDataAcess.TransformTransaction07InsertAsync(transJson, isReadyDbExtra);
            }

            return default;
        }
    }
}
