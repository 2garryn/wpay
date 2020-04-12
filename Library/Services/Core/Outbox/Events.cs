using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Models;

namespace wpay.Library.Services.Core.Outbox
{

    public class TransactionCreated
    {
        public TransactionCreated(Transaction transaction, Guid? id)
        {

        }
    }
    public class TransactionUpdated
    {
        public TransactionUpdated(Transaction transaction, Guid? id)
        {

        }
    }

    public class AccountCreated
    {
        public AccountCreated(Account account, Guid? id)
        {
            
        }
    }

    public class AccountLocked
    {

    }

    public class AccountUnlocked
    {

    }

    public class ErrorRaised
    {
        public ErrorRaised(string error, Dictionary<string, string> info)
        {
            
        } 
    }
}