using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.HR.EmployeeDepartment.Request;
using JPP.Models.HR.EmployeeDepartment.Responses;
using System.Data;

namespace JPP.Data.Repositories
{
    public class EmployeeDepartmentRepository : IEmployeeDepartmentRepository
    {
        private readonly ICrmDbConnectionFactory _connectionFactory;

        public EmployeeDepartmentRepository(ICrmDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<EmployeeDepartmentItemDto>> GetListAsync(EmployeeDepartmentFilterRequest filter)
        {
            using var conn = _connectionFactory.Create();
            var orderBy = filter.SortColumn switch
            {
                "Id" => "ID",
                "Code" => "Code",
                "Inactive" => "Inactive",
                "LastUpdated" => "LastUpdated",
                _ => "Name"
            };

            var sql = $@"
SELECT ID AS Id,
       ISNULL(Code, '') AS Code,
       ISNULL(Name, '') AS Name,
       ISNULL(Inactive, 0) AS Inactive,
       LastUpdated
FROM BIZ_EmployeeDepartment
WHERE (@Keyword = '' OR CAST(ID AS NVARCHAR(20)) LIKE @Search OR Code LIKE @Search OR Name LIKE @Search)
  AND (@Status = 'All'
       OR (@Status = 'Active' AND ISNULL(Inactive, 0) = 0)
       OR (@Status = 'Inactive' AND ISNULL(Inactive, 0) = 1))
ORDER BY {orderBy} {filter.SortDirection}
OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var rows = await conn.QueryAsync<EmployeeDepartmentItemDto>(sql, new
            {
                Keyword = filter.Keyword ?? string.Empty,
                Search = $"%{filter.Keyword ?? string.Empty}%",
                filter.Status,
                filter.Skip,
                filter.PageSize
            });
            return rows.ToList();
        }

        public async Task<EmployeeDepartmentSummaryDto> GetSummaryAsync(EmployeeDepartmentFilterRequest filter)
        {
            using var conn = _connectionFactory.Create();
            const string sql = @"
SELECT COUNT(1) AS TotalDepartment,
       SUM(CASE WHEN ISNULL(Inactive, 0) = 0 THEN 1 ELSE 0 END) AS ActiveDepartment,
       SUM(CASE WHEN ISNULL(Inactive, 0) = 1 THEN 1 ELSE 0 END) AS InactiveDepartment
FROM BIZ_EmployeeDepartment
WHERE (@Keyword = '' OR CAST(ID AS NVARCHAR(20)) LIKE @Search OR Code LIKE @Search OR Name LIKE @Search)
  AND (@Status = 'All'
       OR (@Status = 'Active' AND ISNULL(Inactive, 0) = 0)
       OR (@Status = 'Inactive' AND ISNULL(Inactive, 0) = 1));";
            return await conn.QuerySingleAsync<EmployeeDepartmentSummaryDto>(sql, new
            {
                Keyword = filter.Keyword ?? string.Empty,
                Search = $"%{filter.Keyword ?? string.Empty}%",
                filter.Status
            });
        }

        public async Task<EmployeeDepartmentDetailRequest?> GetByIdAsync(int id)
        {
            using var conn = _connectionFactory.Create();
            const string sql = @"
SELECT TOP 1 ID AS Id, ISNULL(Code, '') AS Code, ISNULL(Name, '') AS Name, ISNULL(Inactive, 0) AS Inactive
FROM BIZ_EmployeeDepartment WHERE ID = @Id;";
            return await conn.QuerySingleOrDefaultAsync<EmployeeDepartmentDetailRequest>(sql, new { Id = id });
        }

        public async Task<bool> CodeExistsAsync(string code, int excludeId = 0)
        {
            using var conn = _connectionFactory.Create();
            const string sql = @"SELECT COUNT(1) FROM BIZ_EmployeeDepartment WHERE UPPER(LTRIM(RTRIM(Code))) = UPPER(LTRIM(RTRIM(@Code))) AND ID <> @ExcludeId;";
            return await conn.ExecuteScalarAsync<int>(sql, new { Code = code, ExcludeId = excludeId }) > 0;
        }

        public async Task<int> CreateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId)
        {
            using var conn = _connectionFactory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                const string sql = @"
INSERT INTO BIZ_EmployeeDepartment (Code, Name, Inactive, LastUpdated)
VALUES (@Code, @Name, @Inactive, GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS INT);";
                var id = await conn.ExecuteScalarAsync<int>(sql, request, tx);
                await SaveAuditAsync(conn, tx, id, currentUserId, currentUserName, "Add Record", hqId, storeId);
                tx.Commit();
                return id;
            }
            catch { tx.Rollback(); throw; }
        }

        public async Task<bool> UpdateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId)
        {
            using var conn = _connectionFactory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                const string sql = @"UPDATE BIZ_EmployeeDepartment SET Code=@Code, Name=@Name, Inactive=@Inactive, LastUpdated=GETDATE() WHERE ID=@Id;";
                var affected = await conn.ExecuteAsync(sql, request, tx);
                if (affected > 0) await SaveAuditAsync(conn, tx, request.Id, currentUserId, currentUserName, "Update Record", hqId, storeId);
                tx.Commit();
                return affected > 0;
            }
            catch { tx.Rollback(); throw; }
        }

        public async Task<bool> SoftDeleteAsync(int id, int currentUserId, string currentUserName, int hqId, int storeId)
        {
            using var conn = _connectionFactory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                const string sql = @"UPDATE BIZ_EmployeeDepartment SET Inactive=1, LastUpdated=GETDATE() WHERE ID=@Id;";
                var affected = await conn.ExecuteAsync(sql, new { Id = id }, tx);
                if (affected > 0) await SaveAuditAsync(conn, tx, id, currentUserId, currentUserName, "Delete Record", hqId, storeId);
                tx.Commit();
                return affected > 0;
            }
            catch { tx.Rollback(); throw; }
        }

        private static async Task SaveAuditAsync(IDbConnection conn, IDbTransaction tx, int recordId, int userId, string userName, string action, int hqId, int storeId)
        {
            const string existsSql = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BIZ_AuditTrailLog';";
            if (await conn.ExecuteScalarAsync<int>(existsSql, transaction: tx) == 0) return;

            const string sql = @"
INSERT INTO BIZ_AuditTrailLog (LogDate, [Table], RecordID, UserID, Employee, Action, HQID, StoreID)
VALUES (GETDATE(), 'BIZ_EmployeeDepartment', @RecordId, @UserId, @UserName, @Action, @HqId, @StoreId);";
            await conn.ExecuteAsync(sql, new { RecordId = recordId, UserId = userId, UserName = userName, Action = action, HqId = hqId, StoreId = storeId }, tx);
        }
    }
}
