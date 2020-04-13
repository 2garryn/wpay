using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using System.Text;
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
            var query = "insert into outbox_replication (Id, Event) VALUES (@Id, CAST(@Event AS json))";
            var options = new JsonSerializerOptions()
            {
                IgnoreNullValues = false,


            };
            var serializedEvent = JsonSerializer.Serialize<T>(ev.Event);
            Console.WriteLine(serializedEvent);
            byte[] barr = Encoding.UTF8.GetBytes(serializedEvent);
            await _connection.ExecuteAsync(query, new { Id = ev.ConversationId, Event = serializedEvent }, _transaction);
        }

    }


}