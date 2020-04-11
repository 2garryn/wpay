using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Models
{

    public class AccountBalance
    {
        public AccountBalance(Amount money) => Amount = money;
        public static AccountBalance Zero(Currency currency) => new AccountBalance(AmountFactory.NewZero(currency));
        public Amount Amount { get; }
        public static AccountBalance operator +(AccountBalance balance, AmountIncomeCompleted amount) => new AccountBalance(balance.Amount + amount.Amount);
        public static AccountBalance operator -(AccountBalance balance, AmountOutcomeProcessing amount) => new AccountBalance(balance.Amount - amount.Amount);
        public static AccountBalance operator +(AccountBalance balance, AmountOutcomeCancelled amount) => new AccountBalance(balance.Amount - amount.Amount);
        public static bool operator >(AccountBalance balance, AmountOutcomeProcessing amount) => balance.Amount > amount.Amount;
        public static bool operator <(AccountBalance balance, AmountOutcomeProcessing amount) => balance.Amount < amount.Amount;
        public static bool operator >=(AccountBalance balance, AmountOutcomeProcessing amount) => balance.Amount >= amount.Amount;
        public static bool operator <=(AccountBalance balance, AmountOutcomeProcessing amount) => balance.Amount <= amount.Amount;
    }


}