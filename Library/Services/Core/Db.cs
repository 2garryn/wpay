using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

using Npgsql;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

using wpay.Library.Services.Core.Service;
using wpay.Library.Models;
using wpay.Library.Services.Core.Models;
using wpay.Library.Services.Core.Commands;



namespace wpay.Library.Services.Core
{

    public class Db : ICoreRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _tx;
        public Db(IDbConnection connection, IDbTransaction tx) => (_connection, _tx) = (connection, tx);


        public async Task SetSavePointAsync()
        {
            var query = "SAVEPOINT before_savepoint";
            await _connection.ExecuteAsync(query, _tx);
        }

        public async Task RollbackToSavePointAsync()
        {
            var query = "ROLLBACK TO SAVEPOINT before_savepoint";
            await _connection.ExecuteAsync(query, _tx);
        }

        public async Task CreateAsync(Account account)
        {
            try
            {
                var exec = "INSERT INTO \"Accounts\" (Id, Balance, Currency, Locked) VALUES (@Id, @Balance, @Currency, @Locked)";
                var schema = AccountSchema.From(account);
                await _connection.ExecuteAsync(exec, schema, _tx);
            }
            catch (PostgresException pgEx)
            {
                if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new AccountUniqException("Account already exist", account.Id);
                }
            }
        }
        public async Task<Account> GetAsync(AccountId id, bool forUpdate = false)
        {
            var query = forUpdate ? "SELECT * FROM \"Accounts\" WHERE Id = @Id FOR UPDATE" : "SELECT * FROM \"Accounts\" WHERE Id = @Id";
            var acc = await _connection.QuerySingleOrDefaultAsync<AccountSchema>(query, new {Id = id.Value.Value}, _tx);
            var _ = acc ?? throw new AccountNotFoundException("Account not found", id);
            return acc.To();

        }
        public async Task<Dictionary<AccountId, Account>> GetAsync(HashSet<AccountId> id, bool forUpdate)
        {
            var query = forUpdate ? "SELECT * FROM \"Accounts\" WHERE Id IN {0} FOR UPDATE" : "SELECT * FROM \"Accounts\" WHERE Id IN {0}";
            var uuids = id.Select(id => id.Value.Value).ToList();
            query = String.Format(query, String.Join(",", uuids));
            return (await _connection.QueryAsync<AccountSchema>(query, _tx))
                .Select(schema => schema.To())
                .ToDictionary(acc => acc.Id, acc => acc);
        }
        public async Task UpdateAsync(Account account)
        {
            var schema = AccountSchema.From(account);
            var query = "UPDATE \"Accounts\" SET Balance = @Balance, Currency = @Currency, Locked = @Locked WHERE Id = @Id";
            var affectedRows = await _connection.ExecuteAsync(query, schema, _tx);
            if(affectedRows != 1) 
            {
                throw new AccountNotFoundException("Account not found", account.Id);
            };
        }

        public async Task CreateAsync(Transaction transaction)
        {
            var schema = TransactionSchema.From(transaction);
            var query = "INSERT INTO \"Transactions\" (Id, AccountId, Label, Status, Amount, Currency, Description, Metadata, CreatedOn, UpdatedOn) VALUES (@Id, @AccountId, @Label, @Status, @Amount, @Currency, @Description, @Metadata, @CreatedOn, @UpdatedOn)";
            try
            {
                await _connection.ExecuteAsync(query, schema, _tx);
            }
            catch (PostgresException pgEx)
            {
                if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new TransactionUniqException("transaction already exist", transaction.Id);
                }
            }
        }
        public async Task UpdateAsync(TransactionId id, ITransactionAmount amount, TransactionMetadata metadata, TransactionDescription description, DateTime updatedOn)
        {
            (decimal _, string _, string status) = TransactionSchema.ConvertAmount(amount);
            var query = "UPDATE \"Transactions\" SET Status = @Status, Metadata = @Metadata, Description = @Description, UpdatedOn = @UpdatedOn WHERE Id = @Id";
            var data = new 
            {
                Id = id.Value.Value, 
                Status = status, 
                Description = TransactionSchema.ConvertDescription(description),
                Metadata = TransactionSchema.ConvertMetadata(metadata)
            };
            var affectedRows = await _connection.ExecuteAsync(query, data, _tx);
            if(affectedRows != 1) 
            {
                throw new TransactionNotFoundException("Transaction not found", id);
            };
        }
        public async Task<Transaction> GetAsync(TransactionId id, bool forUpdate)
        {
            var query = forUpdate ? "SELECT * FROM \"Transactions\" WHERE Id = @Id FOR UPDATE" : "SELECT * FROM \"Transactions\" WHERE Id = @Id";
            var acc = await _connection.QuerySingleOrDefaultAsync<TransactionSchema>(query, new {Id = id.Value.Value}, _tx);
            var _ = acc ?? throw new TransactionNotFoundException("Transaction not found", id);
            return acc.To();
        }
    
        public async Task<IEnumerable<Transaction>> ListAsync(AccountId id, TransactionListOptions options)
        {
            var query = "SELECT * FROM \"Transactions\" WHERE AccountId = @AccountId AND StartDate >= @StartDate AND EndDate < @EndDate";
            var pars = new { AccountId = id.Value.Value, StartDate = options.StartDate, EndDate = options.EndDate};
            return (await _connection.QueryAsync<TransactionSchema>(query, pars, _tx)).Select(schema => schema.To());
        }
    
    }

    [Table("Accounts")]
    public class AccountSchema
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public bool Locked { get; set; }
        public string? Currency { get; set; }

        public static AccountSchema From(Account account)
        {
            return new AccountSchema
            {
                Id = account.Id.Value.Value,
                Balance = account.Balance.Amount.ToDecimal(),
                Currency = account.Balance.Amount.Currency().Code(),
                Locked = account.Locked
            };
        }

        public Account To()
        {
            var amount = AmountFactory.New(Balance, CurrencyFactory.New(Currency!));
            var id = new AccountId(new UniqId(Id));
            return new Account(id, new AccountBalance(amount), Locked);
        }
 
    }
    [Table("Transactions")]
    public class TransactionSchema
    {
        public const string StatusProcessing = "processing";
        public const string StatusCompleted = "completed";
        public const string StatusCancelled = "cancelled";


        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid AccountId {get; set;}
        public string? Label {get; set;}
        public string? Status {get;set;}
        public decimal? Amount {get; set;}
        public string? Currency {get; set;}
        public string? Description {get; set;}
        public string? Metadata {get;set;}
        public DateTime CreatedOn {get; set;}
        public DateTime UpdatedOn {get; set;}

        public static TransactionSchema From(Transaction tran)
        {
            (decimal amount, string currency, string status) = ConvertAmount(tran.Amount);
            return new TransactionSchema{
                Id = tran.Id.Value.Value,
                AccountId = tran.AccountId.Value.Value,
                Label = tran.Label.Value,
                Status = status,
                Currency = currency,
                Amount = amount,
                Description = ConvertDescription(tran.Description),
                Metadata = ConvertMetadata(tran.Metadata),
                CreatedOn = tran.CreatedOn,
                UpdatedOn = tran.UpdatedOn
            };
        }

        public static (decimal amount, string currency, string status) ConvertAmount(ITransactionAmount amount) => 
            amount switch 
            {
                AmountIncomeCompleted am => (am.Amount.ToDecimal(), am.Amount.Currency().Code(), StatusCompleted),
                AmountOutcomeProcessing am => (-am.Amount.ToDecimal(), am.Amount.Currency().Code(), StatusProcessing),
                AmountOutcomeCancelled am => (-am.Amount.ToDecimal(), am.Amount.Currency().Code(), StatusCancelled),
                AmountOutcomeCompleted am => (-am.Amount.ToDecimal(), am.Amount.Currency().Code(), StatusCompleted),
                _ => throw new InvalidOperationException("Invalid amount " + amount.ToString())
            };

        public static string ConvertMetadata(TransactionMetadata meta) => JsonSerializer.Serialize(meta.Value);
        public static string ConvertDescription(TransactionDescription description) => JsonSerializer.Serialize(description.Value);


        public Transaction To()
        {
            var accId = new AccountId(new UniqId(AccountId));
            var id = new TransactionId(new UniqId(Id));
            var label = new TransactionLabel(Label!);
            var am = AmountFactory.New(Math.Abs(Amount.GetValueOrDefault()), CurrencyFactory.New(Currency!));
            ITransactionAmount amount = Status switch 
            {
                StatusProcessing when Amount < 0 => new AmountOutcomeProcessing(am),
                StatusCompleted when Amount < 0 => new AmountOutcomeCompleted(am),
                StatusCompleted when Amount > 0 => new AmountIncomeCompleted(am),
                StatusCancelled when Amount < 0 => new AmountOutcomeCancelled(am),
                _ => throw new InvalidOperationException("Invalid amount " + Amount.ToString() + " " + Status)
            };
            var descr = JsonSerializer.Deserialize<Dictionary<string, string>>(Description);
            var tranDescr = new TransactionDescription();
            foreach(KeyValuePair<string, string> kv in descr)
            {
                tranDescr.Add(kv.Key, kv.Value);
            };
            var meta = JsonSerializer.Deserialize<Dictionary<string, ValueType>>(Metadata);
            var tranMeta = new TransactionMetadata();
            foreach(KeyValuePair<string, ValueType> kv in meta)
            {
                tranMeta.Add(kv.Key, kv.Value);
            };
            return new Transaction(accId, id, label, amount, tranMeta, tranDescr, CreatedOn, UpdatedOn);
        }
    }


}