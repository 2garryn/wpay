using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Services.Core.Service
{

    public class AccountErrors
    {
        public static readonly string Prefix = "account";
        public static readonly string AccountNotExist = $"{Prefix}.not_exist";
        public static readonly string AlreadyExist = $"{Prefix}.already_exist";
    }
    public class TransactionErrors
    {
        public static readonly string Prefix = "transaction";
        public static readonly string InvalidCurrency = $"{Prefix}.invalid_currency";
        public static readonly string AlreadyExist = $"{Prefix}.already_exist";
        public static readonly string InvalidStatus = $"{Prefix}.invalid_status";
        public static readonly string InvalidAmount = $"{Prefix}.invalid_amount";
        public static readonly string NotEnoughMoney = $"{Prefix}.not_enough_money";
        public static readonly string NotExist = $"{Prefix}.not_exist";
        public static readonly string AlreadyCompleted = $"{Prefix}.already_completed";
    }


}