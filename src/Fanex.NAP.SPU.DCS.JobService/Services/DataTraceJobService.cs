
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        Task<MemoryStream> ReportLoginTransactionDetail(DateTime transDate);

        Task<bool> InitBetTransactionAsync(DateTime transDate);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);

        Task<int> CountInitDWBetTransactionByDate(DateTime transDate);
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly IDataTraceServices dataTraceServices;

        public DataTraceJobService(IDataTraceServices dataTraceServices)
        {
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
            return await GenerateReport(fromDate, toDate, "ReportLoginTransaction_Template.xlsx", dataTraceServices.GetReportLoginTransaction);
        }

        public async Task<MemoryStream> ReportBotDTransaction(DateTime fromDate, DateTime toDate)
        {
            return await GenerateReport(fromDate, toDate, "ReportBotDTransaction_Template.xlsx", dataTraceServices.GetReportBotDTransaction);
        }

        public async Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await GenerateReport(fromDate, toDate, "ReportMissingTransaction_Template.xlsx", dataTraceServices.GetReportMissingTransaction);
        }

        private async Task<MemoryStream> GenerateReport<TModel>(DateTime fromDate, DateTime toDate, string templateFileName, Func<DateTime, DateTime, Task<IEnumerable<TModel>>> getDataFunc)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo($"Template/{templateFileName}")))
                {
                    var weekRanges = GetWeekRanges(fromDate, toDate);
                    var data = new List<TModel>();

                    var tasks = weekRanges.Select(async weekRange =>
                    {
                        var result = await getDataFunc(weekRange.Item1, weekRange.Item2);
                        lock (data)
                        {
                            data.AddRange(result);
                        }
                    });

                    await Task.WhenAll(tasks);

                    data = data.OrderBy(x => ((dynamic)x).TransDate).ToList();

                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MemoryStream> ReportLoginTransactionDetail(DateTime transDate)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo("Template/ReportLoginTransactionDetail_Template.xlsx")))
                {
                    var data = await dataTraceServices.GetReportLoginTransactionDetail(transDate);
                    return GenerateMemoryStream(package, data);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> InitBetTransactionAsync(DateTime transDate)
        {
            return await dataTraceServices.ProcessInitBetTransactionAsync(transDate);
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

        public async Task<int> CountInitDWBetTransactionByDate(DateTime transDate)
        {
            try
            {
                return await dataTraceServices.CountInitDWBetTransactionByDate(transDate);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DCS - DataTrace CountInitDWBetTransactionByDate execute exeption message: ");

                return 0;
            }
        }

        private static List<Tuple<DateTime, DateTime>> GetWeekRanges(DateTime fromDate, DateTime toDate)
        {
            var weekRanges = new List<Tuple<DateTime, DateTime>>();

            var week = 7;

            DateTime currentDate = fromDate;
            while (currentDate <= toDate)
            {
                DateTime weekStart = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                DateTime weekEnd = weekStart.AddDays(6);

                weekStart = weekStart < fromDate ? fromDate : weekStart;
                weekEnd = weekEnd > toDate ? toDate : weekEnd;

                weekRanges.Add(new Tuple<DateTime, DateTime>(weekStart, weekEnd));
                currentDate = currentDate.AddDays(week);
            }

            return weekRanges;
        }

        private static MemoryStream GenerateMemoryStream<T>(ExcelPackage package, IEnumerable<T> data, int firstRowData = 1)
        {
            var dataWorksheet = package.Workbook.Worksheets["Data"];

            dataWorksheet.Cells["A" + firstRowData].LoadFromCollection(data, true);
            var pivotWorksheet = package.Workbook.Worksheets["Pivot"];

            foreach (var pivotTable in pivotWorksheet.PivotTables)
            {
                pivotTable.CacheDefinition.SourceRange = dataWorksheet.Cells[firstRowData, dataWorksheet.Dimension.Start.Column, dataWorksheet.Dimension.End.Row, dataWorksheet.Dimension.End.Column];
            }

            return new MemoryStream(package.GetAsByteArray());
        }

        private static void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            LogService.Error(extraLogMessage + logMessage);
        }
    }
}
