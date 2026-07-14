using JPP.Models.HR.EmployeeDepartment.Request;
using JPP.Models.HR.EmployeeDepartment.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEmployeeDepartmentService
    {
        Task<EmployeeDepartmentPagedResponse> GetPagedAsync(EmployeeDepartmentFilterRequest filter);
        Task<EmployeeDepartmentFormViewModel> BuildAddViewModelAsync();
        Task<EmployeeDepartmentFormViewModel?> BuildEditViewModelAsync(int id);
        Task<int> CreateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId);
        Task<bool> UpdateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId);
        Task<bool> DeleteAsync(int id, int currentUserId, string currentUserName, int hqId, int storeId);
        Task<bool> CodeExistsAsync(string code, int excludeId = 0);
    }
}
