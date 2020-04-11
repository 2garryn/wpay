using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;


namespace wpay.Library.Services.Core.Messages
{

    public class CreateAccountCommand
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public bool Locked { get; set; }

        public bool IgnoreOnDuplicate { get; set; }

        public CreateAccount To()
        {
            var options = new CreateAccountOptions();
            options.IngoreOnDuplicate = IgnoreOnDuplicate;
            return new CreateAccount(new AccountId(new UniqId(Id)), CurrencyFactory.New(Currency), Locked, options);
        }

        public static CreateAccountCommand From(CreateAccount createAccount) =>
            new CreateAccountCommand
            {
                Id = createAccount.Id.Value.Value,
                Currency = createAccount.Currency.Code(),
                Locked = createAccount.Locked,
                IgnoreOnDuplicate = createAccount.Options.IngoreOnDuplicate
            };
    }

    public class CreateTransactionCommand
    {
        private const string _processingOutcome = "processing_outcome";
        private const string _completedIncome = "completed_income";
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Label {get;set;}
        public Dictionary<string, ValueType> Metadata {get;set;}
        public Dictionary<string, string> Description {get;set;}
        public string Signature {get;set;}
        public string CreateAmountType {get;set;}
        public string CreateAmountCurrency {get;set;}
        public string CreateAmountValue {get;set;}

        public bool FailOnExist {get;set;}

        public (CreateTransaction, CreateTransactionOptions) To() 
        {
            var am = AmountFactory.New(CreateAmountValue, CreateAmountCurrency);
            CreateAmount createAmount = CreateAmountType switch
            {
                _processingOutcome => new CreateAmountProcessingOutcome(am),
                _completedIncome => new CreateAmountCompletedIncome(am),
                _ => throw new InvalidOperationException("Invalid create transaction status")
            };
            
            var descr = new TransactionDescription();
            foreach(var kv in Description) descr.Add(kv.Key, kv.Value);
            
            var metadata = new TransactionMetadata();
            foreach(var kv in Metadata) metadata.Add(kv.Key, kv.Value);
            
            var create = new CreateTransaction(
                new AccountId(new UniqId(AccountId)),
                new TransactionId(new UniqId(Id)),
                new TransactionLabel(Label),
                createAmount,
                Signature,
                descr,
                metadata
            );

            var options = new CreateTransactionOptions();
            options.FailOnExist = FailOnExist;
            return (create, options);
        }

        public CreateTransactionCommand From(CreateTransaction create, CreateTransactionOptions options)
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
    }

    public class UpdateTransactionCommand
    {
        public const string StatusCompleted = "completed";
        public const string StatusCancelled = "cancelled";
        public Guid Id {get;set;}
        public string Status {get;set;}
        public string Signature {get;set;}
        public Dictionary<string, ValueType> Metadata {get;set;}
        public Dictionary<string, string> Description {get;set;}

        public bool FailOnUpdateDone {get;set;}

        public (UpdateTransaction, UpdateTransactionOptions) To()
        {
            var descr = new TransactionDescription();
            foreach(var kv in Description) descr.Add(kv.Key, kv.Value);
            
            var metadata = new TransactionMetadata();
            foreach(var kv in Metadata) metadata.Add(kv.Key, kv.Value);

            var update =  new UpdateTransaction(
                new TransactionId(new UniqId(Id)),
                Status == StatusCompleted ? UpdateStatus.Complete : UpdateStatus.Cancel,
                Signature,
                descr,
                metadata 
            );
            var options = new UpdateTransactionOptions();
            options.FailOnUpdateDone = FailOnUpdateDone;
            return (update, options);
        }

        public UpdateTransactionCommand From(UpdateTransaction update, UpdateTransactionOptions options)
        {
            return new UpdateTransactionCommand
            {
                Id = update.Id.Value.Value,
                Status = update.Status == UpdateStatus.Complete ? StatusCompleted : StatusCancelled,
                Signature = update.Signature,
                Description = update.Description.Value,
                Metadata = update.Metadata.Value,
                FailOnUpdateDone = options.FailOnUpdateDone
            };
        }
    }
}