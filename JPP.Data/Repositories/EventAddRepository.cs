using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Event.Responses;

namespace JPP.Data.Repositories
{
    public class EventAddRepository : IEventAddRepository
    {
        private readonly ICrmDbConnectionFactory _connFactory;

        public EventAddRepository(ICrmDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;

            const string sql = "SELECT COUNT(1) FROM BIZ_CustomerEvent WHERE Code = @Code";
            using var conn = _connFactory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, new { Code = code.Trim() }) > 0;
        }

        public async Task<int> CreateEventAsync(EventDto request)
        {
            const string sql = @"
                INSERT INTO BIZ_CustomerEvent (Name, Code, Description)
                VALUES (@Name, @Code, @Description);
                
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = _connFactory.Create();
            var newId = await conn.ExecuteScalarAsync<int>(sql, new
            {
                Name = request.Name?.Trim() ?? string.Empty,
                Code = request.Code?.Trim() ?? string.Empty,
                Description = request.Description?.Trim() ?? string.Empty
            });

            return newId;
        }
    }
}
