using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;

namespace wpay.Library.Services.Core.Commands
{

    public class UpdateTransaction
    {
        public UpdateTransaction(TransactionId id, UpdateStatus status, string signature, TransactionDescription? description = null, TransactionMetadata? metadata = null)
        {
            Id = id;
            Status = status;
            Signature = signature;
            Description = description ?? new TransactionDescription();
            Metadata = metadata ?? new TransactionMetadata();
        }

        public TransactionId Id { get; }
        public TransactionMetadata Metadata { get; }
        public TransactionDescription Description { get; }
        public UpdateStatus Status { get; internal set; }
        public string Signature { get; internal set; }
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch
            {
                UpdateTransaction update => update.Id.Equals(Id),
                _ => false
            };
    }


    public enum UpdateStatus
    {
        Complete,
        Cancel
    }

    public class UpdateTransactionOptions
    {
        public UpdateTransactionOptions()
        {
            FailOnUpdateDone = true;

        }
        public bool FailOnUpdateDone { get; set; }
    }


}