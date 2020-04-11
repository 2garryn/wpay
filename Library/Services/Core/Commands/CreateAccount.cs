using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;

namespace wpay.Library.Services.Core.Commands
{
    /*
    *   Create account options
    */
    public class CreateAccountOptions
    {
        public CreateAccountOptions() => 
            IngoreOnDuplicate = false;
        
        public bool IngoreOnDuplicate {get; set;}
    }
    public class CreateAccount
    {
        public CreateAccount(AccountId id, Currency currency, bool locked, CreateAccountOptions options) => 
            (Id, Locked, Currency, Options) = (id, locked, currency, options);
        public AccountId Id { get; }
        public Currency Currency { get; }
        public bool Locked { get; }
        public CreateAccountOptions Options {get;}
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) =>
            obj switch 
            {
                CreateAccount create => create.Id.Equals(Id),
                _ => false
            };

        
    }





}