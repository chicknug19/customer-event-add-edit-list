using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace JPP.Data.Repositories
{
    public class CustomerStoreDropdownRepository : ICustomerStoreDropdownRepository
    {
        private readonly ICrmDbConnectionFactory _connFactory;

        public CustomerStoreDropdownRepository(ICrmDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<IEnumerable<CustomerStoreDropdownDto>> GetAllStoresAsync()
        {
            const string sql = @"
                SELECT
                    ID AS Id,
                    StoreName AS Name
                FROM BIZ_Stores
                WHERE ISNULL(Inactive, 0) = 0
                AND ID IN (19, 27)
                ORDER BY StoreName ASC;";

            using var conn = _connFactory.Create();
            return await conn.QueryAsync<CustomerStoreDropdownDto>(sql);
        }
    }
}