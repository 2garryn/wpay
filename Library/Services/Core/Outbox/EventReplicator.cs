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
        public async Task PutAsync(object ev, Guid? convId)
        {
            var query = "insert into outbox_replication (Id, EventType, Event) VALUES (@Id, @EventType, CAST(@Event AS json))";
            var options = new JsonSerializerOptions()
            {
                IgnoreNullValues = false,

            };
            var fqClassName = ev.GetType().FullName;
            var serializedEvent = JsonSerializer.Serialize(ev) + ", Library";

            Type myType1 = Type.GetType(fqClassName);
            Console.WriteLine($"The full name is {myType1.FullName}.");
            

            await _connection.ExecuteAsync(query, new { Id = convId, Event = serializedEvent, EventType = fqClassName }, _transaction);



            
        }

    }


}