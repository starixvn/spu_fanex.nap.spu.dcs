
namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.DataTrace;
    using Fanex.NAP.SPU.DCS.Features.DataTrace.Model;
    using OfficeOpenXml;

    public interface IDataTraceJobService
    {
        Task<bool> SummaryBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingBetTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> SummaryMissingLoginTransactionAsync(DateTime? fromDate, DateTime? toDate, int batchSize);

        Task<bool> UpdateSystemSetting(int SystemSettingID, string SystemSettingValue);

        Task<int> CountDWBetTransactionByDate(DateTime transDate);

        Task<int> CookDataLoginTransaction(int size);

        Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate);
    }
    public class DataTraceJobService : IDataTraceJobService
    {
        private readonly IDataTraceServices dataTraceServices;
        private readonly ILogService logService;

        public DataTraceJobService(IDataTraceServices dataTraceServices, ILogService logService)
        {
            this.dataTraceServices = dataTraceServices;
            this.logService = logService;
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

        private static List<Tuple<DateTime, DateTime>> GetWeekRanges(DateTime fromDate, DateTime toDate)
        {
            var weekRanges = new List<Tuple<DateTime, DateTime>>();

            const int week = 7;

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

        private void HandleException(Exception ex, string extraLogMessage)
        {
            string logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            logService.ErrorMessageAsync(extraLogMessage + logMessage);
        }

        public async Task<int> CookDataLoginTransaction(int size)
        {
            if(size > 0)
            {
                return await dataTraceServices.CookDataLoginTransaction(size);
            }

            return 0;
        }


        #region Report

        public async Task<MemoryStream> ReportMissingTransaction(DateTime fromDate, DateTime toDate)
        {
            return await GenerateReport(fromDate, toDate, "ReportMissingTransaction_Template.xlsx", dataTraceServices.GetReportMissingTransaction);
        }

        public async Task<IEnumerable<ReportLoginTransactionBySubscriberModel>> GetReportLoginTransactionBySubscriber(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionBySubscriber(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionBySiteModel>> GetReportLoginTransactionBySite(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionBySite(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByAccountModel>> GetReportLoginTransactionByAccount(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByAccount(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByBrowserModel>> GetReportLoginTransactionByBrowser(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByBrowser(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReportLoginTransactionByFlaggedModel>> GetReportLoginTransactionByFlagged(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await dataTraceServices.GetReportLoginTransactionByFlagged(fromDate, toDate);
            }
            catch (Exception)
            {
                return null;
            }
        }

#pragma warning disable S6507 // Do not lock on local variable 'data', use a readonly field instead.
        private static async Task<MemoryStream> GenerateReport<TModel>(DateTime fromDate, DateTime toDate, string templateFileName, Func<DateTime, DateTime, Task<IEnumerable<TModel>>> getDataFunc)
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
#pragma warning restore S6507 // Do not lock on local variable 'data', use a readonly field instead.
        #endregion
    }
}
