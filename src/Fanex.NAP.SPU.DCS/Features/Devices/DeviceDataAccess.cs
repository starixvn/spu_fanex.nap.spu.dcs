namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.DCS.Features.Devices.Criteria;
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;

    public interface IDeviceDataAccess
    {
        bool IsReadyDbExtra { get; set; }

        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch);

        Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(string transIdJson);

        Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(
            string rawFingerPrintIDs,
            string browserFamilyCode
        );

        Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId);

        Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson);

        Task<int> TransformTransaction07InsertAsync(string transJson);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public bool IsReadyDbExtra { get; set; }

        public DeviceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public async Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch)
        {
            var criteria = new GetDeviceStagingTransactionPackageCriteria
            {
                Size = size,
                NoOfBatch = noOfBatch,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.FetchAsync<StagingTransId>(criteria);
        }

        public async Task<IEnumerable<IdentityDevice>> TransformDeviceInsertAsync(string transIdJson)
        {
            var criteria = new TransformDeviceInsertCriteria
            {
                TransIdJson = transIdJson,
                IsReadyDbExtra = IsReadyDbExtra
            };

            var test = IsReadyDbExtra;

            return await repository.FetchAsync<IdentityDevice>(criteria);
        }

        public async Task<(IEnumerable<TransactionRecover>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            var criteria = new GetIdentityDeviceRecoverCriteria
            {
                TransIds = transIds,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.FetchMultipleAsync<TransactionRecover, TransactionEvolution>(criteria);
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(
            string rawFingerPrintIDs,
            string browserFamilyCode
            )
        {
            var criteria = new GetIdentityEvolutionAttributesCriteria
            {
                RawFingerPrintIDs = rawFingerPrintIDs,
                BrowserFamilyCode = browserFamilyCode,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.FetchAsync<TransactionEvolutionAttributes>(criteria);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityDeviceInsertAsync(string transId)
        {
            var criteria = new IdentityDeviceInsertCriteria
            {
                transIdJson = transId,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<IEnumerable<TransactionRecover>> IdentityEvolutionInsertAsync(string transJson)
        {
            var criteria = new IdentityEvolutionInsertCriteria
            {
                TransJson = transJson,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.FetchAsync<TransactionRecover>(criteria);
        }

        public async Task<int> TransformTransaction07InsertAsync(string transJson)
        {
            var criteria = new TransformTransaction07InsertCommand
            {
                transJson = transJson,
                IsReadyDbExtra = IsReadyDbExtra
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}
