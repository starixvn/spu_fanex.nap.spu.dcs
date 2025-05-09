using System;

namespace Fanex.NAP.SPU.DCS.Features.Common
{
    public interface ICommonServices
    {
        int InsertTableDailyIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date);

        int UpdateTableDailyIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID);
    }

    public class CommonServices : ICommonServices
    {
        private readonly ICommonDataAccess _dataAccess;

        public CommonServices(ICommonDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public int InsertTableDailyIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date)
        {
            return _dataAccess.InsertTableDailyIDRange(databaseName, tableName, dateField, idField, date);
        }

        public int UpdateTableDailyIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID)
        {
            return _dataAccess.UpdateTableDailyIDRange(databaseName, tableName, date, minID, maxID);
        }
    }
}
