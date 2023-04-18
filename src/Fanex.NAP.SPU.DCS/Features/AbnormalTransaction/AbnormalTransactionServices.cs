using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionServices
    {

        void PushAbnormalTransaction(int size, string abnormalTransactionUrl);
    }

    public class AbnormalTransactionServices : IAbnormalTransactionServices
    {
        private readonly IAbnormalTransactionDataAccess _dataAccess;

        public AbnormalTransactionServices(IAbnormalTransactionDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void PushAbnormalTransaction(int size, string abnormalTransactionUrl)
        {
            var (abnormalTransactions, transIDMinMax) = _dataAccess.GetAbnormalTransactionsAsync(size);

            ulong minID = transIDMinMax?.FirstOrDefault()?.MinID ?? 0;
            ulong maxID = transIDMinMax?.FirstOrDefault()?.MaxID ?? 0;

            if (abnormalTransactions.Any())
            {
                var dataTransformed = TransformData(abnormalTransactions);

                var isResult = PushData(dataTransformed, abnormalTransactionUrl);

                if (isResult)
                {
                    var updateStatus = abnormalTransactions.Select(t => new AbnormalTransactionUpdateModel { AssociationID = t.AssociationID, Status = 1 });
                    var jsonTrans = JsonConvert.SerializeObject(updateStatus);

                    _dataAccess.UpdateAbnormalTransactionsAsync(jsonTrans, minID, maxID);
                }
            }
            else
            {
                //Update MaxId
                if (maxID > 0)
                    _dataAccess.UpdateAbnormalTransactionsAsync(string.Empty, minID, maxID);
            }
        }

        private IEnumerable<AbnormalTransactionModel> TransformData(IEnumerable<AbnormalTransactionEntity> entities)
        {
            if (entities.Any())
            {
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
                    });
            }

            return new List<AbnormalTransactionModel>();
        }

        private bool PushData(IEnumerable<AbnormalTransactionModel> modelsToPush, string abnormalTransactionUrl)
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(modelsToPush);
            var response = httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent).Result;

            return response.IsSuccessStatusCode;
        }
    }
}
