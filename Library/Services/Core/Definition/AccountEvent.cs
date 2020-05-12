using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Definition
{

    public class AccountEvent
    {
        public Guid Id { get; set; }
        public string Balance { get; set; }
        public bool Locked { get; set; }
        public string? Currency { get; set; }

        public Account To()
        {
            return new Account(
                new AccountId(new UniqId(Id)),
                new AccountBalance(AmountFactory.New(Balance, Currency!)),
                Locked
            );
        }
        public static AccountEvent From(Account account)
        {
            return new AccountEvent
            {
                Id = account.Id.Value.Value,
                Balance = account.Balance.Amount.ToString(),
                Currency = account.Balance.Amount.Currency().Code(),
                Locked = account.Locked
            };
        }
    };

}

