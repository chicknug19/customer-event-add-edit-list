using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPP.Models.Customer.Request;

namespace JPP.Data.Repositories
{
    public class CustomerListRepository : ICustomerListRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerListRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<CustomerListDto>> GetCustomerListAsync(CustomerListFilterRequest filter)
        {
            var keyword = filter.Keyword?.Trim() ?? string.Empty;

            const string sql = @"
                SELECT
                    ID AS CustomerID,
                    EventId,
                    ISNULL(FirstName, '') + ' ' + ISNULL(MiddleName, '') + ' ' + ISNULL(LastName, ' ') AS FullName,
                    Address1,
                    PhoneNumber
                FROM BIZ_Customer
                WHERE
                    (
                        @Keyword = ''
                        OR CAST(ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(FirstName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(MiddleName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(LastName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(Address1 AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(PhoneNumber AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    )
                ORDER BY FullName ASC
                OFFSET @Skip ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<CustomerListDto>(
                sql,
                new
                {
                    Keyword = keyword,
                    Skip = filter.Skip,
                    PageSize = filter.PageSize
                });

            return result.ToList();
        }
    }
}