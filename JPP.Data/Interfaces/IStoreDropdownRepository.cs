using JPP.Models.Store.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Data.Interfaces
{
    public interface IStoreDropdownRepository
    {
        Task<IEnumerable<StoreDto>> GetFilteredStoresAsync();
    }
}