using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IAbnormalTransactionJobService
    {
        Task<bool> Execute(int size);
    }

    public class AbnormalTransactionJobService : IAbnormalTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAbnormalTransactionJobServices abnormalTransactionJobServices;

        public AbnormalTransactionJobService(ISentryClient sentryClient, IAbnormalTransactionJobServices abnormalTransactionJobServices)
        {
            this.sentryClient = sentryClient;
            this.abnormalTransactionJobServices = abnormalTransactionJobServices;
        }

        public Task<bool> Execute(int size)
        {
            if (size == 0) return Task.FromResult(false);

            var abnormalTransasction = abnormalTransactionJobServices.GetAbnormalTransasction(size);

            var listModel = TransformEntityToModel(abnormalTransasction);

            return PostData(listModel);
        }

        private async Task<bool> PostData(IEnumerable<AbnormalTransactionModel> models)
        {
            try
            {
                if (models != null && models.Any())
                {
                    var httpClient = new HttpClient();
                    var url = "https://localhost:7093/api/test/post-data";
                    var jsonContent = JsonConvert.SerializeObject(models);
                    var response = await httpClient.PostAsJsonAsync(url, jsonContent);

                    if (!ValidateResponse(response))
                    {
                        sentryClient.CaptureException(new Exception(string.Format("Invalid response! Status code: {0} \nUrl: {1} \nContent: {2}", response.StatusCode, url, jsonContent)));
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param)
        {
            using (var web = new HttpClient())
            {
                var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                return await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
            }
        }

        static bool ValidateResponse(HttpResponseMessage response)
        {
            // Check if the response status code is in the 200-299 range
            return response.IsSuccessStatusCode;
        }

        private IEnumerable<AbnormalTransactionModel> TransformEntityToModel(IEnumerable<AbnormalTransactionEntity> entities)
        {
            if (entities != null && entities.Any())
            {
                var listModel = new List<AbnormalTransactionModel>();

                foreach (var entity in entities)
                {
                    var model = new AbnormalTransactionModel
                    {
                        Browser = entity.Browser,
                        Device = entity.DeviceTypeName,
                        IP = entity.IP,
                        LoginName = entity.LoginName,
                        OS = entity.OS,
                        TransID = entity.TransID,
                        TransTime = entity.TransTime,
                        UserAgent = entity.UserAgent
                    };
                    listModel.Add(model);
                }

                return listModel;
            }

            return null;
        }
    }
}
