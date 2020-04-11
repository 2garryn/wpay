using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace wpay.Library.Libs.Db
{

    public class DbClient
    {
        private string _connectionString;
        public DbClient(string host, string username, string password, string database)
        {
            var template = "Host={0};Username={1};Password={2};Database={3}";
            _connectionString = String.Format(template, host, username, password, database);

        }

        public async Task ExecuteTransaction(Func<IDbConnection, IDbTransaction, Task> act)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var tx = connection.BeginTransaction();
            await act(connection, tx);
            await connection.CloseAsync();
        } 
        

    }

    


}