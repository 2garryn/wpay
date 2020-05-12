using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Definition
{

    public class CreateTransactionCommand
    {
        private const string _processingOutcome = "processing_outcome";
        private const string _completedIncome = "completed_income";
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Label { get; set; }
        public Dictionary<string, ValueType> Metadata { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string Signature { get; set; }
        public string CreateAmountType { get; set; }
        public string CreateAmountCurrency { get; set; }
        public string CreateAmountValue { get; set; }
        public static CreateTransactionCommand From(CreateTransaction create)
        {
            var (amtype, amcur, amvalue) = create.Amount switch
            {
                CreateAmountProcessingOutcome am => (_processingOutcome, am.Amount.Currency().Code(), am.Amount.ToString()),
                CreateAmountCompletedIncome am => (_completedIncome, am.Amount.Currency().Code(), am.Amount.ToString()),
                _ => throw new InvalidOperationException("Invalid create transaction status")
            };
            return new CreateTransactionCommand
            {
                Id = create.Id.Value.Value,
                AccountId = create.AccountId.Value.Value,
                Label = create.Label.Value,
                Metadata = create.Metadata.Value,
                Description = create.Description.Value,
                Signature = create.Signature,
                CreateAmountType = amtype,
                CreateAmountCurrency = amcur,
                CreateAmountValue = amvalue
            };
        }


        public CreateTransaction To()
        {
            Console.WriteLine("My Value {0} {1} {2}", CreateAmountValue, CreateAmountCurrency, CreateAmountType);
            var am = AmountFactory.New(CreateAmountValue, CreateAmountCurrency);
            CreateAmount createAmount = CreateAmountType switch
            {
                _processingOutcome => new CreateAmountProcessingOutcome(am),
                _completedIncome => new CreateAmountCompletedIncome(am),
                _ => throw new InvalidOperationException("Invalid create transaction status")
            };

            var descr = new TransactionDescription();
            foreach (var kv in Description) descr.Add(kv.Key, kv.Value);

            var metadata = new TransactionMetadata();
            foreach (var kv in Metadata) metadata.Add(kv.Key, kv.Value);

            return new CreateTransaction(
                new AccountId(new UniqId(AccountId)),
                new TransactionId(new UniqId(Id)),
                new TransactionLabel(Label),
                createAmount,
                Signature,
                descr,
                metadata
            );
        }
    }


}