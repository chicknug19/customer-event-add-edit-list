using JPP.Models.Store.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface IStoreDropdownService
    {
        Task<IEnumerable<StoreDto>> GetStoreDropdownListAsync();
    }
}