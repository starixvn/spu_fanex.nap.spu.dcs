using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction;
using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
using Fanex.NAP.SPU.DCS.JobService.Models;
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
        bool Transform(int size);

        Task<bool> PostAbnormalTransaction(int size);
    }

    public class AbnormalTransactionJobService : IAbnormalTransactionJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAbnormalTransactionJobServices abnormalTransactionJobServices;

        private IEnumerable<AbnormalTransactionEntity> _abnormalTransasction;
        private IEnumerable<AbnormalTransactionEntity> _abnormalTransasctionFiltered;
        private IEnumerable<AbnormalTransactionModel> _abnormalTransasctionModel;
        private readonly string abnormalTransactionUrl;

        public AbnormalTransactionJobService(ISentryClient sentryClient, IAppSettings appSettings, IAbnormalTransactionJobServices abnormalTransactionJobServices)
        {
            this.sentryClient = sentryClient;
            this.abnormalTransactionJobServices = abnormalTransactionJobServices;

            abnormalTransactionUrl = appSettings.AbnormalTransactionUrl;
        }


        public bool Transform(int size)
        {
            if (size <= 0)
            {
                return false;
            }

            try
            {
                abnormalTransactionJobServices.TransformAbnormalTransasction(size);
                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }

        public async Task<bool> PostAbnormalTransaction(int size)
        {
            if (size <= 0)
            {
                return false;
            }

            try
            {
                _abnormalTransasction = abnormalTransactionJobServices.GetAbnormalTransasction(size);

                var isSuccessed = await FilterCountDevice()
                                          .TransformEntityToModel()
                                          .PostData();

                if (isSuccessed)
                {
                    UpdateAbnormalTransasction(_abnormalTransasction, _abnormalTransasctionFiltered);
                }

                return isSuccessed;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return false;
            }
        }

        private AbnormalTransactionJobService FilterCountDevice()
        {
            if (_abnormalTransasction != null && _abnormalTransasction.Any())
            {
                _abnormalTransasctionFiltered = _abnormalTransasction.Where(x => x.CountDevice > 1).ToList();
            }

            return this;
        }

        private AbnormalTransactionJobService TransformEntityToModel()
        {
            if (_abnormalTransasctionFiltered == null || !_abnormalTransasctionFiltered.Any())
            {
                return this;
            }

            var listModel = _abnormalTransasctionFiltered
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

            _abnormalTransasctionModel = listModel;
            return this;
        }

        private async Task<bool> PostData()
        {
            if (_abnormalTransasctionModel == null || !_abnormalTransasctionModel.Any() || string.IsNullOrEmpty(abnormalTransactionUrl))
            {
                return false;
            }

            using var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(_abnormalTransasctionModel);
            var response = await httpClient.PostAsJsonAsync(abnormalTransactionUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            sentryClient.CaptureException(new Exception($"Invalid response! Status code: {response.StatusCode}\nUrl: {abnormalTransactionUrl}\nContent: {jsonContent}"));
            return false;
        }

        private void UpdateAbnormalTransasction(IEnumerable<AbnormalTransactionEntity> entities, IEnumerable<AbnormalTransactionEntity> filteredEntities)
        {
            var modelsUpdate = TransformEntityToUpdateModel(entities, filteredEntities);
            if (modelsUpdate != null && modelsUpdate.Any())
            {
                var transJson = JsonConvert.SerializeObject(modelsUpdate);
                abnormalTransactionJobServices.Update(transJson);
            }
        }

        private IEnumerable<AbnormalTransactionUpdateModel> TransformEntityToUpdateModel(IEnumerable<AbnormalTransactionEntity> entities, IEnumerable<AbnormalTransactionEntity> filteredEntities)
        {
            var entityDict = filteredEntities.ToDictionary(x => x.TransID);

            return entities.Select(entity =>
            {
                var updateModel = new AbnormalTransactionUpdateModel
                {
                    ID = entity.ID,
                    Status = entityDict.ContainsKey(entity.TransID) ? 1 : 0
                };

                return updateModel;
            });
        }

    }
}
