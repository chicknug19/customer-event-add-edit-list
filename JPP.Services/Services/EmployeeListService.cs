using JPP.Commons.Extensions;
using JPP.Data.Interfaces;
using JPP.Models.HR.EmployeeList.Request;
using JPP.Models.HR.EmployeeList.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;

namespace JPP.Services.Services
{
    public class EmployeeListService : IEmployeeListService
    {
        private readonly IEmployeeListRepository _employeeListRepository;

        public EmployeeListService(IEmployeeListRepository employeeListRepository)
        {
            _employeeListRepository = employeeListRepository;
        }

        public async Task<EmployeeListPagedResponse> GetPagedAsync(EmployeeListFilterRequest filter)
        {
            filter.NormalizeFilter();

            var summaryTask = _employeeListRepository.GetEmployeeListSummaryAsync(filter);
            var employeesTask = _employeeListRepository.GetEmployeeListAsync(filter);

            await Task.WhenAll(summaryTask, employeesTask);

            var summary = await summaryTask;
            var employees = await employeesTask;

            return new EmployeeListPagedResponse
            {
                Summary = summary,
                Employees = new PagedResult<EmployeeListItemDto>
                {
                    Items = employees,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalRecords = summary.TotalEmployee,
                    LoadedRecords = filter.Skip + employees.Count
                }
            };
        }

        public async Task<EmployeeListItemDto?> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _employeeListRepository.GetEmployeeByIdAsync(id);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _employeeListRepository.SoftDeleteEmployeeAsync(id);
        }

        public async Task<EmployeeDetailViewModel> BuildAddViewModelAsync(int? roasterYear = null, int? roasterMonth = null)
        {
            var today = DateTime.Today;
            var year = ResolveRoasterYear(roasterYear);
            var month = ResolveRoasterMonth(roasterMonth);

            var request = new EmployeeDetailRequest
            {
                Number = await _employeeListRepository.GetNextEmployeeNumberAsync(),
                DateHired = today,
                Dob = today,
                RoasterYear = year,
                RoasterMonth = month,
                TimeTableDays = BuildDefaultTimeTableDays(),
                DutyRoasterDays = BuildDefaultDutyRoasterDays(year, month)
            };

            return await BuildViewModelAsync(request);
        }

        public async Task<EmployeeDetailViewModel?> BuildEditViewModelAsync(int id, bool isReadOnly = false, int? roasterYear = null, int? roasterMonth = null)
        {
            if (id <= 0)
            {
                return null;
            }

            var year = ResolveRoasterYear(roasterYear);
            var month = ResolveRoasterMonth(roasterMonth);
            var request = await _employeeListRepository.GetEmployeeDetailAsync(id, year, month);

            if (request == null)
            {
                return null;
            }

            request.RoasterYear = year;
            request.RoasterMonth = month;

            if (request.TimeTableDays.Count == 0)
            {
                request.TimeTableDays = BuildDefaultTimeTableDays();
            }

            if (request.DutyRoasterDays.Count == 0)
            {
                request.DutyRoasterDays = BuildDefaultDutyRoasterDays(year, month);
            }

            return await BuildViewModelAsync(request, isReadOnly);
        }

        public async Task<EmployeeDetailViewModel> RebuildViewModelAsync(EmployeeDetailRequest request, bool isReadOnly = false)
        {
            request.RoasterYear = ResolveRoasterYear(request.RoasterYear);
            request.RoasterMonth = ResolveRoasterMonth(request.RoasterMonth);

            if (request.TimeTableDays == null || request.TimeTableDays.Count == 0)
            {
                request.TimeTableDays = BuildDefaultTimeTableDays();
            }

            if (request.DutyRoasterDays == null || request.DutyRoasterDays.Count == 0)
            {
                request.DutyRoasterDays = BuildDefaultDutyRoasterDays(request.RoasterYear, request.RoasterMonth);
            }

            return await BuildViewModelAsync(request, isReadOnly);
        }

        public async Task<int> CreateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            if (request == null)
            {
                return 0;
            }

            return await _employeeListRepository.CreateEmployeeAsync(request, hqId, storeId, currentUserId, currentUserName);
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            if (request == null || request.Id <= 0)
            {
                return false;
            }

            return await _employeeListRepository.UpdateEmployeeAsync(request, hqId, storeId, currentUserId, currentUserName);
        }

        public async Task<List<EmployeeCustomTimeTableDto>> GetCustomTimeTablesAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                return new List<EmployeeCustomTimeTableDto>();
            }

            return await _employeeListRepository.GetCustomTimeTablesAsync(employeeId);
        }

        public async Task<int> AddCustomTimeTableAsync(EmployeeCustomTimeTableRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            return await _employeeListRepository.AddCustomTimeTableAsync(request, hqId, storeId, currentUserId, currentUserName);
        }

        public async Task<bool> DeleteCustomTimeTableAsync(int id, int currentUserId, string currentUserName)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _employeeListRepository.DeleteCustomTimeTableAsync(id, currentUserId, currentUserName);
        }

        private async Task<EmployeeDetailViewModel> BuildViewModelAsync(EmployeeDetailRequest request, bool isReadOnly = false)
        {
            var rolesTask = _employeeListRepository.GetRoleOptionsAsync();
            var departmentsTask = _employeeListRepository.GetDepartmentOptionsAsync();
            var supervisorsTask = _employeeListRepository.GetSupervisorOptionsAsync();
            var timeCategoriesTask = _employeeListRepository.GetTimeCategoryOptionsAsync();
            var workLocationsTask = _employeeListRepository.GetWorkLocationOptionsAsync();
            var customTimeTablesTask = request.Id > 0
                ? _employeeListRepository.GetCustomTimeTablesAsync(request.Id)
                : Task.FromResult(new List<EmployeeCustomTimeTableDto>());

            await Task.WhenAll(
                rolesTask,
                departmentsTask,
                supervisorsTask,
                timeCategoriesTask,
                workLocationsTask,
                customTimeTablesTask);

            return new EmployeeDetailViewModel
            {
                Form = request,
                Roles = await rolesTask,
                Departments = await departmentsTask,
                Supervisors = await supervisorsTask,
                TimeCategories = await timeCategoriesTask,
                WorkLocations = await workLocationsTask,
                CustomTimeTables = await customTimeTablesTask,
                IsReadOnly = isReadOnly
            };
        }

        private static int ResolveRoasterYear(int? year)
        {
            var value = year.GetValueOrDefault(DateTime.Today.Year);

            return value <= 0
                ? DateTime.Today.Year
                : value;
        }

        private static int ResolveRoasterMonth(int? month)
        {
            var value = month.GetValueOrDefault(DateTime.Today.Month);

            return value is < 1 or > 12
                ? DateTime.Today.Month
                : value;
        }

        private static List<EmployeeTimeTableDayRequest> BuildDefaultTimeTableDays()
        {
            return new[]
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            }
            .Select(day => new EmployeeTimeTableDayRequest
            {
                DayName = day,
                Working = false
            })
            .ToList();
        }

        private static List<EmployeeDutyRoasterDayRequest> BuildDefaultDutyRoasterDays(int year, int month)
        {
            var days = DateTime.DaysInMonth(year, month);
            var result = new List<EmployeeDutyRoasterDayRequest>();

            for (var day = 1; day <= days; day++)
            {
                result.Add(new EmployeeDutyRoasterDayRequest
                {
                    WorkDate = new DateTime(year, month, day),
                    OnDuty = true
                });
            }

            return result;
        }
    }
}
