using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class CustomerDiagnosticRepository : ICustomerDiagnosticRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerDiagnosticRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<CustomerDiagnosticDto>> GetCustomerDiagnosticAsync(int customerId)
        {
            const string sql = @"
                SELECT 
                    cd.Id,
                    cd.CustomerId,
                    cd.EventId,
                    cd.Type,
                    cd.Description,
                    cd.LogDate
                FROM Customer_Diagnostic cd
                INNER JOIN Customer_Event ce
                    ON ce.EventId = cd.EventId
                AND ce.CustomerId = cd.CustomerId
                WHERE cd.CustomerId = @CustomerId
                ORDER BY cd.LogDate DESC;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<CustomerDiagnosticDto>(
                sql,
                new
                {
                    CustomerId = customerId,
                });

            return result.ToList();
        }
    }
}