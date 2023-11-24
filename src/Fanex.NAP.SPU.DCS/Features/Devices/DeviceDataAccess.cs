namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceDataAccess
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<IdentityDevice> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra);

        Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode);

        Task<IEnumerable<Transaction>> IdentityDeviceInsertAsync(string transId);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DeviceDataAccess()
        {
            repository = new DynamicRepository();
        }


        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<StagingTransId>(criteria);
        }

        public async Task<IdentityDevice> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra)
        {
            var transIdJson = JsonConvert.SerializeObject(stagingTransId);

            var criteria = new TransformDeviceInsertCriteria
            {
                TransIdJson = transIdJson,
                IsReadyDbExtra = isReadyDbExtra
            };
            return await repository.GetAsync<IdentityDevice>(criteria);
        }

        public async Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            var criteria = new GetIdentityDeviceRecoverCriteria
            {
               TransIds = transIds
            };

            return await repository.FetchMultipleAsync<Transaction, TransactionEvolution>(criteria);
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode)
        {
            var criteria = new GetIdentityEvolutionAttributesCriteria
            {
                RawFingerPrintIDs = rawFingerPrintIDs,
                BrowserFamilyCode = browserFamilyCode
            };

            return await repository.FetchAsync<TransactionEvolutionAttributes>(criteria);
        }

        public async Task<IEnumerable<Transaction>> IdentityDeviceInsertAsync(string transId)
        {
            var criteria = new IdentityDeviceInsertCriteria
            {
                transIdJson = transId
            };

            return await repository.FetchAsync<Transaction>(criteria);
        }

    }
}
