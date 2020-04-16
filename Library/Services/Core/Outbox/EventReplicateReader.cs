using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using Dapper;
using MassTransit;
using Npgsql;
using wpay.Library.Libs.Db;


namespace wpay.Library.Services.Core.Outbox
{

    public class EventReplicateReader
    {
        private IBusControl _bus;
        private DbClient _db;
        public EventReplicateReader(DbClient db, IBusControl bus)
        {
            _bus = bus;
            _db = db;
        }
        public async Task Replicate() =>
            await _db.ExecuteTransaction(async (conn, tx) => await Replicate(conn, tx));
        
        public async Task Replicate(IDbConnection connection, IDbTransaction tx)
        {
            var query = "SELECT * FROM outbox_replication for update";
            var events = await connection.QueryAsync<ReplicateEvent>(query, tx);
            var ids = new List<Guid>();
            foreach(var ev in events)
            {
                await ReplEv(ev);
                ids.Add(ev.Id);
            }  
            query = "DELETE FROM outbox_replication WHERE ids = ANY(@Ids as ARRAY)";
            await connection.ExecuteAsync(query, new{Ids = ids}, tx);
            tx.Commit();
        }
        private async Task ReplEv(ReplicateEvent ev)
        {
            var type = Type.GetType(ev.EventType);
            var deser = JsonSerializer.Deserialize(ev.Event, type);
            await _bus.Publish(deser, context => context.ConversationId = ev.Id);
        }


    }


}