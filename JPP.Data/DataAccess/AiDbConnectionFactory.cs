using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.DataAccess
{
    public interface IAiDbConnectionFactory
    {
        IDbConnection Create();
    }

    public class AiDbConnectionFactory : IAiDbConnectionFactory
    {
        private readonly string _connectionString;

        public AiDbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("AiConnection is empty.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
