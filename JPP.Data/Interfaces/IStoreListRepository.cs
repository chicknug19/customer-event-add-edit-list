using JPP.Models.Admin.StoreList.Request;
using JPP.Models.Admin.StoreList.Responses;

namespace JPP.Data.Interfaces
{
    public interface IStoreListRepository
    {
        Task<List<StoreListItemDto>> GetStoreListAsync(StoreListFilterRequest filter);

        Task<StoreListSummaryDto> GetStoreListSummaryAsync(StoreListFilterRequest filter);

        Task<StoreListItemDto?> GetStoreByIdAsync(int id);

        Task<StoreDetailDto?> GetStoreDetailByIdAsync(int id);

        Task<List<StoreEmployeeOptionDto>> GetEmployeeOptionsAsync();

        Task<int> CreateStoreAsync(StoreDetailRequest request, int hqId);

        Task<bool> UpdateStoreAsync(StoreDetailRequest request);

        Task RegisterOpeningQuantityAsync(int storeId, int currentUserId);

        Task<bool> SoftDeleteStoreAsync(int id);
    }
}