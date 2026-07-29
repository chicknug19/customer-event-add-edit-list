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

        public async Task<LatestCustomerEventDto> GetLatestCustomerEventAsync(int customerId)
        {
            const string sql = @"
                SELECT TOP 1
                    HQID,
                    EventID AS EventId
                FROM Customer_Event
                WHERE CustomerId = @CustomerId
                ORDER BY DateCreated DESC;";

            using var conn = _crmDbConnectionFactory.Create();

            return await conn.QueryFirstOrDefaultAsync<LatestCustomerEventDto>(
                sql,
                new { CustomerId = customerId });
        }

        public async Task<int> AddCustomerDiagnosticAsync(NewCustomerDiagnosticDto dto)
        {
            const string sql = @"
                INSERT INTO Customer_Diagnostic
                    (UID, HQID, CustomerID, Type, EventID, Description, LogDate, InActive, DateCreated, LastUpdated, IsUpdating)
                VALUES
                    (NEWID(), @HQID, @CustomerId, @Type, @EventId, @Description, GETDATE(), 0, GETDATE(), GETDATE(), 0);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using var conn = _crmDbConnectionFactory.Create();

            return await conn.ExecuteScalarAsync<int>(sql, dto);
        }
        public async Task<int?> GetCustomerStoreIdAsync(int customerId)
        {
            const string sql = @"
                SELECT StoreID
                FROM BIZ_Customer
                WHERE ID = @CustomerId;";

            using var conn = _crmDbConnectionFactory.Create();

            return await conn.QueryFirstOrDefaultAsync<int?>(
                sql,
                new
                {
                    CustomerId = customerId
                });
        }
    }
}