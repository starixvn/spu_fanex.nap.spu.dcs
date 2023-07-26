namespace Fanex.NAP.SPU.DCS.Features.Transactions
{
    using Fanex.NAP.SPU.DCS.Features.Transactions.Entities;
    using Sentry;
    using System;
    using System.Collections.Generic;

    public interface ITransactionServices
    {
        
        IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra);

        ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices TransactionProcessing(long fromId, long toId, bool isReadyDbExtra);

        ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId, bool isReadyDbExtra);
    }

    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionDataAccess transactionDataAccess;
        private readonly ISentryClient sentryClient;

        public TransactionServices(ITransactionDataAccess transactionDataAccess, ISentryClient sentryClient)
        {
            this.transactionDataAccess = transactionDataAccess;
            this.sentryClient = sentryClient;
        }

        public IEnumerable<RawTransactionEntity> GetRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch, bool isReadyDbExtra)
        {
            return this.transactionDataAccess.GetRawTransaction(isProcessed, numberOfTrans, numberOfBatch, isReadyDbExtra);
        }       

        public ITransactionServices TransactionProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            var startTime = DateTime.Now;
            this.transactionDataAccess.InsertTransaction(fromId, toId, isReadyDbExtra);

            var durationInSeconds = (DateTime.Now - startTime).TotalSeconds;
            sentryClient.CaptureMessage($"TransactionProcessing from {fromId} - to {toId} duration: {durationInSeconds}s");

            return this;
        }

        public ITransactionServices UnhandleSubscriberAccountProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            var startTime = DateTime.Now;
            this.transactionDataAccess.UnhandleSubscriberActionProcess(fromId, toId, isReadyDbExtra);

            var durationInSeconds = (DateTime.Now - startTime).TotalSeconds;
            sentryClient.CaptureMessage($"UnhandleSubscriberAccountProcessing from {fromId} - to {toId} duration: {durationInSeconds}s");

            return this;
        }

        public ITransactionServices UserAgentProcessing(long fromId, long toId, bool isReadyDbExtra)
        {
            var startTime = DateTime.Now;
            this.transactionDataAccess.InserUserAgent(fromId, toId, isReadyDbExtra);

            var durationInSeconds = (DateTime.Now - startTime).TotalSeconds;
            sentryClient.CaptureMessage($"UserAgentProcessing from {fromId} - to {toId} duration: {durationInSeconds}s");

            return this;
        }

        
    }
}
