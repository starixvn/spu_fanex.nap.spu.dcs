using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
using Fanex.NAP.SPU.DCS.JobService.Services;

namespace Fanex.NAP.SPU.DCS.Features.AbnormalTransaction
{
    public interface IAbnormalTransactionServices
    {

        Task<bool> PushAbnormalTransaction(int size, string abnormalTransactionUrl, bool ignorePushData);
    }

    public class AbnormalTransactionServices : IAbnormalTransactionServices
    {
        private readonly ILogService _logService;
        private readonly IAbnormalTransactionDataAccess _dataAccess;

        public AbnormalTransactionServices(ILogService logService, IAbnormalTransactionDataAccess dataAccess)
        {
            _logService = logService;
            _dataAccess = dataAccess;
        }

        public async Task<bool> PushAbnormalTransaction(int size, string abnormalTransactionUrl, bool ignorePushData)
        {
            var (abnormalTransactions, rangeID) = _dataAccess.GetAbnormalTransactionsAsync(size);

            if (abnormalTransactions.Any())
            {
                var dataTransformed = TransformData(abnormalTransactions);

                var isResult = await PushData(dataTransformed, abnormalTransactionUrl, ignorePushData);
                if (isResult)
                {
                    var updateStatus = abnormalTransactions.Select(t => new AbnormalTransactionUpdateModel { ID = t.ID, Status = 1 });
                    var jsonTrans = JsonSerializer.Serialize(updateStatus);

                    _dataAccess.UpdateAbnormalTransactions(jsonTrans, rangeID.MinID, rangeID.MaxID);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Update MaxId
                if (rangeID.MaxID > 0)
                {
                    _dataAccess.UpdateAbnormalTransactions(string.Empty, rangeID.MinID, rangeID.MaxID);
                }
            }

            return true;
        }

        private static IEnumerable<AbnormalTransactionModel> TransformData(IEnumerable<AbnormalTransactionEntity> entities)
        {
            if (entities.Any())
            {
                return entities
                    .Select(entity => new AbnormalTransactionModel
                    {
                        ID = entity.ID,
                        Browser = entity.Browser,
                        Device = entity.DeviceTypeName,
                        IP = entity.IP,
                        LoginName = entity.LoginName,
                        OS = entity.OS,
                        TransTime = entity.TransTime,
                        UserAgent = entity.UserAgent
                    });
            }

            return new List<AbnormalTransactionModel>();
        }

        private async Task<bool> PushData(IEnumerable<AbnormalTransactionModel> modelsToPush, string abnormalTransactionUrl, bool ignorePushData)
        {
            if(ignorePushData)
            {
                return true;
            }

            using var httpClient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(modelsToPush);
            var response = await httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode || responseBody != "true")
            {
                var exception = new Exception("PushData return false,\nURL: " + abnormalTransactionUrl + ".\n ResponseBody: " + responseBody + "\n" + JsonSerializer.Serialize(modelsToPush));
                await _logService.ErrorAsync(exception);
                return false;
            }

            return true;
        }
    }
}
