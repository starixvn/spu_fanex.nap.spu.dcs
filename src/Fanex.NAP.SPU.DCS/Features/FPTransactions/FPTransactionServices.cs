namespace Fanex.NAP.SPU.DCS.Features.FPTransactions
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.DCS.Features.FPTransactions.Entities;
    using System;
    using System.Collections.Generic;

    public interface IFPTransactionServices
    {
        IEnumerable<FPRawTransactionEntity> GetFPRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        IFPTransactionServices UserAgentProcessing(long fromId, long toId);

        IFPTransactionServices TransactionProcessing(long fromId, long toId);

        IFPTransactionServices UnhandleFPSubscriberActionProcessing(long fromId, long toId);
    }

    public class FPTransactionServices : IFPTransactionServices
    {
        private readonly IFPTransactionDataAccess _dataAccess;

        public FPTransactionServices(IFPTransactionDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        public IEnumerable<FPRawTransactionEntity> GetFPRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch)
        {            
            return this._dataAccess.GetFPRawTransaction(isProcessed, numberOfTrans, numberOfBatch);         
        }

        public IFPTransactionServices TransactionProcessing(long fromId, long toId)
        {
            this._dataAccess.InsertTransaction(fromId, toId);

            return this;
        }

        public IFPTransactionServices UnhandleFPSubscriberActionProcessing(long fromId, long toId)
        {
            this._dataAccess.UnhandleSubscriberActionProcess(fromId, toId);            

            return this;
        }

        public IFPTransactionServices UserAgentProcessing(long fromId, long toId)
        {
            this._dataAccess.InsertUserAgent(fromId, toId);

            return this;
        }

    }
}
