using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Models;

namespace wpay.Library.Services.Core.Definition
{
    public interface ICoreEvent{}

    public class TransactionCreated : ICoreEvent
    {
        public TransactionEvent Event { get; set; }
    }

    public class TransactionUpdated : ICoreEvent
    {
        public TransactionEvent Event { get; set; }
    }

    public class AccountCreated : ICoreEvent
    {
        public AccountEvent Event { get; set; }
    }

    public class AccountLocked : ICoreEvent
    {
        public AccountEvent Event { get; set; }
    }

    public class AccountUnlocked : ICoreEvent
    {
        public AccountEvent Event { get; set; }
    }

    public class ErrorRaised : ICoreEvent
    {
        public string Error { get; set; }
        public Dictionary<string, string> Info { get; set; }
    }

}