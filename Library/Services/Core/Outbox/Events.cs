using System;
using System.Collections.Generic;
using System.Linq;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Messages;

namespace wpay.Library.Services.Core.Outbox
{
    public interface ICoreEvent<T>
    {
        Guid? ConversationId { get; set; }
        T Event { get; set; }
    }

    public class TransactionCreated : ICoreEvent<TransactionEvent>
    {
        public Guid? ConversationId { get; set; }
        public TransactionEvent Event { get; set; }
        public TransactionCreated(Transaction transaction, Guid? conversationId) =>
            (ConversationId, Event) = (conversationId, new TransactionEvent(transaction));
    }

    public class TransactionUpdated : ICoreEvent<TransactionEvent>
    {
        public Guid? ConversationId { get; set; }
        public TransactionEvent Event { get; set; }
        public TransactionUpdated(Transaction transaction, Guid? conversationId) =>
            (ConversationId, Event) = (conversationId, new TransactionEvent(transaction));
    }

    public class AccountCreated : ICoreEvent<AccountEvent>
    {
        public Guid? ConversationId { get; set; }
        public AccountEvent Event { get; set; }
        public AccountCreated(Account account, Guid? conversationId) =>
            (ConversationId, Event) = (conversationId, new AccountEvent(account));
    }

    public class AccountLocked : ICoreEvent<AccountEvent>
    {
        public Guid? ConversationId { get; set; }
        public AccountEvent Event { get; set; }
        public AccountLocked(Account account, Guid? conversationId) =>
            (ConversationId, Event) = (conversationId, new AccountEvent(account));
    }

    public class AccountUnlocked : ICoreEvent<AccountEvent>
    {
        public Guid? ConversationId { get; set; }
        public AccountEvent Event { get; set; }
        public AccountUnlocked(Account account, Guid? conversationId) =>
            (ConversationId, Event) = (conversationId, new AccountEvent(account));
    }

    public class ErrorRaised : ICoreEvent<ErrorValue>
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