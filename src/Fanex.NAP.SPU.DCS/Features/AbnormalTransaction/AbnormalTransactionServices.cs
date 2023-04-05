using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionServices
    {
        Task TransformTrans07ToAbnormalTransaction(int size);

        Task PushAbnormalTransaction(int size, string abnormalTransactionUrl);
    }

    public class AbnormalTransactionServices : IAbnormalTransactionServices
    {
        private readonly IAbnormalTransactionDataAccess _dataAccess;

        public AbnormalTransactionServices(IAbnormalTransactionDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task TransformTrans07ToAbnormalTransaction(int size)
        {
            if (size > 0)
                await _dataAccess.TransformTrans07ToAbnormalTransaction(size);
        }

        public async Task PushAbnormalTransaction(int size, string abnormalTransactionUrl)
        {
            var abnormalTransactions = await _dataAccess.GetAbnormalTransactionsAsync(size);
            if (abnormalTransactions == null || !abnormalTransactions.Any()) 
                return;

            var modelsToPush = TransformEntityToModel(abnormalTransactions);
            await PushAbnormalTransactionsAsync(modelsToPush, abnormalTransactionUrl);

            var modelsToUpdate = abnormalTransactions
                                .Select(t => new AbnormalTransactionUpdateModel { ID = t.ID, Status = 1 })
                                .ToList();
            await _dataAccess.UpdateAbnormalTransactionsAsync(JsonConvert.SerializeObject(modelsToUpdate));
        }

        private IEnumerable<AbnormalTransactionModel> TransformEntityToModel(IEnumerable<AbnormalTransactionEntity> entities)
        {
            if (entities == null || !entities.Any())
            {
                return new List<AbnormalTransactionModel>();
            }

            return entities
                .Select(entity => new AbnormalTransactionModel
                {
                    Browser = entity.Browser,
                    Device = entity.DeviceTypeName,
                    IP = entity.IP,
                    LoginName = entity.LoginName,
                    OS = entity.OS,
                    TransID = entity.TransID,
                    TransTime = entity.TransTime,
                    UserAgent = entity.UserAgent
                })
                .ToList();
        }

        private async Task PushAbnormalTransactionsAsync(IEnumerable<AbnormalTransactionModel> modelsToPush, string abnormalTransactionUrl)
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(modelsToPush);
            var response = await httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Invalid response! Status code: {response.StatusCode}\nUrl: {abnormalTransactionUrl}\nContent: {jsonContent}");
            }
        }
    }
}
