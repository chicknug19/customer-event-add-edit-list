using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class CustomerListRepository : ICustomerListRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerListRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<CustomerListDto>> GetCustomerListAsync()
        {
            const string sql = @"
            SELECT
                CustomerID,
                ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') AS FullName,
                Address1,
                PhoneNumber
            FROM BIZ_Customer;";

            using var conn = _crmDbConnectionFactory.Create();
            
            var result = await conn.QueryAsync<CustomerListDto>(sql);
            
            return result.ToList();
        }
    }
}