
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

        Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<MemoryStream> ReportLoginTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> ReportBotDTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<MemoryStream> GetReportLoginTransactionDetail(DateTime transDate);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);
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
                HandleException(ex, "DCS - DataTrace SummaryLoginTransactionsAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryBetTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace SummaryBetTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingBetTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace SummaryMissingBetTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize)
        {
            try
            {
                return await dataTraceServices.ProcessSummaryMissingLoginTransactionAsync(fromDate, toDate, batchSize);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace ProcessSummaryMissingLoginTransactionAsync execute exeption message: ");

                return false;
            }
        }

        public async Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue)
        {
            try
            {
                return await dataTraceServices.UpdateSystemSetting(SystemSettingID, SystemSettingValue);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace UpdateSystemSetting job execute exeption message: ");
            }

            return false;
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

        public async Task<int> CountDWBetTransactionByDate(DateTime transDate)
        {
            try
            {
                return await dataTraceServices.CountDWBetTransactionByDate(transDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace CountDWBetTransactionByDate execute exeption message: ");

                return 0;
            }
        }

        private void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            LogService.Error(extraLogMessage + logMessage);
        }
    }
}
