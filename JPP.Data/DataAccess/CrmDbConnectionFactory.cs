using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.DataAccess
{
    public interface ICrmDbConnectionFactory
    {
        IDbConnection Create();
    }

    public class CrmDbConnectionFactory : ICrmDbConnectionFactory
    {
        private readonly string _connectionString;

        public CrmDbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("CrmConnection is empty.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
