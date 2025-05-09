using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Common.Criteria;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Common
{
    public interface ICommonDataAccess
    {
        int InsertTableDailyIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date);
        
        int UpdateTableDailyIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID);
    }

    public class CommonDataAccess : ICommonDataAccess
    {
        private readonly IDynamicRepository repository;

        public CommonDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public int InsertTableDailyIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date)
        {
            var criteria = new InsertTableDailyIDRangeCriteria
            {
                DatabaseName = databaseName,
                TableName = tableName,
                DateField = dateField,
                IDField = idField,
                Date = date
            };

            return repository.Execute(criteria);
        }

        public int UpdateTableDailyIDRange(string databaseName, string tableName, DateTime date, ulong minID, ulong maxID)
        {
            var criteria = new UpdateTableDailyIDRangeCriteria
            {
                DatabaseName = databaseName,
                TableName = tableName,
                Date = date,
                MinID = minID,
                MaxID = maxID
            };

            return repository.Execute(criteria);
        }
    }
}
