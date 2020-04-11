using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Exceptions;

namespace wpay.Library.Services.Core.Service
{

    public partial class Service
    {
        public async Task CreateAsync(CreateTransaction create, CreateTransactionOptions options)
        {
            try
            {
                var account = await _repo.GetAsync(create.AccountId, true);
                var tran = new Calculator().CreateToTransaction(create);
                var balance = tran.Amount switch
                {
                    AmountIncomeCompleted am => account.Balance + am,
                    AmountOutcomeProcessing am when account.Balance < am => throw new WPayException(TransactionErrors.NotEnoughMoney),
                    AmountOutcomeProcessing am => account.Balance - am,
                    _ => throw new InvalidOperationException("Invalid amount")
                };
                account = new Account(account.Id, balance, account.Locked);

                await _repo.UpdateAsync(account);
                await _repo.CreateAsync(tran);

            }
            catch (TransactionUniqException)
            {
                if(options.FailOnExist)
                {
                    var info = new Dictionary<string, string>()
                    {
                        ["Id"] = create.Id.ToString()
                    };
                    throw new WPayException(TransactionErrors.AlreadyExist, info);
                }

            }

        }
        public async Task UpdateAsync(UpdateTransaction update, UpdateTransactionOptions options)
        {
            try
            {
                var tran = await _repo.GetAsync(update.Id, true);
                if(tran.Amount is AmountIncomeCompleted || tran.Amount is AmountOutcomeCompleted)
                {
                    if(options.FailOnUpdateDone)
                    {
                        throw new WPayException(TransactionErrors.AlreadyCompleted);
                    }
                    return;
                }
                var mergedTran = new Calculator().UpdateToTransaction(tran, update);
                var account = await _repo.GetAsync(tran.AccountId, true);
                var balance = mergedTran.Amount switch
                {
                    AmountOutcomeCompleted _ => account.Balance,
                    AmountOutcomeCancelled am => account.Balance + am,
                    _ => throw new InvalidOperationException("Invalid state")
                };
                account = new Account(account.Id, balance, account.Locked);
                await _repo.UpdateAsync(account);
                await _repo.UpdateAsync(mergedTran.Id, mergedTran.Amount, mergedTran.Metadata, mergedTran.Description, mergedTran.UpdatedOn);
            }
            catch (TransactionNotFoundException)
            {
                var info = new Dictionary<string, string>()
                {
                    ["Id"] = update.Id.ToString()
                };
                throw new WPayException(TransactionErrors.NotExist, info);
            }
        }

        public async Task<Transaction> GetAsync(TransactionId id)
        {
            try
            {
                return await _repo.GetAsync(id);
            }
            catch (TransactionNotFoundException)
            {
                var info = new Dictionary<string, string>()
                {
                    ["Id"] = id.ToString()
                };
                throw new WPayException(TransactionErrors.NotExist, info);
            }
        }
        
        public async Task<IEnumerable<Transaction>> ListAsync(AccountId id, TransactionListOptions options)
        {
            return await _repo.ListAsync(id, options);

        }
        
    }


}