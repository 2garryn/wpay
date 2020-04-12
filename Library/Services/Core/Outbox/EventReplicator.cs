using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace wpay.Library.Services.Core.Outbox
{

    public class EventReplicator
    {
        
        public EventReplicator(IDbConnection connection, IDbTransaction transaction)
        {
            
        }
        public async Task PutAsync<T>(ICoreEvent<T> ev)
        {
            
        }
    }


}