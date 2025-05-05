using System;

namespace Fanex.NAP.SPU.DCS.Features.Common
{
    public interface ICommonServices
    {
        int InsertTableIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date);

        int UpdateTableIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID);
    }

    public class CommonServices : ICommonServices
    {
        private readonly ICommonDataAccess _dataAccess;

        public CommonServices(ICommonDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public int InsertTableIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date)
        {
            return _dataAccess.InsertTableIDRange(databaseName, tableName, dateField, idField, date);
        }

        public int UpdateTableIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID)
        {
            return _dataAccess.UpdateTableIDRange(databaseName, tableName, date, minID, maxID);
        }
    }
}
