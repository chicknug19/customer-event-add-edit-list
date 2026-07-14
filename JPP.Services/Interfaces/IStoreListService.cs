using JPP.Models.Admin.StoreList.Request;
using JPP.Models.Admin.StoreList.Responses;

namespace JPP.Services.Interfaces
{
    public interface IStoreListService
    {
        Task<StoreListPagedResponse> GetPagedAsync(StoreListFilterRequest filter);

        Task<StoreListItemDto?> GetStoreByIdAsync(int id);

        Task<StoreDetailViewModel> BuildAddViewModelAsync(bool canChangeInCharge = true);

        Task<StoreDetailViewModel?> BuildEditViewModelAsync(int id, bool canChangeInCharge = true);

        Task<int> CreateStoreAsync(StoreDetailRequest request, int hqId, int currentUserId);

        Task<bool> UpdateStoreAsync(StoreDetailRequest request);

        Task<bool> DeleteStoreAsync(int id);
    }
}