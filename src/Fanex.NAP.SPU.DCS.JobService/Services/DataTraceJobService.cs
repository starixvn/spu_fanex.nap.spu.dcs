
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using OfficeOpenXml;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryLoginTransactionsAsync(int batchSize);

        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingBetTransactionAsync(DateTime? transDateint, int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(DateTime? transDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType);

        Task<MemoryStream> ReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> ReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> GetReportLoginTransactionDetail(DateTime transDate);
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

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            return await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate, batchSize);
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(DateTime? transDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(transDate, batchSize);
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

        public async Task<bool> SummaryMissingLoginTransactionAsync(DateTime? transDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(transDate, batchSize);
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

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            return await dataTraceServices.UpdateSystemSetting(SystemSettingID, SystemSettingValue);
        }

        public async Task<bool> DeleteMissingTransactionByDate(DateTime transDate, int missingType)
        {
            return await dataTraceServices.DeleteMissingTransactionByDate(transDate, missingType);
        }

        public async Task<MemoryStream> ReportLoginTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo("Template/ReportLoginTransaction_Template.xlsx")))
                {
                    var data = await dataTraceServices.GetReportLoginTransaction(fromDate, toDate);
                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MemoryStream> ReportBotDTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo("Template/ReportBotDTransaction_Template.xlsx")))
                {
                    var data = await dataTraceServices.GetReportBotDTransaction(fromDate, toDate);
                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo("Template/ReportMissingTransaction_Template.xlsx")))
                {
                    var data = await dataTraceServices.GetReportMissingTransaction(fromDate, toDate);
                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MemoryStream> GetReportLoginTransactionDetail(DateTime transDate)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo("Template/ReportLoginTransactionDetail_Template.xlsx")))
                {
                    var data = await dataTraceServices.GetReportLoginTransactionDetail(transDate);
                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private MemoryStream GenerateMemoryStream<T>(ExcelPackage package, IEnumerable<T> data, int firstRowData = 1)
        {
            var dataWorksheet = package.Workbook.Worksheets["Data"];

            dataWorksheet.Cells["A" + firstRowData].LoadFromCollection(data, true);
            var pivotWorksheet = package.Workbook.Worksheets["Pivot"];

            foreach (var pivotTable in pivotWorksheet.PivotTables)
            {
                pivotTable.CacheDefinition.SourceRange = dataWorksheet.Cells[firstRowData, dataWorksheet.Dimension.Start.Column, dataWorksheet.Dimension.End.Row, dataWorksheet.Dimension.End.Column];
            }

            //var dashboardWorksheet = package.Workbook.Worksheets["Dashboard"];

            //foreach (var chart in dashboardWorksheet.Drawings.OfType<ExcelChart>())
            //{
            //    chart.Series[0].XSeries = pivotWorksheet.Cells["A2:A4"]; ;  // Clear existing data
            //    chart.Series.Add(dashboardWorksheet.Cells["NewDataRange"], worksheet.Cells["XValuesRange"]);
            //}

            return new MemoryStream(package.GetAsByteArray());
        }
    }
}
