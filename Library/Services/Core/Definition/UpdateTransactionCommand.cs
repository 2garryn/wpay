using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Definition
{

    public class UpdateTransactionCommand
    {
        public const string StatusCompleted = "completed";
        public const string StatusCancelled = "cancelled";
        public Guid Id {get;set;}
        public string Status {get;set;}
        public string Signature {get;set;}
        public Dictionary<string, ValueType> Metadata {get;set;}
        public Dictionary<string, string> Description {get;set;}

        public UpdateTransaction To()
        {
            var descr = new TransactionDescription();
            foreach(var kv in Description) descr.Add(kv.Key, kv.Value);
            
            var metadata = new TransactionMetadata();
            foreach(var kv in Metadata) metadata.Add(kv.Key, kv.Value);

            return new UpdateTransaction(
                new TransactionId(new UniqId(Id)),
                Status == StatusCompleted ? UpdateStatus.Complete : UpdateStatus.Cancel,
                Signature,
                descr,
                metadata 
            );
        }

        public UpdateTransactionCommand From(UpdateTransaction update)
        {
            return new UpdateTransactionCommand
            {
                Id = update.Id.Value.Value,
                Status = update.Status == UpdateStatus.Complete ? StatusCompleted : StatusCancelled,
                Signature = update.Signature,
                Description = update.Description.Value,
                Metadata = update.Metadata.Value,
            };
        }
    }


}