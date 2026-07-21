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
    public class EventDropdownRepository : IEventDropdownRepository
    {
        private readonly ICrmDbConnectionFactory _connFactory;

        public EventDropdownRepository(ICrmDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<IEnumerable<EventDropdownDto>> GetAllEventsAsync()
        {
            const string sql = "SELECT Id, Name, Code FROM BIZ_Event ORDER BY Name ASC";

            using var conn = _connFactory.Create();
            return await conn.QueryAsync<EventDropdownDto>(sql);
        }
    }
}
