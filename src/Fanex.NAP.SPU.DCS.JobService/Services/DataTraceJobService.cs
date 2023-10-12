
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using System.IO;
    using OfficeOpenXml;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryLoginTransactionsAsync(int batchSize);

        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate);

        Task<bool> SummaryMissingBetTransactionAsync(int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(int batchSize);

        Task<bool> InitBetTransactionAsync(DateTime fromDate, DateTime toDate);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType);
        
        MemoryStream ReportDaily();
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IDataTraceServices dataTraceServices;

        public DataTraceJobService(ISentryClient sentryClient, IDataTraceServices dataTraceServices)
        {
            this.sentryClient = sentryClient;
            this.dataTraceServices = dataTraceServices;
        }

        public async Task<bool> SummaryLoginTransactionsAsync(int batchSize)
        {
            try
            {
                await dataTraceServices.InsertSummaryLoginTransactionAsync(batchSize);

                return true;
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary login transaction job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate)
        {
            return await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate);
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(batchSize);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary missing bet ticket job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public async Task<bool> SummaryMissingLoginTransactionAsync(int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(batchSize);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"DCS - DataTrace summary missing login transaction job execute exeption message:  {logMessage}");

                return false;
            }
        }

        public MemoryStream ReportDaily()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo("Template/Template.xlsx")))
                {
                    var dataWorksheet = package.Workbook.Worksheets["Data"];

                    // Sample data
                    var data = new[]
                    {
                        new { TransDate = "04/10/2023", SubscriberName = "Alpha", LoginName = "sutest01", DeviceID = 1, Flagged = "Human", TotalTrans = 3020000 },
                        new { TransDate = "05/10/2023", SubscriberName = "Alpha1", LoginName = "sutest02", DeviceID = 1, Flagged = "Bot", TotalTrans = 2520000 }
                    };

                    dataWorksheet.Cells["A2"].LoadFromCollection(data, true);
                    var pivotWorksheet = package.Workbook.Worksheets["Pivot"];

                    var firstRowData = 2;
                    foreach (var pivotTable in pivotWorksheet.PivotTables)
                    {
                        pivotTable.CacheDefinition.SourceRange = dataWorksheet.Cells[firstRowData, dataWorksheet.Dimension.Start.Column, dataWorksheet.Dimension.End.Row, dataWorksheet.Dimension.End.Column];
                    }

                    return new MemoryStream(package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> InitBetTransactionAsync(DateTime fromDate, DateTime toDate)
        {
            return await dataTraceServices.ProcessInitBetTransactionAsync(fromDate, toDate);
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            return await dataTraceServices.UpdateSystemSetting(SystemSettingID, SystemSettingValue);
        }

        public async Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType)
        {
            return await dataTraceServices.DeleteMissingTransactionByDate(transDate, missingType);
        }
    }
}
