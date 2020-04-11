using System;
using System.Collections.Generic;
using System.Linq;

namespace wpay.Library.Services.Core.Models
{

    public class Account
    {
        public Account(AccountId id, AccountBalance balance, bool locked)
        {
            Id = id;
            Balance = balance;
            Locked = locked;
        }
        public AccountBalance Balance {get;}
        public AccountId Id { get; }
        public bool Locked { get; }
        public override int GetHashCode() => 
            Id.GetHashCode();
        public override bool Equals(object obj) => 
            obj switch 
            {
                Account c => c.Id.Equals(Id),
                _ => false
            };
    }


}