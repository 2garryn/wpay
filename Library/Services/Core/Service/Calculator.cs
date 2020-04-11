using System;
using System.Collections.Generic;
using System.Text;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Exceptions;

namespace wpay.Library.Services.Core.Service
{
    class Calculator
    {
        internal Transaction CreateToTransaction(CreateTransaction create)
        {
            var amount = CreateToTransactionAmount(create.Amount);
            var ts = DateTime.UtcNow;
            return new Transaction(create.AccountId, create.Id, create.Label, amount, create.Metadata, create.Description, ts, ts);
        }

        internal Transaction UpdateToTransaction(Transaction tran, UpdateTransaction update)
        {
            var meta = tran.Metadata.Merge(update.Metadata);
            var descr = tran.Description.Merge(update.Description);
            var amount = UpdateToTransactionAmount(update.Status, tran.Amount);
            
            return new Transaction(tran.AccountId, tran.Id, tran.Label, amount, meta, descr, tran.CreatedOn, DateTime.UtcNow);
        }

        private ITransactionAmount CreateToTransactionAmount(CreateAmount amount) =>
            amount switch
            {
                CreateAmountCompletedIncome am => new AmountIncomeCompleted(am.Amount),
                CreateAmountProcessingOutcome am => new AmountOutcomeProcessing(am.Amount),
                _ => throw new InvalidOperationException("Invalid state")
            };
        private ITransactionAmount UpdateToTransactionAmount(UpdateStatus status, ITransactionAmount amount) =>
            (status, amount) switch
            {
                (UpdateStatus.Cancel, AmountOutcomeProcessing am) => new AmountOutcomeCancelled(am.Amount),
                (UpdateStatus.Complete, AmountOutcomeProcessing am) => new AmountOutcomeCompleted(am.Amount),
                _ => throw new InvalidOperationException("Invalid state")
            };
    }


}
