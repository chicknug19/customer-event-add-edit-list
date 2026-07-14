using JPP.Commons.Extensions;
using JPP.Data.Interfaces;
using JPP.Models.HR.EmployeeDepartment.Request;
using JPP.Models.HR.EmployeeDepartment.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;

namespace JPP.Services.Services
{
    public class EmployeeDepartmentService : IEmployeeDepartmentService
    {
        private readonly IEmployeeDepartmentRepository _repository;

        public EmployeeDepartmentService(IEmployeeDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeDepartmentPagedResponse> GetPagedAsync(EmployeeDepartmentFilterRequest filter)
        {
            filter.NormalizeFilter();
            var summaryTask = _repository.GetSummaryAsync(filter);
            var listTask = _repository.GetListAsync(filter);
            await Task.WhenAll(summaryTask, listTask);
            var summary = await summaryTask;
            var items = await listTask;
            return new EmployeeDepartmentPagedResponse
            {
                Summary = summary,
                Departments = new PagedResult<EmployeeDepartmentItemDto>
                {
                    Items = items,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalRecords = summary.TotalDepartment,
                    LoadedRecords = filter.Skip + items.Count
                }
            };
        }

        public Task<EmployeeDepartmentFormViewModel> BuildAddViewModelAsync() =>
            Task.FromResult(new EmployeeDepartmentFormViewModel { Form = new EmployeeDepartmentDetailRequest() });

        public async Task<EmployeeDepartmentFormViewModel?> BuildEditViewModelAsync(int id)
        {
            var form = await _repository.GetByIdAsync(id);
            return form == null ? null : new EmployeeDepartmentFormViewModel { Form = form };
        }

        public Task<bool> CodeExistsAsync(string code, int excludeId = 0) => _repository.CodeExistsAsync(code.Trim(), excludeId);

        public Task<int> CreateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId)
        {
            Normalize(request);
            return _repository.CreateAsync(request, currentUserId, currentUserName, hqId, storeId);
        }

        public Task<bool> UpdateAsync(EmployeeDepartmentDetailRequest request, int currentUserId, string currentUserName, int hqId, int storeId)
        {
            Normalize(request);
            return _repository.UpdateAsync(request, currentUserId, currentUserName, hqId, storeId);
        }

        public Task<bool> DeleteAsync(int id, int currentUserId, string currentUserName, int hqId, int storeId) =>
            _repository.SoftDeleteAsync(id, currentUserId, currentUserName, hqId, storeId);

        private static void Normalize(EmployeeDepartmentDetailRequest request)
        {
            request.Code = request.Code.Trim();
            request.Name = request.Name.Trim();
        }
    }
}
