using JPP.Commons.Extensions;
using JPP.Data.Interfaces;
using JPP.Models.Admin.StoreList.Request;
using JPP.Models.Admin.StoreList.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;

namespace JPP.Services.Services
{
    public class StoreListService : IStoreListService
    {
        private readonly IStoreListRepository _storeListRepository;

        public StoreListService(IStoreListRepository storeListRepository)
        {
            _storeListRepository = storeListRepository;
        }

        public async Task<StoreListPagedResponse> GetPagedAsync(StoreListFilterRequest filter)
        {
            filter.NormalizeFilter();

            var summaryTask = _storeListRepository.GetStoreListSummaryAsync(filter);
            var storesTask = _storeListRepository.GetStoreListAsync(filter);

            await Task.WhenAll(summaryTask, storesTask);

            var summary = await summaryTask;
            var stores = await storesTask;

            return new StoreListPagedResponse
            {
                Summary = summary,
                Stores = new PagedResult<StoreListItemDto>
                {
                    Items = stores,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalRecords = summary.TotalStore,
                    LoadedRecords = filter.Skip + stores.Count
                }
            };
        }

        public async Task<StoreListItemDto?> GetStoreByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _storeListRepository.GetStoreByIdAsync(id);
        }

        public async Task<StoreDetailViewModel> BuildAddViewModelAsync(bool canChangeInCharge = true)
        {
            var employees = await _storeListRepository.GetEmployeeOptionsAsync();

            return new StoreDetailViewModel
            {
                Form = new StoreDetailRequest
                {
                    Id = 0,
                    IsPublished = false,
                    InChargeId = 0,
                    InChargeName = "- None -"
                },
                Employees = employees,
                CanChangeInCharge = canChangeInCharge,
                LastUpdatedText = DateTime.Now.ToString("dd-MMM-yyyy")
            };
        }

        public async Task<StoreDetailViewModel?> BuildEditViewModelAsync(int id, bool canChangeInCharge = true)
        {
            if (id <= 0)
            {
                return null;
            }

            var store = await _storeListRepository.GetStoreDetailByIdAsync(id);

            if (store == null)
            {
                return null;
            }

            var employees = await _storeListRepository.GetEmployeeOptionsAsync();

            return new StoreDetailViewModel
            {
                Form = new StoreDetailRequest
                {
                    Id = store.Id,
                    Code = store.Code,
                    StoreName = store.StoreName,
                    IsPublished = store.IsPublished,
                    InChargeId = store.InChargeId,
                    InChargeName = store.InChargeName,
                    BizRegNo = store.BizRegNo,
                    BlockHouseNo = store.BlockHouseNo,
                    UnitNo = store.UnitNo,
                    Address1 = store.Address1,
                    Address2 = store.Address2,
                    City = store.City,
                    State = store.State,
                    Country = store.Country,
                    PostalCode = store.PostalCode,
                    Phone = store.Phone,
                    Fax = store.Fax,
                    IpAddress = store.IpAddress,
                    PortNo = store.PortNo,
                    DbName = store.DbName,
                    DbLogin = store.DbLogin,
                    DbPassword = store.DbPassword
                },
                Employees = employees,
                CanChangeInCharge = canChangeInCharge,
                LastUpdatedText = store.LastUpdatedText
            };
        }

        public async Task<int> CreateStoreAsync(StoreDetailRequest request, int hqId, int currentUserId)
        {
            NormalizeDetailRequest(request);

            var employees = await _storeListRepository.GetEmployeeOptionsAsync();
            request.InChargeName = ResolveEmployeeName(employees, request.InChargeId);

            var newStoreId = await _storeListRepository.CreateStoreAsync(request, hqId);

            if (newStoreId > 0 && currentUserId > 0)
            {
                await _storeListRepository.RegisterOpeningQuantityAsync(newStoreId, currentUserId);
            }

            return newStoreId;
        }

        public async Task<bool> UpdateStoreAsync(StoreDetailRequest request)
        {
            if (request.Id <= 0)
            {
                return false;
            }

            NormalizeDetailRequest(request);

            var employees = await _storeListRepository.GetEmployeeOptionsAsync();
            request.InChargeName = ResolveEmployeeName(employees, request.InChargeId);

            return await _storeListRepository.UpdateStoreAsync(request);
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _storeListRepository.SoftDeleteStoreAsync(id);
        }

        private static void NormalizeDetailRequest(StoreDetailRequest request)
        {
            request.Code = (request.Code ?? string.Empty).Trim();
            request.StoreName = (request.StoreName ?? string.Empty).Trim();
            request.InChargeName = (request.InChargeName ?? string.Empty).Trim();
            request.BizRegNo = (request.BizRegNo ?? string.Empty).Trim();
            request.BlockHouseNo = (request.BlockHouseNo ?? string.Empty).Trim();
            request.UnitNo = (request.UnitNo ?? string.Empty).Trim();
            request.Address1 = (request.Address1 ?? string.Empty).Trim();
            request.Address2 = (request.Address2 ?? string.Empty).Trim();
            request.City = (request.City ?? string.Empty).Trim();
            request.State = (request.State ?? string.Empty).Trim();
            request.Country = (request.Country ?? string.Empty).Trim();
            request.PostalCode = (request.PostalCode ?? string.Empty).Trim();
            request.Phone = (request.Phone ?? string.Empty).Trim();
            request.Fax = (request.Fax ?? string.Empty).Trim();
            request.IpAddress = (request.IpAddress ?? string.Empty).Trim();
            request.PortNo = (request.PortNo ?? string.Empty).Trim();
            request.DbName = (request.DbName ?? string.Empty).Trim();
            request.DbLogin = (request.DbLogin ?? string.Empty).Trim();
            request.DbPassword = (request.DbPassword ?? string.Empty).Trim();
            request.SubmitMode = (request.SubmitMode ?? "Save").Trim();

            if (request.InChargeId < 0)
            {
                request.InChargeId = 0;
            }
        }

        private static string ResolveEmployeeName(
            List<StoreEmployeeOptionDto> employees,
            int inChargeId)
        {
            var employee = employees.FirstOrDefault(x => x.Value == inChargeId);

            return employee?.Text ?? "- None -";
        }
    }
}