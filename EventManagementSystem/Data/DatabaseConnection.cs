using System.Data;
using Npgsql;

namespace EventManagementSystem.Data
{
    public class DatabaseConnectionService
    {
        private readonly NpgsqlConnection _connection;

        public DatabaseConnectionService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }
    }
}
