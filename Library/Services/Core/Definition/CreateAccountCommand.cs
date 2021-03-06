using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Commands;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;

namespace wpay.Library.Services.Core.Definition
{

    public class CreateAccountCommand
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public bool Locked { get; set; }

        public static CreateAccountCommand From(CreateAccount createAccount)
        {
            return new CreateAccountCommand
            {
                Id = createAccount.Id.Value.Value,
                Currency = createAccount.Currency.Code(),
                Locked = createAccount.Locked,
            };
        }
        public CreateAccount To() =>
            new CreateAccount(
                new AccountId(new UniqId(Id)),
                CurrencyFactory.New(Currency),
                Locked
            );
    }
}