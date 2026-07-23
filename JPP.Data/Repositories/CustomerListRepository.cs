using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.CustomerEvent.Request;
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

        public async Task<List<CustomerListDto>> GetCustomerListAsync(CustomerListFilterRequest filter)
        {
            var keyword = filter.Keyword?.Trim() ?? string.Empty;

            const string sql = @"
                SELECT
                    c.ID AS CustomerID,
                    c.AccountNumber AS AccNo,
                    ISNULL(c.FirstName, '') + ' ' +
                    ISNULL(c.MiddleName, '') + ' ' +
                    ISNULL(c.LastName, '') AS FullName,
                    c.PhoneNumber,
                    c.Age,
                    c.Address1,
                    c.District AS Kecamatan,
                    c.EventID AS EventId,
                    e.Name AS EventName,
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM Customer_Event ce
                        INNER JOIN Customer_Diagnostic cd
                            ON cd.CustomerId = ce.CustomerId
                        AND cd.EventId   = ce.EventId
                        WHERE ce.CustomerId = c.ID
                        AND cd.Type = 'Skin'
                    ) THEN 'Yes' ELSE 'No' END AS Skin,
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM Customer_Event ce
                        INNER JOIN Customer_Diagnostic cd
                            ON cd.CustomerId = ce.CustomerId
                        AND cd.EventId   = ce.EventId
                        WHERE ce.CustomerId = c.ID
                        AND cd.Type = 'Dental'
                    ) THEN 'Yes' ELSE 'No' END AS Dental
                FROM BIZ_Customer c
                LEFT JOIN BIZ_Event e ON e.Id = c.EventID
                WHERE
                (
                    @Keyword = ''
                    OR CAST(c.ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.AccountNumber AS NVARCHAR(50)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.FirstName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.MiddleName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.LastName AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.Address1 AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.PhoneNumber AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(c.District AS NVARCHAR(150)), '') LIKE '%' + @Keyword + '%'
                )
                AND (@StoreId = 0 OR c.StoreID = @StoreId)
                AND (@EventId = 0 OR c.EventID = @EventId)
                ORDER BY FullName ASC
                OFFSET @Skip ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<CustomerListDto>(
                sql,
                new
                {
                    Keyword = keyword,
                    StoreId = filter.StoreId,
                    EventId = filter.EventId,
                    Skip = filter.Skip,
                    PageSize = filter.PageSize
                });

            return result.ToList();
        }


        public async Task<bool> SaveCustomerEventsAsync(CustomerEventSaveRequest request)
        {
            using var conn = _crmDbConnectionFactory.Create();


            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            string getStoreIdSql = "SELECT StoreId FROM BIZ_Customer WHERE ID = @CustomerId";
            int hqId = await conn.QueryFirstOrDefaultAsync<int>(getStoreIdSql, new { CustomerId = request.CustomerId });

            string insertSql = @"
            INSERT INTO Customer_Event 
            (UID, HQID, CustomerID, EventID, InActive, DateCreated, LastUpdated, IsUpdating, LastHQSync)
            VALUES 
            (@UID, @HQID, @CustomerID, @EventID, 0, @Now, @Now, 0, NULL)";

            using var transaction = conn.BeginTransaction();
            try
            {
                var now = DateTime.Now;

                // Looping sebanyak event yang dipilih oleh user
                foreach (var eventId in request.EventIds)
                {
                    await conn.ExecuteAsync(insertSql, new
                    {
                        UID = Guid.NewGuid(),
                        HQID = hqId,
                        CustomerID = request.CustomerId,
                        EventID = eventId,
                        Now = now
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }






    }
}