using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Models
{
    public struct AccountId
    {
        public AccountId(UniqId value) 
            => Value = value;
        public UniqId Value { get; }
        public override int GetHashCode() 
            => Value.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch 
            {
                AccountId c => c.Value.Equals(Value),
                _ => false
            };
    }
}