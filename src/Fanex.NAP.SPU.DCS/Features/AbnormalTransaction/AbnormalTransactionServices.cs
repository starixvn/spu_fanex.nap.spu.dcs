using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionServices
    {
        void TransformTrans07ToAbnormalTransaction(int size);

        void PushAbnormalTransaction(int size, string abnormalTransactionUrl);
    }

    public class AbnormalTransactionServices : IAbnormalTransactionServices
    {
        private readonly IAbnormalTransactionDataAccess _dataAccess;

        public AbnormalTransactionServices(IAbnormalTransactionDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void TransformTrans07ToAbnormalTransaction(int size)
        {
            if (size > 0)
                _dataAccess.TransformTrans07ToAbnormalTransaction(size);
        }

        public void PushAbnormalTransaction(int size, string abnormalTransactionUrl)
        {
            var abnormalTransactions = _dataAccess.GetAbnormalTransactionsAsync(size);

            if (abnormalTransactions.Any())
            {
                var dataTransformed = TransformData(abnormalTransactions);

                var isResult = PushData(dataTransformed, abnormalTransactionUrl);

                if(isResult)
                {
                    var updateStatus = abnormalTransactions
                                                .Select(t => new AbnormalTransactionUpdateModel { ID = t.ID, Status = 1 });
                    var jsonTrans = JsonConvert.SerializeObject(updateStatus);

                    _dataAccess.UpdateAbnormalTransactionsAsync(jsonTrans);
                }
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
