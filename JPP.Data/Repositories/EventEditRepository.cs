using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Event.Request;

namespace JPP.Data.Repositories
{
    public class EventEditRepository : IEventEditRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public EventEditRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<EventRequestDto?> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            const string sql = @"
                SELECT Id, Name, Code, Description
                FROM BIZ_CustomerEvent
                WHERE Id = @Id";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.QuerySingleOrDefaultAsync<EventRequestDto>(sql, new { Id = id });
        }

        public async Task<bool> CodeExistsAsync(string code, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM BIZ_CustomerEvent
                WHERE Code = @Code
                  AND Id <> @ExcludeId";

            using var conn = _crmDbConnectionFactory.Create();
            var count = await conn.ExecuteScalarAsync<int>(sql, new
            {
                Code = code.Trim(),
                ExcludeId = excludeId
            });

            return count > 0;
        }

        public async Task<bool> UpdateEventAsync(EventRequestDto request)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            const string sql = @"
                UPDATE BIZ_CustomerEvent
                SET Name = @Name,
                    Code = @Code,
                    Description = @Description
                WHERE Id = @Id";

            using var conn = _crmDbConnectionFactory.Create();
            var rowsAffected = await conn.ExecuteAsync(sql, new
            {
                Id = request.Id,
                Name = request.Name?.Trim() ?? string.Empty,
                Code = request.Code?.Trim() ?? string.Empty,
                Description = request.Description?.Trim() ?? string.Empty
            });

            return rowsAffected > 0;
        }
    }
}
