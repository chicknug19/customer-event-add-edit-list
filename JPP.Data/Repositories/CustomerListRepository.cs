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
                    c.ID AS CustomerID,
                    ISNULL(c.FirstName, '') + ' ' + ISNULL(c.MiddleName, '') + ' ' + ISNULL(c.LastName, ' ') AS FullName,
                    c.Address1,
                    c.PhoneNumber,
                    c.EventID AS EventId,
                    e.Name AS EventName
                FROM BIZ_Customer c
                LEFT JOIN BIZ_Event e ON e.Id = c.EventID
                WHERE
                    (
                        @Keyword = ''
                        OR CAST(c.ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(c.FirstName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(c.MiddleName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(c.LastName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(c.Address1 AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(c.PhoneNumber AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
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