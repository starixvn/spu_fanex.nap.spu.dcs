namespace Fanex.NAP.SPU.DCS.Features.Devices
{
    using Fanex.NAP.SPU.DCS.Features.Devices.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDeviceServices
    {
        Task<IEnumerable<StagingTransId>> GetStagingTransIdForDeviceAsync(int size, int noOfBatch, bool isReadyDbExtra);

        Task<IdentityDevice> TransformDeviceInsertAsync(IEnumerable<StagingTransId> stagingTransId, bool isReadyDbExtra);

        Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds);

        Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode);

        Task<IEnumerable<Transaction>> IdentityDeviceInsertAsync(string transId);
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
            return await deviceDataAcess.TransformDeviceInsertAsync(stagingTransId, isReadyDbExtra);
        }

        public async Task<(IEnumerable<Transaction>, IEnumerable<TransactionEvolution>)> GetIdentityDeviceRecoverAsync(string transIds)
        {
            if(!string.IsNullOrEmpty(transIds)) return await deviceDataAcess.GetIdentityDeviceRecoverAsync(transIds);

            return (new List<Transaction>(), new List<TransactionEvolution>());
        }

        public async Task<IEnumerable<TransactionEvolutionAttributes>> GetIdentityEvolutionAttributesAsync(string rawFingerPrintIDs, string browserFamilyCode)
        {
            return await deviceDataAcess.GetIdentityEvolutionAttributesAsync(rawFingerPrintIDs, browserFamilyCode);
        }

        public async Task<IEnumerable<Transaction>> IdentityDeviceInsertAsync(string transId)
        {
            return await deviceDataAcess.IdentityDeviceInsertAsync(transId);
        }
    }
}
