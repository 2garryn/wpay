using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;

namespace wpay.Library.Services.Core.Commands
{

    public class CreateTransaction
    {
        internal CreateTransaction(AccountId accId, TransactionId id, TransactionLabel label,
            CreateAmount amount, string signature, TransactionDescription? description = null, TransactionMetadata? metadata = null)
        {
            AccountId = accId;
            Id = id;
            Label = label;
            Amount = amount;
            Metadata = metadata ?? new TransactionMetadata();;
            Description = description ?? new TransactionDescription();
            Signature = signature;
        }
        public AccountId AccountId { get; internal set; }
        public TransactionId Id { get; internal set; }
        public TransactionLabel Label { get; internal set; }
        public CreateAmount Amount { get; internal set; }
        public TransactionMetadata Metadata { get; internal set; }
        public TransactionDescription Description { get; internal set; }
        public string Signature {get; internal set;}
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch
            {
                CreateTransaction create => create.Id.Equals(Id),
                _ => false
            };
    }

    public abstract class CreateAmount { }

    public class CreateAmountProcessingOutcome : CreateAmount
    {
        public CreateAmountProcessingOutcome(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }

    public class CreateAmountCompletedIncome : CreateAmount
    {
        public CreateAmountCompletedIncome(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }


    public class CreateTransactionOptions
    {
        public CreateTransactionOptions()
        {
            FailOnExist = true;
        }
        public bool FailOnExist { get; set; }
    }


}