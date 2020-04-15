using System;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
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
        public async Task PutAsync(object ev, Guid? convId)
        {
            var query = "insert into outbox_replication (Id, EventType, Event) VALUES (@Id, @EventType, CAST(@Event AS json))";
            var options = new JsonSerializerOptions()
            {
                IgnoreNullValues = false,

            };
            await _connection.ExecuteAsync(query, new 
            { 
                Id = convId, 
                Event = JsonSerializer.Serialize(ev, options), 
                EventType = ev.GetType().FullName 
            }, _transaction);
        }

    }


}