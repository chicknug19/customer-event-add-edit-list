using JPP.Models.HR.EmployeeList.Request;
using JPP.Models.HR.EmployeeList.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEmployeeListService
    {
        Task<EmployeeListPagedResponse> GetPagedAsync(EmployeeListFilterRequest filter);

        Task<EmployeeListItemDto?> GetEmployeeByIdAsync(int id);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<EmployeeDetailViewModel> BuildAddViewModelAsync(int? roasterYear = null, int? roasterMonth = null);

        Task<EmployeeDetailViewModel?> BuildEditViewModelAsync(int id, bool isReadOnly = false, int? roasterYear = null, int? roasterMonth = null);

        Task<EmployeeDetailViewModel> RebuildViewModelAsync(EmployeeDetailRequest request, bool isReadOnly = false);

        Task<int> CreateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<bool> UpdateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<List<EmployeeCustomTimeTableDto>> GetCustomTimeTablesAsync(int employeeId);

        Task<int> AddCustomTimeTableAsync(EmployeeCustomTimeTableRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<bool> DeleteCustomTimeTableAsync(int id, int currentUserId, string currentUserName);
    }
}
