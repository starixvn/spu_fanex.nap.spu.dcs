namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;

    public interface IDeviceServices
    {
        bool IsReadyDbExtra { get; set; }

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId);

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode);

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(IEnumerable<IdentifyEvolutionModel> models);

        Task<int> TransformTransaction07InsertAsync(IEnumerable<TransformTransactionModel> transformTransactionModels);
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAcess;
        public bool IsReadyDbExtra { get; set; }

        public DeviceServices(IDeviceDataAccess deviceDataAcess)
        {
            this.deviceDataAcess = deviceDataAcess;
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch)
        {
            if (size != 0 && noOfBatch != 0)
            {
                deviceDataAcess.IsReadyDbExtra = IsReadyDbExtra;

                return await deviceDataAcess.GetStagingTransIdForDeviceAsync(size, noOfBatch);
            }

            return new List<StagingTransId>();
        }

        public async Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId)
        {
            var transIdJson = JsonSerializer.Serialize(stagingTransId);
            return await deviceDataAcess.TransformDeviceInsertAsync(transIdJson);
        }

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            if (!string.IsNullOrEmpty(transIds))
            {
                return await deviceDataAcess.GetIdentityDeviceRecoverAsync(transIds);
            }

            return (new List<TransactionRecover>(), new List<TransactionEvolution>());
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode)
        {
            return await deviceDataAcess.GetIdentityEvolutionAttributesAsync(rawFingerPrintIDs, browserFamilyCode);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId)
        {
            return await deviceDataAcess.IdentityDeviceInsertAsync(transId);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(IEnumerable<IdentifyEvolutionModel> models)
        {
            var transJson = JsonSerializer.Serialize(models);
            return await deviceDataAcess.IdentityEvolutionInsertAsync(transJson);
        }

        public async Task<int> TransformTransaction07InsertAsync(IEnumerable<TransformTransactionModel> transformTransactionModels)
        {
            if (transformTransactionModels != null && transformTransactionModels.Any())
            {
                var transJson = JsonSerializer.Serialize(transformTransactionModels);
                return await deviceDataAcess.TransformTransaction07InsertAsync(transJson);
            }

            return default;
        }
    }
}
