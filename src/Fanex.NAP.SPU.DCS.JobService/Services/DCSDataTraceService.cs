using OfficeOpenXml;
using System;
using System.IO;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public interface IDCSDataTraceService
    {
        MemoryStream ReportDaily();
    }

    public class DCSDataTraceService : IDCSDataTraceService
    {
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
    }
}
