namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;

    public interface IDeviceServices
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<IdentityDevice> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra);

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode);

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(IEnumerable<IdentifyEvolutionModel> models);

        Task<int> TransformTransaction07InsertAsync(IEnumerable<TransformTransactionModel> transformTransactionModels);
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
            if(size != 0 && noOfBatch != 0)
            {
                return await deviceDataAcess.GetStagingTransIdForDeviceAsync(size, noOfBatch, isReadyDbExtra);
            }

            return new List<StagingTransId>();
        }

        public async Task<IdentityDevice> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra)
        {
            var transIdJson = JsonSerializer.Serialize(stagingTransId);
            return await deviceDataAcess.TransformDeviceInsertAsync(transIdJson, isReadyDbExtra);
        }

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            if(!string.IsNullOrEmpty(transIds)) return await deviceDataAcess.GetIdentityDeviceRecoverAsync(transIds);

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
            var transJson = JsonSerializer.Serialize(transformTransactionModels);
            return await deviceDataAcess.TransformTransaction07InsertAsync(transJson);
        }
    }
}
