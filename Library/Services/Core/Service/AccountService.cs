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
        private readonly ICoreRepository _repo;
        public Service(ICoreRepository repo)
        {
            _repo = repo;
        }

        public async Task<Account> CreateAsync(CreateAccount create, Action<CreateAccountOptions>? setOpts = null)
        {
            var balance = AccountBalance.Zero(create.Currency);
            var acc = new Account(create.Id, balance, true);
            try
            {
                await _repo.CreateAsync(acc);
                return acc;
            }
            catch (AccountUniqException)
            {
                var options = new CreateAccountOptions();
                setOpts?.Invoke(options);
                if (!options.IngoreOnDuplicate)
                {
                    var info = new Dictionary<string, string>()
                    {
                        ["Id"] = create.Id.ToString()
                    };
                    throw new WPayException(AccountErrors.AlreadyExist, info);
                }
                return acc;
            }
        }
        public async Task<Account> GetAsync(AccountId id)
        {
            try
            {
                return await _repo.GetAsync(id);
            }
            catch (AccountNotFoundException)
            {
                var info = new Dictionary<string, string>()
                {
                    ["Id"] = id.ToString()
                };
                throw new WPayException(AccountErrors.AccountNotExist, info);
            }
        }
        public async Task<Dictionary<AccountId, Account>> GetAsync(HashSet<AccountId> ids)
        {
            return await _repo.GetAsync(ids);
        }
        public async Task LockAsync(AccountId id, bool locked)
        {
            try
            {
                var acc = await _repo.GetAsync(id, true);
                var newAcc = new Account(acc.Id, acc.Balance, locked);
                await _repo.UpdateAsync(newAcc);
            }
            catch (AccountNotFoundException)
            {
                var info = new Dictionary<string, string>()
                {
                    ["Id"] = id.ToString()
                };
                throw new WPayException(AccountErrors.AccountNotExist, info);

            }
        }
    }


}