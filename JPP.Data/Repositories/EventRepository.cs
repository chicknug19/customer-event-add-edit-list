using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Entities;
using JPP.Data.Interfaces;
using JPP.Models.Event.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ICrmDbConnectionFactory _connFactory;

        public EventRepository(ICrmDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            const string sql = "SELECT Id, Name, Code FROM BIZ_CustomerEvent ORDER BY Name ASC";

            using var conn = _connFactory.Create();
            return await conn.QueryAsync<EventDto>(sql);
        }
    }
}
