using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Definition
{

    public class TransactionEvent
    {
        private const string _completedIncome = "completed_income";
        private const string _processingOutcome = "processing_outcome";
        private const string _completedOutcome = "completed_outcome";
        private const string _cancelledOutcome = "cancelled_outcome";
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Label { get; set; }
        public Dictionary<string, ValueType> Metadata { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string AmountCurrency { get; set; }
        public string AmountValue { get; set; }
        public string AmountType { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdateOn { get; set; }

        public Transaction To()
        {
            var descr = new TransactionDescription();
            foreach (var kv in Description) descr.Add(kv.Key, kv.Value);

            var metadata = new TransactionMetadata();
            foreach (var kv in Metadata) metadata.Add(kv.Key, kv.Value);

            var am = AmountFactory.New(AmountValue, AmountCurrency);
            ITransactionAmount amount = AmountType switch
            {
                _completedIncome => new AmountIncomeCompleted(am),
                _cancelledOutcome => new AmountOutcomeCancelled(am),
                _processingOutcome => new AmountOutcomeProcessing(am),
                _completedOutcome => new AmountOutcomeCompleted(am),
                _ => throw new InvalidOperationException("Invalid transaction status")
            };
            return new Transaction(
                new AccountId(new UniqId(AccountId)),
                new TransactionId(new UniqId(Id)),
                new TransactionLabel(Label),
                amount,
                metadata,
                descr,
                CreatedOn,
                UpdateOn
            );
        }

        public static TransactionEvent From(Transaction transaction)
        {
            var (amountType, amountValue, amountCurrency) = transaction.Amount switch
            {
                AmountIncomeCompleted am => (_completedIncome, am.Amount.ToString(), am.Amount.Currency().Code()),
                AmountOutcomeProcessing am => (_processingOutcome, am.Amount.ToString(), am.Amount.Currency().Code()),
                AmountOutcomeCancelled am => (_cancelledOutcome, am.Amount.ToString(), am.Amount.Currency().Code()),
                AmountOutcomeCompleted am => (_completedOutcome, am.Amount.ToString(), am.Amount.Currency().Code()),
                _ => throw new Exception("Invalid amount")
            };
            return new TransactionEvent
            {
                Id = transaction.Id.Value.Value,
                AccountId = transaction.AccountId.Value.Value,
                Label = transaction.Label.Value,
                Metadata = transaction.Metadata.Value,
                Description = transaction.Description.Value,
                AmountType = amountType,
                AmountCurrency = amountCurrency,
                AmountValue = amountValue
            };
        }

    }


}