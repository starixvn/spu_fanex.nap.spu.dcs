using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Newtonsoft.Json;
using Sentry;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionServices
    {

        Task<bool> PushAbnormalTransaction(int size, string abnormalTransactionUrl);
    }

    public class AbnormalTransactionServices : IAbnormalTransactionServices
    {
        private readonly ISentryClient _sentryClient;
        private readonly IAbnormalTransactionDataAccess _dataAccess;

        public AbnormalTransactionServices(ISentryClient sentryClient, IAbnormalTransactionDataAccess dataAccess)
        {
            _sentryClient = sentryClient;
            _dataAccess = dataAccess;
        }

        public async Task<bool> PushAbnormalTransaction(int size, string abnormalTransactionUrl)
        {
            var (abnormalTransactions, transIDMinMax) = _dataAccess.GetAbnormalTransactionsAsync(size);

            ulong minID = transIDMinMax?.FirstOrDefault()?.MinID ?? 0;
            ulong maxID = transIDMinMax?.FirstOrDefault()?.MaxID ?? 0;

            if (abnormalTransactions.Any())
            {
                var dataTransformed = TransformData(abnormalTransactions);

                var isResult = await PushData(dataTransformed, abnormalTransactionUrl);

                _sentryClient.CaptureMessage($"PushAbnormalTransaction data (${isResult}): ${JsonConvert.SerializeObject(dataTransformed)}");
                if (isResult)
                {
                    var updateStatus = abnormalTransactions.Select(t => new AbnormalTransactionUpdateModel { AssociationID = t.AssociationID, Status = 1 });
                    var jsonTrans = JsonConvert.SerializeObject(updateStatus);

                    _dataAccess.UpdateAbnormalTransactions(jsonTrans, minID, maxID);
                }
                else
                    return false;
            }
            else
            {

                _sentryClient.CaptureMessage($"Cannot GetAbnormalTransactions: {minID} - {maxID}");
                //Update MaxId
                if (maxID > 0)
                {
                    try
                    {
                        _dataAccess.UpdateAbnormalTransactions(string.Empty, minID, maxID);
                    }
                    catch (Exception ex)
                    {
                        _sentryClient.CaptureException(ex);
                    }
                }
            }

            return true;
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

        private async Task<bool> PushData(IEnumerable<AbnormalTransactionModel> modelsToPush, string abnormalTransactionUrl)
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(modelsToPush);
            var response = await httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _sentryClient.CaptureException(new Exception("PushData return false. ResponseBody: " + responseBody + "\n" + JsonConvert.SerializeObject(modelsToPush)));
            }

            return response.IsSuccessStatusCode;
        }
    }
}
