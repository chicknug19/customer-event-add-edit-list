using JPP.Data.Interfaces;
using JPP.Models.Store.Responses;
using JPP.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class StoreDropdownService : IStoreDropdownService
    {
        private readonly IStoreDropdownRepository _storeRepo;

        public StoreDropdownService(IStoreDropdownRepository storeRepo)
        {
            _storeRepo = storeRepo;
        }

        public async Task<IEnumerable<StoreDto>> GetStoreDropdownListAsync()
        {
            return await _storeRepo.GetFilteredStoresAsync();
        }
    }
}