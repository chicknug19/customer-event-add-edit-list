using JPP.Models.HR.EmployeeList.Request;
using JPP.Models.HR.EmployeeList.Responses;

namespace JPP.Data.Interfaces
{
    public interface IEmployeeListRepository
    {
        Task<List<EmployeeListItemDto>> GetEmployeeListAsync(EmployeeListFilterRequest filter);

        Task<EmployeeListSummaryDto> GetEmployeeListSummaryAsync(EmployeeListFilterRequest filter);

        Task<EmployeeListItemDto?> GetEmployeeByIdAsync(int id);

        Task<bool> SoftDeleteEmployeeAsync(int id);

        Task<EmployeeDetailRequest?> GetEmployeeDetailAsync(int id, int roasterYear, int roasterMonth);

        Task<string> GetNextEmployeeNumberAsync();

        Task<List<EmployeeOptionDto>> GetRoleOptionsAsync();

        Task<List<EmployeeOptionDto>> GetDepartmentOptionsAsync();

        Task<List<EmployeeOptionDto>> GetSupervisorOptionsAsync();

        Task<List<EmployeeOptionDto>> GetTimeCategoryOptionsAsync();

        Task<List<EmployeeOptionDto>> GetWorkLocationOptionsAsync();

        Task<List<EmployeeCustomTimeTableDto>> GetCustomTimeTablesAsync(int employeeId);

        Task<int> CreateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<bool> UpdateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<int> AddCustomTimeTableAsync(EmployeeCustomTimeTableRequest request, int hqId, int storeId, int currentUserId, string currentUserName);

        Task<bool> DeleteCustomTimeTableAsync(int id, int currentUserId, string currentUserName);
    }
}
