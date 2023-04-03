using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IAbnormalTransactionJobService
    {
        Task<bool> Transform(int size);

        Task<bool> PostAbnormalTransaction(int size);
    }

    public class AbnormalTransactionJobService : IAbnormalTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAbnormalTransactionJobServices abnormalTransactionJobServices;

        private readonly string abnormalTransactionUrl;

        public AbnormalTransactionJobService(ISentryClient sentryClient, IAppSettings appSettings, IAbnormalTransactionJobServices abnormalTransactionJobServices)
        {
            this.sentryClient = sentryClient;
            this.abnormalTransactionJobServices = abnormalTransactionJobServices;

            abnormalTransactionUrl = appSettings.AbnormalTransactionUrl;
        }

        public async Task<bool> PostAbnormalTransaction(int size)
        {
            try
            {
                if (size > 0)
                {
                    var abnormalTransasction = abnormalTransactionJobServices.GetAbnormalTransasction(size);

                    var listEntities = FilterCountDevice(abnormalTransasction);

                    var listModel = TransformEntityToModel(listEntities);

                    if (await PostData(listModel))
                    {
                        UpdateAbnormalTransasction(abnormalTransasction, listModel);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }

            return false;
        }

        private IEnumerable<AbnormalTransactionEntity> FilterCountDevice(IEnumerable<AbnormalTransactionEntity> entities)
        {
            if (entities != null && entities.Any())
            {
                return entities.Where(x => x.CountDevice > 1).ToList();
            }

            return null;
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

        private async Task<bool> PostData(IEnumerable<AbnormalTransactionModel> models)
        {
            if (models != null && models.Any() && !string.IsNullOrEmpty(abnormalTransactionUrl))
            {
                try
                {
                    var httpClient = new HttpClient();
                    var jsonContent = JsonConvert.SerializeObject(models);
                    var response = await httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent);

                    if (!ValidateResponse(response))
                    {
                        sentryClient.CaptureException(new Exception(string.Format("Invalid response! Status code: {0} \nUrl: {1} \nContent: {2}", response.StatusCode, abnormalTransactionUrl, jsonContent)));
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);
                    return false;
                }
            }
            return true;
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
            return response.IsSuccessStatusCode;
        }

        public Task<bool> Transform(int size)
        {
            try
            {
                if (size > 0)
                {
                    abnormalTransactionJobServices.TransformAbnormalTransasction(size);
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }

            return Task.FromResult(false);
        }

        private void UpdateAbnormalTransasction(IEnumerable<AbnormalTransactionEntity> entities, IEnumerable<AbnormalTransactionModel> models)
        {
            var modelsUpdate = TransformEntityToUpdateModel(entities, models);
            if (modelsUpdate != null && modelsUpdate.Any())
            {
                var transJson = JsonConvert.SerializeObject(modelsUpdate);
                abnormalTransactionJobServices.Update(transJson);
            }
        }

        private IEnumerable<AbnormalTransactionUpdateModel> TransformEntityToUpdateModel(IEnumerable<AbnormalTransactionEntity> entities, IEnumerable<AbnormalTransactionModel> models)
        {
            entities = entities ?? Enumerable.Empty<AbnormalTransactionEntity>();
            models = models ?? Enumerable.Empty<AbnormalTransactionModel>();

            return entities.Select(entity =>
            {
                var updateModel = new AbnormalTransactionUpdateModel
                {
                    ID = entity.ID,
                    Status = models.FirstOrDefault(x => x.TransID == entity.TransID) != null ? 1 : 0
                };

                return updateModel;
            });
        }
    }
}
