using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Messages;

namespace wpay.Library.Services.Core.Outbox
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
        public Guid? ConversationId { get; set; }
        public ErrorValue Event {get;set;}
        public ErrorRaised(ErrorValue error, Guid? conversationId)
        {
            ConversationId = conversationId;
            Event = error;

        }
    }

    public class ErrorValue
    {
        public string Error { get; set; }
        public Dictionary<string, string> Info { get; set; }
        public ErrorValue(string error, Dictionary<string, string> info)
        {
            Error = error;
            Info = info;
        }
    }
}