using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wpay.Library.Services.Core.Models;
using wpay.Library.Models;
using wpay.Library.Services.Core.Commands;

namespace wpay.Library.Services.Core.Service
{

    public interface ICoreRepository
    {
        Task CreateAsync(Account account);
        Task<Account> GetAsync(AccountId id, bool forUpdate = false);
        Task<Dictionary<AccountId, Account>> GetAsync(HashSet<AccountId> id, bool forUpdate = false);
        Task UpdateAsync(Account account);

        Task CreateAsync(Transaction transaction);
        Task UpdateAsync(TransactionId id, ITransactionAmount amount, TransactionMetadata metadata, TransactionDescription description, DateTime updatedOn);
        Task<Transaction> GetAsync(TransactionId id, bool forUpdate = false);
        Task<IEnumerable<Transaction>> ListAsync(AccountId id, TransactionListOptions options);
    }


    public abstract class RepositoryException: Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }
    }
    public class AccountUniqException: Exception
    {
        public AccountUniqException(string message, AccountId accId) : base(message) { AccId = accId; }
        public AccountId AccId { get; }
    }
    public class AccountNotFoundException: RepositoryException
    {
        public AccountNotFoundException(string message, AccountId accId) : base(message) { AccId = accId; }
        public AccountId AccId { get; }
    }
    public class TransactionUniqException: RepositoryException
    {
        public TransactionUniqException(string message, TransactionId tranId) : base(message) { TranId = tranId; }
        public TransactionId TranId { get; }
    }
    public class TransactionNotFoundException: RepositoryException
    {
        public TransactionNotFoundException(string message, TransactionId tranId) : base(message) { TranId = tranId; }
        public TransactionId TranId { get; }
    }

}