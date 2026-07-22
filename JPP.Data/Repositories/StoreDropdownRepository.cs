using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Store.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class StoreDropdownRepository : IStoreDropdownRepository
    {
        private readonly ICrmDbConnectionFactory _connFactory;

        public StoreDropdownRepository(ICrmDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<IEnumerable<StoreDto>> GetFilteredStoresAsync()
        {
            const string sql = "SELECT ID, Code, StoreName FROM BIZ_Stores WHERE ID IN (19, 27) ORDER BY StoreName ASC";

            using var conn = _connFactory.Create();
            return await conn.QueryAsync<StoreDto>(sql);
        }
    }
}