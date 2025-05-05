using Fanex.Data.Repository;
using Fanex.NAP.SPU.DCS.Features.Common.Criteria;
using System;

namespace Fanex.NAP.SPU.DCS.Features.Common
{
    public interface ICommonDataAccess
    {
        int InsertTableIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date);
    }

    public class CommonDataAccess : ICommonDataAccess
    {
        private readonly IDynamicRepository repository;

        public CommonDataAccess(IDynamicRepository repository)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public int InsertTableIDRange(string databaseName, string tableName, string dateField, string idField, DateTime date)
        {
            var criteria = new InsertTableIDRangeCriteria
            {
                DatabaseName = databaseName,
                TableName = tableName,
                DateField = dateField,
                IDField = idField,
                Date = date
            };

            return repository.Execute(criteria);
        }
    }
}
