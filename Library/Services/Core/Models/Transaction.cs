using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Models
{

    public class Transaction
    {
        internal Transaction(AccountId accId, TransactionId id, TransactionLabel label, ITransactionAmount amount, TransactionMetadata meta, TransactionDescription descr, DateTime createdOn, DateTime updatedOn)
        {
            AccountId = accId;
            Id = id;
            Label = label;
            Amount = amount;
            Metadata = meta;
            Description = descr;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }
        public AccountId AccountId { get; }
        public TransactionId Id { get; }
        public TransactionLabel Label { get; }
        public ITransactionAmount Amount { get; }
        public TransactionMetadata Metadata { get; }
        public TransactionDescription Description { get; }
        public DateTime CreatedOn {get;}
        public DateTime UpdatedOn {get;}
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch
            {
                Transaction c => c.Id.Equals(Id),
                _ => false
            };
    }

    public interface ITransactionAmount
    {
        Amount Amount { get;  }
    }

    public struct AmountIncomeCompleted : ITransactionAmount
    {
        public AmountIncomeCompleted(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }

    public struct AmountOutcomeProcessing : ITransactionAmount
    {
        public AmountOutcomeProcessing(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }

    public struct AmountOutcomeCompleted : ITransactionAmount
    {
        public AmountOutcomeCompleted(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }

    public struct AmountOutcomeCancelled : ITransactionAmount
    {
        public AmountOutcomeCancelled(Amount amount) => Amount = amount;
        public Amount Amount { get; }
    }

    public class TransactionDescription
    {
        public TransactionDescription() => Value = new Dictionary<string, string>() { };
        public void Add(string key, string value) => Value[key] = value;
        public TransactionDescription Merge(TransactionDescription descr)
        {
            var newVal = new TransactionDescription();
            foreach (KeyValuePair<string, string> kv in Value)
            {
                newVal.Add(kv.Key, kv.Value);
            }
            foreach (KeyValuePair<string, string> kv in descr.Value)
            {
                newVal.Add(kv.Key, kv.Value);
            }
            return newVal;
        }
        public Dictionary<string, string> Value { get; private set; }
    }

    public struct TransactionId
    {
        public TransactionId(UniqId value) => Value = value;
        public UniqId Value { get; }
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch
            {
                TransactionId c => c.Value.Equals(Value),
                _ => false
            };
    }

    public struct TransactionLabel
    {
        public TransactionLabel(string value) => Value = value;
        public string Value { get; }
    }

    public class TransactionMetadata
    {

        public TransactionMetadata() =>
            Value = new Dictionary<string, ValueType>() { };

        public void Add(string key, ValueType value) => Value[key] = value;

        public TransactionMetadata Merge(TransactionMetadata meta)
        {
            var newVal = new TransactionMetadata();
            foreach (KeyValuePair<string, ValueType> kv in Value)
            {
                newVal.Add(kv.Key, kv.Value);
            }
            foreach (KeyValuePair<string, ValueType> kv in meta.Value)
            {
                newVal.Add(kv.Key, kv.Value);
            }
            return newVal;
        }

        public Dictionary<string, ValueType> Value { get; private set; }
    }

    public struct TransactionTitle
    {
        public TransactionTitle(string value) => Value = value;
        public string Value { get; }
    }

}