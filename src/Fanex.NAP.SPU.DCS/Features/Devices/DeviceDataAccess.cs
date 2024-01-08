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

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds, bool isReadyDbExtra);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode, bool isReadyDbExtra);

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId, bool isReadyDbExtra);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson, bool isReadyDbExtra);

        Task<int> TransformTransaction07InsertAsync(string transJson, bool isReadyDbExtra);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DeviceDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
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

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds, bool isReadyDbExtra)
        {
            var criteria = new GetIdentityDeviceRecoverCriteria
            {
                TransIds = transIds,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchMultipleAsync<TransactionRecover, TransactionEvolution>(criteria);
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(
            string rawFingerPrintIDs,
            string browserFamilyCode,
            bool isReadyDbExtra
            )
        {
            var criteria = new GetIdentityEvolutionAttributesCriteria
            {
                RawFingerPrintIDs = rawFingerPrintIDs,
                BrowserFamilyCode = browserFamilyCode,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<TransactionEvolutionAttributes>(criteria);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId, bool isReadyDbExtra)
        {
            var criteria = new IdentityDeviceInsertCriteria
            {
                transIdJson = transId,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson, bool isReadyDbExtra)
        {
            var criteria = new IdentityEvolutionInsertCriteria
            {
                TransJson = transJson,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<int> TransformTransaction07InsertAsync(string transJson, bool isReadyDbExtra)
        {
            var criteria = new TransformTransaction07InsertCommand
            {
                transJson = transJson,
                IsReadyDbExtra = isReadyDbExtra
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
