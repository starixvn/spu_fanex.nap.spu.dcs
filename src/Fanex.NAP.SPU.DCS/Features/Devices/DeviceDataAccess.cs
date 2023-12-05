namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;

    public interface IDeviceDataAccess
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(string transIdJson, bool isReadyDbExtra);

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode);

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson);

        Task<int> TransformTransaction07InsertAsync(string transJson);
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

        public async Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(string transIdJson, bool isReadyDbExtra)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIdJson = transIdJson,
                IsReadyDbExtra = isReadyDbExtra
            };
            return await repository.FetchAsync<IdentityDevice>(criteria);
        }

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            var criteria = new GetIdentityDeviceRecoverCriteria
            {
                TransIds = transIds
            };

            return await repository.FetchMultipleAsync<TransactionRecover, TransactionEvolution>(criteria);
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

        public async Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId)
        {
            var criteria = new IdentityDeviceInsertCriteria
            {
                transIdJson = transId
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson)
        {
            var criteria = new IdentityEvolutionInsertCriteria
            {
                transJson = transJson
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<int> TransformTransaction07InsertAsync(string transJson)
        {
            var criteria = new TransformTransaction07InsertCommand
            {
                transJson = transJson
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
