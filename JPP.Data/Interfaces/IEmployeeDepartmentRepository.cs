using JPP.Models.HR.EmployeeDepartment.Request;
using JPP.Models.HR.EmployeeDepartment.Responses;

namespace JPP.Data.Interfaces
{
    public interface IEmployeeDepartmentRepository
    {
        Task<List<EmployeeDepartmentItemDto>> GetListAsync(EmployeeDepartmentFilterRequest filter);
        Task<EmployeeDepartmentSummaryDto> GetSummaryAsync(EmployeeDepartmentFilterRequest filter);
        Task<EmployeeDepartmentDetailRequest?> GetByIdAsync(int id);
        Task<bool> CodeExistsAsync(string code, int excludeId = 0);
        Task<int> CreateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId);
        Task<bool> UpdateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId);
        Task<bool> SoftDeleteAsync(int id, int currentUserId, string currentUserName, int hqId, int storeId);
    }
}
