using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;
using Dapper;

namespace wpay.Library.Services.Core.Outbox
{

    public class EventReplicator
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        public EventReplicator(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task PutAsync<T>(ICoreEvent<T> ev)
        {
            var query = "insert into \"OutboxReplication\" (Id, Event) VALUES (@Id, @Event)";
            var serializedEvent = JsonSerializer.Serialize(ev.Event);
            await _connection.ExecuteAsync(query, new { Id = ev.ConversationId, Event = serializedEvent }, _transaction);
        }
    }


}