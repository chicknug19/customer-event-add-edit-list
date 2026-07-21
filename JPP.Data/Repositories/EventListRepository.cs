using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Event.Responses; // Assuming you have an Event equivalent namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPP.Models.Event.Request; // Assuming you have an Event equivalent namespace

namespace JPP.Data.Repositories
{
    public class EventListRepository : IEventListRepository // Ensure you create this interface
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public EventListRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<EventListDto>> GetEventListAsync(EventListFilterRequest filter)
        {
            var keyword = filter.Keyword?.Trim() ?? string.Empty;

            const string sql = @"
                SELECT
                    Id AS EventId,
                    Name AS EventName,
                    Code,
                    Description
                FROM BIZ_Event
                WHERE
                    (
                        @Keyword = ''
                        OR CAST(Id AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(Name AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(Code AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                        OR ISNULL(CAST(Description AS NVARCHAR(MAX)), '') LIKE '%' + @Keyword + '%'
                    )
                ORDER BY Name ASC
                OFFSET @Skip ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<EventListDto>(
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