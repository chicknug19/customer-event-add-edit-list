﻿using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.HR.EmployeeList.Request;
using JPP.Models.HR.EmployeeList.Responses;
using System.Data;

namespace JPP.Data.Repositories
{
    public class EmployeeListRepository : IEmployeeListRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public EmployeeListRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<EmployeeListItemDto>> GetEmployeeListAsync(EmployeeListFilterRequest filter)
        {
            using var conn = _crmDbConnectionFactory.Create();
            var columns = await GetTableColumnsAsync(conn, "BIZ_Employee");
            var query = BuildEmployeeQueryDefinition(columns);
            var keyword = filter.Keyword?.Trim() ?? string.Empty;
            var status = string.IsNullOrWhiteSpace(filter.Status)
                ? "All"
                : filter.Status.Trim();
            var orderByClause = BuildEmployeeOrderByClause(filter.SortColumn, filter.SortDirection);
            var deletedCondition = BuildDeletedCondition(columns, "e");

            var sql = $@"
            SELECT
                e.ID AS Id,
                {query.NumberExpression} AS Number,
                {query.DisplayNameExpression} AS DisplayName,
                {query.DepartmentExpression} AS Department,
                {query.RoleNameExpression} AS RoleName,
                {query.HPNumExpression} AS HPNum,
                {query.SupervisorExpression} AS SupervisorDisplayName,
                {query.HRCategoryExpression} AS HRCategory,
                {query.EmailAddressExpression} AS EmailAddress,
                CAST(ISNULL(e.Inactive, 0) AS BIT) AS Inactive
            FROM BIZ_Employee e
            WHERE
                {deletedCondition}
                AND
                (
                    @Keyword = ''
                    OR CAST(e.ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                    OR {query.NumberExpression} LIKE '%' + @Keyword + '%'
                    OR {query.DisplayNameExpression} LIKE '%' + @Keyword + '%'
                    OR {query.DepartmentExpression} LIKE '%' + @Keyword + '%'
                    OR {query.RoleNameExpression} LIKE '%' + @Keyword + '%'
                    OR {query.HPNumExpression} LIKE '%' + @Keyword + '%'
                    OR {query.SupervisorExpression} LIKE '%' + @Keyword + '%'
                    OR {query.HRCategoryExpression} LIKE '%' + @Keyword + '%'
                    OR {query.EmailAddressExpression} LIKE '%' + @Keyword + '%'
                )
                AND
                (
                    @Status = 'All'
                    OR (@Status = 'Active' AND ISNULL(e.Inactive, 0) = 0)
                    OR (@Status = 'Inactive' AND ISNULL(e.Inactive, 0) = 1)
                )
            ORDER BY
                {orderByClause}
            OFFSET @Skip ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

            var result = await conn.QueryAsync<EmployeeListItemDto>(
                sql,
                new
                {
                    Keyword = keyword,
                    Status = status,
                    Skip = filter.Skip,
                    PageSize = filter.PageSize
                });

            return result.ToList();
        }

        public async Task<EmployeeListSummaryDto> GetEmployeeListSummaryAsync(EmployeeListFilterRequest filter)
        {
            using var conn = _crmDbConnectionFactory.Create();
            var columns = await GetTableColumnsAsync(conn, "BIZ_Employee");
            var query = BuildEmployeeQueryDefinition(columns);
            var keyword = filter.Keyword?.Trim() ?? string.Empty;
            var status = string.IsNullOrWhiteSpace(filter.Status)
                ? "All"
                : filter.Status.Trim();
            var deletedCondition = BuildDeletedCondition(columns, "e");

            var sql = $@"
            SELECT
                COUNT(1) AS TotalEmployee,
                ISNULL(SUM(CASE WHEN ISNULL(e.Inactive, 0) = 0 THEN 1 ELSE 0 END), 0) AS ActiveEmployee,
                ISNULL(SUM(CASE WHEN ISNULL(e.Inactive, 0) = 1 THEN 1 ELSE 0 END), 0) AS InactiveEmployee
            FROM BIZ_Employee e
            WHERE
                {deletedCondition}
                AND
                (
                    @Keyword = ''
                    OR CAST(e.ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                    OR {query.NumberExpression} LIKE '%' + @Keyword + '%'
                    OR {query.DisplayNameExpression} LIKE '%' + @Keyword + '%'
                    OR {query.DepartmentExpression} LIKE '%' + @Keyword + '%'
                    OR {query.RoleNameExpression} LIKE '%' + @Keyword + '%'
                    OR {query.HPNumExpression} LIKE '%' + @Keyword + '%'
                    OR {query.SupervisorExpression} LIKE '%' + @Keyword + '%'
                    OR {query.HRCategoryExpression} LIKE '%' + @Keyword + '%'
                    OR {query.EmailAddressExpression} LIKE '%' + @Keyword + '%'
                )
                AND
                (
                    @Status = 'All'
                    OR (@Status = 'Active' AND ISNULL(e.Inactive, 0) = 0)
                    OR (@Status = 'Inactive' AND ISNULL(e.Inactive, 0) = 1)
                );";

            var result = await conn.QuerySingleOrDefaultAsync<EmployeeListSummaryDto>(
                sql,
                new
                {
                    Keyword = keyword,
                    Status = status
                });

            return result ?? new EmployeeListSummaryDto();
        }

        public async Task<EmployeeListItemDto?> GetEmployeeByIdAsync(int id)
        {
            using var conn = _crmDbConnectionFactory.Create();
            var columns = await GetTableColumnsAsync(conn, "BIZ_Employee");
            var query = BuildEmployeeQueryDefinition(columns);
            var deletedCondition = BuildDeletedCondition(columns, "e");

            var sql = $@"
            SELECT TOP 1
                e.ID AS Id,
                {query.NumberExpression} AS Number,
                {query.DisplayNameExpression} AS DisplayName,
                {query.DepartmentExpression} AS Department,
                {query.RoleNameExpression} AS RoleName,
                {query.HPNumExpression} AS HPNum,
                {query.SupervisorExpression} AS SupervisorDisplayName,
                {query.HRCategoryExpression} AS HRCategory,
                {query.EmailAddressExpression} AS EmailAddress,
                CAST(ISNULL(e.Inactive, 0) AS BIT) AS Inactive
            FROM BIZ_Employee e
            WHERE e.ID = @Id
              AND {deletedCondition};";

            return await conn.QuerySingleOrDefaultAsync<EmployeeListItemDto>(
                sql,
                new
                {
                    Id = id
                });
        }

        public async Task<bool> SoftDeleteEmployeeAsync(int id)
        {
            const string sql = @"
            UPDATE BIZ_Employee
            SET
                Inactive = 1,
                LastUpdated = GETDATE()
            WHERE ID = @Id;

            IF OBJECT_ID('HR_EmployeeProperties', 'U') IS NOT NULL
            BEGIN
                UPDATE HR_EmployeeProperties
                SET
                    Inactive = 1,
                    LastUpdated = GETDATE()
                WHERE EmployeeID = @Id;
            END;

            IF OBJECT_ID('HR_LeaveSummary', 'U') IS NOT NULL
            BEGIN
                UPDATE HR_LeaveSummary
                SET
                    Inactive = 1,
                    LastUpdated = GETDATE()
                WHERE EmployeeID = @Id;
            END;";

            using var conn = _crmDbConnectionFactory.Create();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                var affectedRows = await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        Id = id
                    },
                    transaction);

                transaction.Commit();

                return affectedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<EmployeeDetailRequest?> GetEmployeeDetailAsync(int id, int roasterYear, int roasterMonth)
        {
            using var conn = _crmDbConnectionFactory.Create();
            var employeeColumns = await GetTableColumnsAsync(conn, "BIZ_Employee");
            var deletedCondition = BuildDeletedCondition(employeeColumns, "e");
            var allowToChangePriceExpression = BuildEmployeeBoolExpression(employeeColumns, "AllowToChangePrice");
            var allowToManageServicePackageExpression = BuildEmployeeBoolExpression(employeeColumns, "AllowToManageServicePackage");
            var allowToManageRedemptionExpression = BuildEmployeeBoolExpression(employeeColumns, "AllowToManageRedemption");
            var allowToVoidExpression = employeeColumns.Contains("AllowToVoid")
                ? "CAST(ISNULL(e.AllowToVoid, 0) AS BIT)"
                : employeeColumns.Contains("AllowToVoID")
                    ? "CAST(ISNULL(e.AllowToVoID, 0) AS BIT)"
                    : "CAST(0 AS BIT)";

            var sql = $@"
            SELECT TOP 1
                e.ID AS Id,
                ISNULL(CAST(e.Number AS NVARCHAR(50)), '') AS Number,
                ISNULL(CAST(e.FirstName AS NVARCHAR(100)), '') AS FirstName,
                ISNULL(CAST(e.LastName AS NVARCHAR(100)), '') AS LastName,
                ISNULL(CAST(e.Username AS NVARCHAR(100)), '') AS Username,
                ISNULL(CAST(e.Passcode AS NVARCHAR(20)), '') AS Passcode,
                ISNULL(e.RoleID, 0) AS RoleId,
                ISNULL(e.EmployeeDepartmentID, 0) AS EmployeeDepartmentId,
                ISNULL(e.SupervisorID, 0) AS SupervisorId,
                ISNULL(CAST(e.HPNum AS NVARCHAR(100)), '') AS HPNum,
                ISNULL(CAST(e.EmailAddress AS NVARCHAR(255)), '') AS EmailAddress,
                ISNULL(e.HR_TimeCategoryID, 0) AS HrTimeCategoryId,
                {allowToChangePriceExpression} AS AllowToChangePrice,
                {allowToManageServicePackageExpression} AS AllowToManageServicePackage,
                {allowToManageRedemptionExpression} AS AllowToManageRedemption,
                {allowToVoidExpression} AS AllowToVoid,
                CAST(ISNULL(e.Inactive, 0) AS BIT) AS Inactive,
                p.DateHired AS DateHired,
                CAST(CASE WHEN p.DateContractEnd IS NOT NULL AND p.DateContractEnd > '1901-01-01' THEN 1 ELSE 0 END AS BIT) AS HasContractEnd,
                p.DateContractEnd AS DateContractEnd,
                ISNULL(CAST(p.NRIC AS NVARCHAR(100)), '') AS Nric,
                ISNULL(CAST(p.Race AS NVARCHAR(100)), '') AS Race,
                ISNULL(CAST(p.CountryOfBirth AS NVARCHAR(100)), '') AS CountryOfBirth,
                p.DOB AS Dob,
                ISNULL(CAST(p.MaritalStatus AS NVARCHAR(100)), '') AS MaritalStatus,
                ISNULL(CAST(p.SpouseName AS NVARCHAR(255)), '') AS SpouseName,
                ISNULL(CAST(p.SpouseContactNo AS NVARCHAR(100)), '') AS SpouseContactNo,
                ISNULL(CAST(p.PayMethod AS NVARCHAR(100)), '') AS PayMethod,
                ISNULL(CAST(p.PayFrequency AS NVARCHAR(100)), '') AS PayFrequency,
                ISNULL(p.RateUsedToBillCustomer, 0) AS RateUsedToBillCustomer,
                ISNULL(p.RateSalary, 0) AS RateSalary,
                ISNULL(p.RateSalaryHourly, 0) AS RateSalaryHourly,
                ISNULL(p.DirectorRemuneration, 0) AS DirectorRemuneration,
                ISNULL(p.CPFEmployee, 0) * 100 AS CpfEmployeePercent,
                ISNULL(p.CPFEmployer, 0) * 100 AS CpfEmployerPercent,
                ISNULL(CAST(p.PaymentMode AS NVARCHAR(100)), '') AS PaymentMode,
                ISNULL(CAST(p.BankAccNo AS NVARCHAR(100)), '') AS BankAccNo,
                ISNULL(p.RateOvertimeWeekday, 0) AS RateOvertimeWeekday,
                ISNULL(p.RateOvertimeWeekend, 0) AS RateOvertimeWeekend,
                ISNULL(p.RateOvertimePublicHoliday, 0) AS RateOvertimePublicHoliday,
                ISNULL(p.RateAllowance_Gen, 0) AS SalesCommission,
                ISNULL(p.Deduction1, 0) AS DeductionUnpaidLeave,
                ISNULL(p.RateAllowance_Transport, 0) AS AllowanceTransport,
                ISNULL(p.Deduction2, 0) AS DeductionOthers,
                ISNULL(p.RateAllowance_Performance, 0) AS AllowancePaidLeave,
                ISNULL(p.Levy, 0) AS ForeignLevy,
                ISNULL(p.RateAllowance_Gen2, 0) AS AllowanceOther,
                ISNULL(p.RateBonus, 0) AS Bonus,
                ISNULL(p.Donation, 0) AS Donation,
                ISNULL(CAST(p.DonationType AS NVARCHAR(100)), '') AS DonationType,
                ISNULL(p.SDL, 0) AS Sdl,
                ISNULL(p.Reimbursement, 0) AS Reimbursement,
                ISNULL(p.DeductionWithoutCPF, 0) AS DeductionWithoutCpf,
                ISNULL(CAST(p.AllowanceRemarks AS NVARCHAR(MAX)), '') AS AllowanceRemarks,
                ISNULL(CAST(p.Notice AS NVARCHAR(MAX)), '') AS Notice
            FROM BIZ_Employee e
            LEFT JOIN HR_EmployeeProperties p ON p.EmployeeID = e.ID AND ISNULL(p.Inactive, 0) = 0
            WHERE e.ID = @Id
              AND {deletedCondition};";

            var employee = await conn.QuerySingleOrDefaultAsync<EmployeeDetailRequest>(sql, new { Id = id });

            if (employee == null)
            {
                return null;
            }

            employee.RoasterYear = roasterYear;
            employee.RoasterMonth = roasterMonth;
            employee.TimeTableDays = await GetEmployeeTimeTableDaysAsync(conn, id);
            employee.DutyRoasterDays = await GetDutyRoasterDaysAsync(conn, id, roasterYear, roasterMonth);
            await FillEmployeeSecurityAsync(conn, employee);

            return employee;
        }

        public async Task<string> GetNextEmployeeNumberAsync()
        {
            const string sql = @"
        SELECT ISNULL(
            MAX(
                CASE
                    WHEN LTRIM(RTRIM(ISNULL([Number], ''))) <> ''
                         AND LTRIM(RTRIM([Number])) NOT LIKE '%[^0-9]%'
                         AND LEN(LTRIM(RTRIM([Number]))) <= 18
                    THEN CONVERT(BIGINT, LTRIM(RTRIM([Number])))
                    ELSE NULL
                END
            ),
            0
        ) + 1
        FROM BIZ_Employee;";

            using var conn = _crmDbConnectionFactory.Create();

            var number = await conn.ExecuteScalarAsync<long>(sql);

            return number.ToString();
        }

        public async Task<List<EmployeeOptionDto>> GetRoleOptionsAsync()
        {
            const string sql = @"
            SELECT 0 AS Value, 'All Roles' AS Text
            UNION ALL
            SELECT ID AS Value, ISNULL(CAST(RoleName AS NVARCHAR(255)), '') AS Text
            FROM BIZ_EmployeeRoles
            WHERE ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            return await QueryOptionsAsync(sql);
        }

        public async Task<List<EmployeeOptionDto>> GetDepartmentOptionsAsync()
        {
            const string sql = @"
            SELECT 0 AS Value, 'None' AS Text
            UNION ALL
            SELECT ID AS Value, ISNULL(CAST([Name] AS NVARCHAR(255)), '') AS Text
            FROM BIZ_EmployeeDepartment
            WHERE ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            return await QueryOptionsAsync(sql);
        }

        public async Task<List<EmployeeOptionDto>> GetSupervisorOptionsAsync()
        {
            const string sql = @"
            SELECT 0 AS Value, 'None' AS Text
            UNION ALL
            SELECT ID AS Value, ISNULL(NULLIF(DisplayName, ''), ISNULL(Username, '')) AS Text
            FROM BIZ_Employee
            WHERE ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            return await QueryOptionsAsync(sql);
        }

        public async Task<List<EmployeeOptionDto>> GetTimeCategoryOptionsAsync()
        {
            const string sql = @"
            SELECT 0 AS Value, '- Please Select -' AS Text
            UNION ALL
            SELECT ID AS Value, ISNULL(CAST(Category AS NVARCHAR(255)), '') AS Text
            FROM HR_TimeCategory
            WHERE ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            return await QueryOptionsAsync(sql);
        }

        public async Task<List<EmployeeOptionDto>> GetWorkLocationOptionsAsync()
        {
            const string sql = @"
            SELECT ID AS Value, ISNULL(CAST(StoreName AS NVARCHAR(255)), '') AS Text
            FROM BIZ_Stores
            WHERE ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            return await QueryOptionsAsync(sql);
        }

        public async Task<List<EmployeeCustomTimeTableDto>> GetCustomTimeTablesAsync(int employeeId)
        {
            const string sql = @"
            IF OBJECT_ID('HR_EmployeeCustomTimeTable', 'U') IS NOT NULL
            BEGIN
                SELECT
                    H.ID AS Id,
                    H.EmployeeID AS EmployeeId,
                    ISNULL(H.LocationID, 0) AS LocationId,
                    ISNULL(CAST(S.StoreName AS NVARCHAR(255)), '') AS LocationName,
                    H.CustomDate AS CustomDate,
                    H.StartTime AS StartTime,
                    H.EndTime AS EndTime
                FROM HR_EmployeeCustomTimeTable H
                LEFT JOIN BIZ_Stores S ON S.ID = H.LocationID
                WHERE ISNULL(H.Inactive, 0) = 0
                  AND H.EmployeeID = @EmployeeId
                ORDER BY H.CustomDate DESC, H.StartTime ASC;
            END;";

            using var conn = _crmDbConnectionFactory.Create();
            var result = await conn.QueryAsync<EmployeeCustomTimeTableDto>(sql, new { EmployeeId = employeeId });

            return result.ToList();
        }

        public async Task<int> CreateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            using var conn = _crmDbConnectionFactory.Create();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                await ValidateDuplicateAsync(conn, transaction, request.Id, request.Username, request.Passcode);

                var employeeId = await InsertEmployeeAsync(conn, transaction, request, hqId, storeId);
                request.Id = employeeId;

                await SaveSecurityAsync(conn, transaction, request, employeeId);
                await SaveEmployeePropertiesAsync(conn, transaction, request, employeeId, hqId, storeId);
                await SaveEmployeeLeaveSummaryAsync(conn, transaction, employeeId);
                await SaveEmployeeTimeTableAsync(conn, transaction, request, employeeId, storeId);
                await SaveDutyRoasterAsync(conn, transaction, request, employeeId);
                await SaveAuditAsync(conn, transaction, "BIZ_Employee", employeeId, currentUserId, currentUserName, "Add Record", hqId, storeId);

                transaction.Commit();

                return employeeId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeDetailRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            using var conn = _crmDbConnectionFactory.Create();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                await ValidateDuplicateAsync(conn, transaction, request.Id, request.Username, request.Passcode);

                var affected = await UpdateEmployeeCoreAsync(conn, transaction, request, hqId, storeId);

                if (affected <= 0)
                {
                    transaction.Rollback();
                    return false;
                }

                await SaveSecurityAsync(conn, transaction, request, request.Id);
                await SaveEmployeePropertiesAsync(conn, transaction, request, request.Id, hqId, storeId);
                await SaveEmployeeLeaveSummaryAsync(conn, transaction, request.Id);
                await SaveEmployeeTimeTableAsync(conn, transaction, request, request.Id, storeId);
                await SaveDutyRoasterAsync(conn, transaction, request, request.Id);
                await SaveAuditAsync(conn, transaction, "BIZ_Employee", request.Id, currentUserId, currentUserName, "Update Record", hqId, storeId);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> AddCustomTimeTableAsync(EmployeeCustomTimeTableRequest request, int hqId, int storeId, int currentUserId, string currentUserName)
        {
            const string sql = @"
            IF OBJECT_ID('HR_EmployeeCustomTimeTable', 'U') IS NULL
            BEGIN
                SELECT 0;
                RETURN;
            END;

            DECLARE @EmployeeName NVARCHAR(255) = (
                SELECT TOP 1 ISNULL(NULLIF(DisplayName, ''), ISNULL(Username, ''))
                FROM BIZ_Employee
                WHERE ID = @EmployeeId
            );

            INSERT INTO HR_EmployeeCustomTimeTable
            (
                HQID,
                StoreID,
                LocationID,
                EmployeeID,
                EmployeeName,
                CustomDate,
                StartTime,
                EndTime,
                LastUpdated
            )
            VALUES
            (
                @HqId,
                @LocationId,
                @LocationId,
                @EmployeeId,
                @EmployeeName,
                @CustomDate,
                @StartTime,
                @EndTime,
                GETDATE()
            );

            SELECT CONVERT(INT, SCOPE_IDENTITY());";

            using var conn = _crmDbConnectionFactory.Create();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                var customDate = request.CustomDate!.Value.Date;
                var newId = await conn.ExecuteScalarAsync<int>(sql, new
                {
                    HqId = hqId,
                    StoreId = storeId,
                    request.EmployeeId,
                    request.LocationId,
                    CustomDate = customDate,
                    StartTime = BuildTimeValue(customDate, request.StartTime),
                    EndTime = BuildTimeValue(customDate, request.EndTime)
                }, transaction);

                if (newId > 0)
                {
                    await SaveAuditAsync(conn, transaction, "HR_EmployeeCustomTimeTable", newId, currentUserId, currentUserName, "Add Record", hqId, storeId);
                }

                transaction.Commit();

                return newId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteCustomTimeTableAsync(int id, int currentUserId, string currentUserName)
        {
            const string sql = @"
            IF OBJECT_ID('HR_EmployeeCustomTimeTable', 'U') IS NULL
            BEGIN
                SELECT 0;
                RETURN;
            END;

            UPDATE HR_EmployeeCustomTimeTable
            SET
                Inactive = 1,
                LastUpdated = GETDATE()
            WHERE ID = @Id;

            SELECT @@ROWCOUNT;";

            using var conn = _crmDbConnectionFactory.Create();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                var affected = await conn.ExecuteScalarAsync<int>(sql, new { Id = id }, transaction);
                if (affected > 0)
                {
                    await SaveAuditAsync(conn, transaction, "HR_EmployeeCustomTimeTable", id, currentUserId, currentUserName, "Remove Record", 0, 0);
                }

                transaction.Commit();

                return affected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<int> InsertEmployeeAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int hqId, int storeId)
        {
            var columns = await GetTableColumnsAsync(conn, "BIZ_Employee", transaction);
            var values = await BuildEmployeeValuesAsync(conn, transaction, columns, request, hqId, storeId, includePassword: true);

            var parameters = new DynamicParameters();
            var sqlColumns = new List<string>();
            var sqlValues = new List<string>();
            var index = 0;

            foreach (var value in values)
            {
                var parameterName = $"p{index}";
                sqlColumns.Add($"[{value.Key}]");
                sqlValues.Add($"@{parameterName}");
                parameters.Add(parameterName, value.Value);
                index++;
            }

            var sql = $@"
            INSERT INTO BIZ_Employee
            (
                {string.Join(",\n                ", sqlColumns)}
            )
            VALUES
            (
                {string.Join(",\n                ", sqlValues)}
            );

            SELECT CONVERT(INT, SCOPE_IDENTITY());";

            return await conn.ExecuteScalarAsync<int>(sql, parameters, transaction);
        }

        private async Task<int> UpdateEmployeeCoreAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int hqId, int storeId)
        {
            var columns = await GetTableColumnsAsync(conn, "BIZ_Employee", transaction);
            var values = await BuildEmployeeValuesAsync(conn, transaction, columns, request, hqId, storeId, includePassword: !string.IsNullOrWhiteSpace(request.Password));
            var parameters = new DynamicParameters();
            var setClauses = new List<string>();
            var index = 0;

            foreach (var value in values)
            {
                var parameterName = $"p{index}";
                setClauses.Add($"[{value.Key}] = @{parameterName}");
                parameters.Add(parameterName, value.Value);
                index++;
            }

            parameters.Add("Id", request.Id);

            var sql = $@"
            UPDATE BIZ_Employee
            SET
                {string.Join(",\n                ", setClauses)}
            WHERE ID = @Id;";

            return await conn.ExecuteAsync(sql, parameters, transaction);
        }

        private async Task<Dictionary<string, object?>> BuildEmployeeValuesAsync(
            IDbConnection conn,
            IDbTransaction transaction,
            IReadOnlySet<string> columns,
            EmployeeDetailRequest request,
            int hqId,
            int storeId,
            bool includePassword)
        {
            var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var displayName = $"{request.FirstName?.Trim()} {request.LastName?.Trim()}".Trim();
            var roleName = await GetLookupTextAsync(conn, transaction, "BIZ_EmployeeRole", "RoleName", request.RoleId);
            var departmentName = await GetLookupTextAsync(conn, transaction, "BIZ_EmployeeDepartment", "Name", request.EmployeeDepartmentId);
            var supervisorName = await GetEmployeeDisplayNameAsync(conn, transaction, request.SupervisorId);
            var hrCategory = await GetLookupTextAsync(conn, transaction, "HR_TimeCategory", "Category", request.HrTimeCategoryId);

            AddValue(values, columns, "HQID", hqId);
            AddValue(values, columns, "StoreID", storeId);
            AddValue(values, columns, "Number", request.Number?.Trim() ?? string.Empty);
            AddValue(values, columns, "FirstName", request.FirstName?.Trim() ?? string.Empty);
            AddValue(values, columns, "LastName", request.LastName?.Trim() ?? string.Empty);
            AddValue(values, columns, "DisplayName", displayName);
            AddValue(values, columns, "Username", request.Username?.Trim() ?? string.Empty);

            if (includePassword)
            {
                AddValue(values, columns, "Password", request.Password?.Trim() ?? string.Empty);
            }

            AddValue(values, columns, "Passcode", request.Passcode?.Trim() ?? string.Empty);
            AddValue(values, columns, "RoleID", request.RoleId);
            AddValue(values, columns, "RoleName", roleName);
            AddValue(values, columns, "EmployeeDepartmentID", request.EmployeeDepartmentId);
            AddValue(values, columns, "Department", departmentName);
            AddValue(values, columns, "SupervisorID", request.SupervisorId);
            AddValue(values, columns, "SupervisorDisplayName", supervisorName);
            AddValue(values, columns, "HPNum", request.HPNum?.Trim() ?? string.Empty);
            AddValue(values, columns, "EmailAddress", request.EmailAddress?.Trim() ?? string.Empty);
            AddValue(values, columns, "HR_TimeCategoryID", request.HrTimeCategoryId);
            AddValue(values, columns, "HRCategory", hrCategory);
            AddValue(values, columns, "AllowToChangePrice", request.AllowToChangePrice);
            AddValue(values, columns, "AllowToManageServicePackage", request.AllowToManageServicePackage);
            AddValue(values, columns, "AllowToManageRedemption", request.AllowToManageRedemption);
            AddValue(values, columns, "AllowToVoid", request.AllowToVoid);
            AddValue(values, columns, "AllowToVoID", request.AllowToVoid);
            AddValue(values, columns, "Inactive", request.Inactive);
            AddValue(values, columns, "IsDeleted", false);
            AddValue(values, columns, "LastUpdated", DateTime.Now);

            return values;
        }

        private static void AddValue(Dictionary<string, object?> values, IReadOnlySet<string> columns, string columnName, object? value)
        {
            if (columns.Contains(columnName))
            {
                values[columnName] = value;
            }
        }

        private async Task ValidateDuplicateAsync(IDbConnection conn, IDbTransaction transaction, int id, string username, string passcode)
        {
            const string usernameSql = @"
            SELECT COUNT(1)
            FROM BIZ_Employee
            WHERE ID <> @Id
              AND ISNULL(IsDeleted, 0) = 0
              AND Username = @Username;";

            var usernameCount = await conn.ExecuteScalarAsync<int>(
                usernameSql,
                new
                {
                    Id = id,
                    Username = username.Trim()
                },
                transaction);

            if (usernameCount > 0)
            {
                throw new InvalidOperationException("Could not save employee. Duplicate Username is found.");
            }

            const string passcodeSql = @"
            SELECT COUNT(1)
            FROM BIZ_Employee
            WHERE ID <> @Id
              AND ISNULL(IsDeleted, 0) = 0
              AND Passcode = @Passcode;";

            var passcodeCount = await conn.ExecuteScalarAsync<int>(
                passcodeSql,
                new
                {
                    Id = id,
                    Passcode = passcode.Trim()
                },
                transaction);

            if (passcodeCount > 0)
            {
                throw new InvalidOperationException("Could not save employee. Duplicate PassCode is found.");
            }
        }

        private async Task SaveEmployeePropertiesAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int employeeId, int hqId, int storeId)
        {
            if (!await TableExistsAsync(conn, "HR_EmployeeProperties", transaction))
            {
                return;
            }

            var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["DateHired"] = request.DateHired ?? DateTime.Today,
                ["DateContractEnd"] = request.HasContractEnd ? request.DateContractEnd ?? DefaultDate() : DefaultDate(),
                ["DOB"] = request.Dob ?? DefaultDate(),
                ["NRIC"] = request.Nric?.Trim() ?? string.Empty,
                ["Race"] = request.Race?.Trim() ?? string.Empty,
                ["CountryOfBirth"] = request.CountryOfBirth?.Trim() ?? string.Empty,
                ["MaritalStatus"] = request.MaritalStatus ?? string.Empty,
                ["SpouseName"] = request.SpouseName?.Trim() ?? string.Empty,
                ["SpouseContactNo"] = request.SpouseContactNo?.Trim() ?? string.Empty,
                ["HR_TimeCategoryID"] = request.HrTimeCategoryId,
                ["PayMethod"] = request.PayMethod ?? string.Empty,
                ["PayFrequency"] = request.PayFrequency ?? string.Empty,
                ["RateUsedToBillCustomer"] = request.RateUsedToBillCustomer,
                ["RateSalary"] = request.RateSalary,
                ["RateSalaryHourly"] = request.RateSalaryHourly,
                ["DirectorRemuneration"] = request.DirectorRemuneration,
                ["CPFEmployee"] = request.CpfEmployeePercent / 100m,
                ["CPFEmployer"] = request.CpfEmployerPercent / 100m,
                ["PaymentMode"] = request.PaymentMode ?? string.Empty,
                ["BankAccNo"] = request.BankAccNo?.Trim() ?? string.Empty,
                ["RateOvertimeWeekday"] = request.RateOvertimeWeekday,
                ["RateOvertimeWeekend"] = request.RateOvertimeWeekend,
                ["RateOvertimePublicHoliday"] = request.RateOvertimePublicHoliday,
                ["RateAllowance_Gen"] = request.SalesCommission,
                ["Deduction1"] = request.DeductionUnpaidLeave,
                ["RateAllowance_Transport"] = request.AllowanceTransport,
                ["Deduction2"] = request.DeductionOthers,
                ["RateAllowance_Performance"] = request.AllowancePaidLeave,
                ["Levy"] = request.ForeignLevy,
                ["RateAllowance_Gen2"] = request.AllowanceOther,
                ["RateBonus"] = request.Bonus,
                ["SDL"] = request.Sdl,
                ["Donation"] = request.Donation,
                ["DonationType"] = request.DonationType ?? string.Empty,
                ["Reimbursement"] = request.Reimbursement,
                ["DeductionWithoutCPF"] = request.DeductionWithoutCpf,
                ["AllowanceRemarks"] = request.AllowanceRemarks ?? string.Empty,
                ["Notice"] = request.Notice ?? string.Empty,
                ["AnnualVacationalLeave"] = 0,
                ["AnnualMedicalLeave"] = 0,
                ["BalanceVacationalLeave"] = 0,
                ["BalanceMedicalLeave"] = 0,
                ["MaxHours"] = 0,
                ["DailyIncentive"] = 0,
                ["BankCharges"] = 0,
                ["HQID"] = hqId,
                ["StoreID"] = storeId,
                ["Inactive"] = false,
                ["LastUpdated"] = DateTime.Now
            };

            await UpsertByEmployeeIdAsync(conn, transaction, "HR_EmployeeProperties", employeeId, values);
        }

        private async Task SaveEmployeeLeaveSummaryAsync(IDbConnection conn, IDbTransaction transaction, int employeeId)
        {
            if (!await TableExistsAsync(conn, "HR_LeaveSummary", transaction) ||
                !await TableExistsAsync(conn, "HR_LeaveType", transaction))
            {
                return;
            }

            const string sql = @"
            INSERT INTO HR_LeaveSummary
            (
                EmployeeID,
                [Year],
                Allocation,
                [Type],
                AllowToBringForward,
                LastUpdated
            )
            SELECT
                @EmployeeId,
                YEAR(GETDATE()),
                ISNULL(AllowancePaid, 0),
                ISNULL(Title, ''),
                ISNULL(IsBroughtForward, 0),
                GETDATE()
            FROM HR_LeaveType lt
            WHERE ISNULL(lt.Inactive, 0) = 0
              AND NOT EXISTS
              (
                    SELECT 1
                    FROM HR_LeaveSummary ls
                    WHERE ISNULL(ls.Inactive, 0) = 0
                      AND ls.EmployeeID = @EmployeeId
                      AND ls.[Year] = YEAR(GETDATE())
                      AND ls.[Type] = lt.Title
              );";

            await conn.ExecuteAsync(sql, new { EmployeeId = employeeId }, transaction);
        }

        private async Task SaveEmployeeTimeTableAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int employeeId, int storeId)
        {
            if (!await TableExistsAsync(conn, "HR_EmployeeTimeTable", transaction))
            {
                return;
            }

            var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["StoreID"] = storeId,
                ["Inactive"] = false,
                ["LastUpdated"] = DateTime.Now
            };

            foreach (var day in EnsureSevenDays(request.TimeTableDays))
            {
                var prefix = day.DayName;
                values[$"{prefix}Working"] = day.Working;
                values[$"{prefix}WorkLocationID"] = day.Working ? day.LocationId : 0;
                values[$"{prefix}StartTime"] = day.Working ? BuildTimeValue(DateTime.Today, day.StartTime) : null;
                values[$"{prefix}EndTime"] = day.Working ? BuildTimeValue(DateTime.Today, day.EndTime) : null;
            }

            await UpsertByEmployeeIdAsync(conn, transaction, "HR_EmployeeTimeTable", employeeId, values);
        }

        private async Task SaveSecurityAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int employeeId)
        {
            if (!await TableExistsAsync(conn, "BIZ_Security", transaction))
            {
                return;
            }

            var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["modHome"] = request.ModHome,
                ["modAdmin"] = request.ModAdmin,
                ["modOutlet"] = request.ModOutlet,
                ["modDental"] = request.ModDental,
                ["modPOS"] = request.ModPos,
                ["modAMS"] = request.ModAms,
                ["modCRM"] = request.ModCrm,
                ["modHR"] = request.ModHr,
                ["modINV"] = request.ModInv,
                ["modSP"] = request.ModSp,
                ["modMED"] = request.ModMed,
                ["modFIN"] = request.ModFin,
                ["modBI"] = request.ModBi,
                ["modECOM"] = request.ModEcom,
                ["modLogistic"] = request.ModLogistic,
                ["LastUpdated"] = DateTime.Now
            };

            await UpsertByEmployeeIdAsync(conn, transaction, "BIZ_Security", employeeId, values);
        }

        private async Task SaveDutyRoasterAsync(IDbConnection conn, IDbTransaction transaction, EmployeeDetailRequest request, int employeeId)
        {
            if (!await TableExistsAsync(conn, "HR_EmployeeDutyRoaster", transaction) || request.DutyRoasterDays == null)
            {
                return;
            }

            const string findSql = @"
            SELECT TOP 1 ID
            FROM HR_EmployeeDutyRoaster
            WHERE ISNULL(Inactive, 0) = 0
              AND EmployeeID = @EmployeeId
              AND CAST(Period AS DATE) = @WorkDate;";

            const string insertSql = @"
            INSERT INTO HR_EmployeeDutyRoaster
            (
                EmployeeID,
                Period,
                OnDuty,
                Remarks,
                LastUpdated
            )
            VALUES
            (
                @EmployeeId,
                @WorkDate,
                @OnDuty,
                @Remarks,
                GETDATE()
            );";

            const string updateSql = @"
            UPDATE HR_EmployeeDutyRoaster
            SET
                Period = @WorkDate,
                OnDuty = @OnDuty,
                Remarks = @Remarks,
                LastUpdated = GETDATE()
            WHERE ID = @Id;";

            foreach (var day in request.DutyRoasterDays)
            {
                var id = day.Id;

                if (id <= 0)
                {
                    id = await conn.ExecuteScalarAsync<int?>(
                        findSql,
                        new
                        {
                            EmployeeId = employeeId,
                            WorkDate = day.WorkDate.Date
                        },
                        transaction) ?? 0;
                }

                var parameters = new
                {
                    Id = id,
                    EmployeeId = employeeId,
                    WorkDate = day.WorkDate.Date,
                    day.OnDuty,
                    Remarks = day.OnDuty ? string.Empty : day.Remarks?.Trim() ?? string.Empty
                };

                if (id > 0)
                {
                    await conn.ExecuteAsync(updateSql, parameters, transaction);
                }
                else
                {
                    await conn.ExecuteAsync(insertSql, parameters, transaction);
                }
            }
        }

        private async Task UpsertByEmployeeIdAsync(IDbConnection conn, IDbTransaction transaction, string tableName, int employeeId, Dictionary<string, object?> values)
        {
            var columns = await GetTableColumnsAsync(conn, tableName, transaction);
            var filtered = values
                .Where(value => columns.Contains(value.Key))
                .ToList();

            if (filtered.Count == 0)
            {
                return;
            }

            var existingId = await conn.ExecuteScalarAsync<int?>(
                $"SELECT TOP 1 ID FROM {tableName} WHERE EmployeeID = @EmployeeId AND ISNULL(Inactive, 0) = 0 ORDER BY ID DESC;",
                new { EmployeeId = employeeId },
                transaction) ?? 0;

            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", employeeId);
            parameters.Add("Id", existingId);

            for (var i = 0; i < filtered.Count; i++)
            {
                parameters.Add($"p{i}", filtered[i].Value);
            }

            if (existingId > 0)
            {
                var setClauses = filtered
                    .Select((value, i) => $"[{value.Key}] = @p{i}")
                    .ToList();

                var updateSql = $@"
                UPDATE {tableName}
                SET {string.Join(", ", setClauses)}
                WHERE ID = @Id;";

                await conn.ExecuteAsync(updateSql, parameters, transaction);
            }
            else
            {
                var insertColumns = new List<string> { "[EmployeeID]" };
                var insertValues = new List<string> { "@EmployeeId" };

                for (var i = 0; i < filtered.Count; i++)
                {
                    insertColumns.Add($"[{filtered[i].Key}]");
                    insertValues.Add($"@p{i}");
                }

                var insertSql = $@"
                INSERT INTO {tableName}
                (
                    {string.Join(", ", insertColumns)}
                )
                VALUES
                (
                    {string.Join(", ", insertValues)}
                );";

                await conn.ExecuteAsync(insertSql, parameters, transaction);
            }
        }

        private async Task<List<EmployeeTimeTableDayRequest>> GetEmployeeTimeTableDaysAsync(IDbConnection conn, int employeeId)
        {
            var result = BuildDefaultTimeTableDays();

            if (!await TableExistsAsync(conn, "HR_EmployeeTimeTable"))
            {
                return result;
            }

            const string sql = @"
            SELECT TOP 1 *
            FROM HR_EmployeeTimeTable
            WHERE ISNULL(Inactive, 0) = 0
              AND EmployeeID = @EmployeeId
            ORDER BY ID DESC;";

            var row = await conn.QuerySingleOrDefaultAsync(sql, new { EmployeeId = employeeId });

            if (row == null)
            {
                return result;
            }

            var data = (IDictionary<string, object>)row;

            foreach (var day in result)
            {
                day.Working = ReadBool(data, $"{day.DayName}Working");
                day.LocationId = ReadInt(data, $"{day.DayName}WorkLocationID");
                day.StartTime = ReadTime(data, $"{day.DayName}StartTime");
                day.EndTime = ReadTime(data, $"{day.DayName}EndTime");
            }

            return result;
        }

        private async Task<List<EmployeeDutyRoasterDayRequest>> GetDutyRoasterDaysAsync(IDbConnection conn, int employeeId, int year, int month)
        {
            var result = BuildDefaultDutyRoasterDays(year, month);

            if (!await TableExistsAsync(conn, "HR_EmployeeDutyRoaster"))
            {
                return result;
            }

            const string sql = @"
            SELECT ID, Period AS WorkDate, CAST(ISNULL(OnDuty, 1) AS BIT) AS OnDuty, ISNULL(CAST(Remarks AS NVARCHAR(255)), '') AS Remarks
            FROM HR_EmployeeDutyRoaster
            WHERE ISNULL(Inactive, 0) = 0
              AND EmployeeID = @EmployeeId
              AND YEAR(Period) = @Year
              AND MONTH(Period) = @Month;";

            var savedDays = (await conn.QueryAsync<EmployeeDutyRoasterDayRequest>(
                sql,
                new
                {
                    EmployeeId = employeeId,
                    Year = year,
                    Month = month
                })).ToList();

            foreach (var savedDay in savedDays)
            {
                var existing = result.FirstOrDefault(x => x.WorkDate.Date == savedDay.WorkDate.Date);

                if (existing == null)
                {
                    continue;
                }

                existing.Id = savedDay.Id;
                existing.OnDuty = savedDay.OnDuty;
                existing.Remarks = savedDay.Remarks;
            }

            return result;
        }

        private async Task FillEmployeeSecurityAsync(IDbConnection conn, EmployeeDetailRequest employee)
        {
            if (!await TableExistsAsync(conn, "BIZ_Security"))
            {
                return;
            }

            const string sql = @"
            SELECT TOP 1 *
            FROM BIZ_Security
            WHERE EmployeeID = @EmployeeId
            ORDER BY ID DESC;";

            var row = await conn.QuerySingleOrDefaultAsync(sql, new { EmployeeId = employee.Id });

            if (row == null)
            {
                return;
            }

            var data = (IDictionary<string, object>)row;
            employee.ModHome = ReadBool(data, "modHome");
            employee.ModAdmin = ReadBool(data, "modAdmin");
            employee.ModOutlet = ReadBool(data, "modOutlet");
            employee.ModDental = ReadBool(data, "modDental");
            employee.ModPos = ReadBool(data, "modPOS");
            employee.ModAms = ReadBool(data, "modAMS");
            employee.ModCrm = ReadBool(data, "modCRM");
            employee.ModHr = ReadBool(data, "modHR");
            employee.ModInv = ReadBool(data, "modINV");
            employee.ModSp = ReadBool(data, "modSP");
            employee.ModMed = ReadBool(data, "modMED");
            employee.ModFin = ReadBool(data, "modFIN");
            employee.ModBi = ReadBool(data, "modBI");
            employee.ModEcom = ReadBool(data, "modECOM");
            employee.ModLogistic = ReadBool(data, "modLogistic");
        }

        private async Task<List<EmployeeOptionDto>> QueryOptionsAsync(string sql)
        {
            using var conn = _crmDbConnectionFactory.Create();
            var result = await conn.QueryAsync<EmployeeOptionDto>(sql);

            return result.ToList();
        }

        private static async Task<HashSet<string>> GetTableColumnsAsync(IDbConnection conn, string tableName, IDbTransaction? transaction = null)
        {
            const string sql = @"
            SELECT name
            FROM sys.columns
            WHERE object_id = OBJECT_ID(@TableName);";

            var columns = await conn.QueryAsync<string>(sql, new { TableName = tableName }, transaction);

            return columns.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static async Task<bool> TableExistsAsync(IDbConnection conn, string tableName, IDbTransaction? transaction = null)
        {
            const string sql = @"
            SELECT CASE WHEN OBJECT_ID(@TableName, 'U') IS NULL THEN 0 ELSE 1 END;";

            return await conn.ExecuteScalarAsync<int>(sql, new { TableName = tableName }, transaction) == 1;
        }

        private static string BuildDeletedCondition(IReadOnlySet<string> columns, string alias)
        {
            return columns.Contains("IsDeleted")
                ? $"ISNULL({alias}.IsDeleted, 0) = 0"
                : "1 = 1";
        }

        private static string BuildEmployeeBoolExpression(IReadOnlySet<string> columns, string columnName)
        {
            return columns.Contains(columnName)
                ? $"CAST(ISNULL(e.[{columnName}], 0) AS BIT)"
                : "CAST(0 AS BIT)";
        }

        private static EmployeeQueryDefinition BuildEmployeeQueryDefinition(IReadOnlySet<string> columns)
        {
            var usernameExpression = BuildEmployeeTextExpression(columns, "Username", "''");
            var displayNameExpression = columns.Contains("DisplayName")
                ? $"ISNULL(NULLIF({BuildEmployeeTextExpression(columns, "DisplayName", "''")}, ''), {usernameExpression})"
                : usernameExpression;

            var numberExpression = columns.Contains("Number")
                ? BuildEmployeeTextExpression(columns, "Number", usernameExpression)
                : usernameExpression;

            var roleNameExpression = columns.Contains("RoleName")
                ? BuildEmployeeTextExpression(columns, "RoleName", "''")
                : BuildEmployeeTextExpression(columns, "RoleID", "''");

            var departmentExpression = columns.Contains("Department")
                ? BuildEmployeeTextExpression(columns, "Department", "''")
                : "ISNULL((SELECT TOP 1 ISNULL(CAST(d.[Name] AS NVARCHAR(255)), '') FROM BIZ_EmployeeDepartment d WHERE d.ID = e.EmployeeDepartmentID), '')";

            var hrCategoryExpression = columns.Contains("HRCategory")
                ? BuildEmployeeTextExpression(columns, "HRCategory", "''")
                : "ISNULL((SELECT TOP 1 ISNULL(CAST(t.Category AS NVARCHAR(255)), '') FROM HR_TimeCategory t WHERE t.ID = e.HR_TimeCategoryID), '')";

            return new EmployeeQueryDefinition
            {
                NumberExpression = numberExpression,
                DisplayNameExpression = displayNameExpression,
                DepartmentExpression = departmentExpression,
                RoleNameExpression = roleNameExpression,
                HPNumExpression = BuildEmployeeHpExpression(columns),
                SupervisorExpression = BuildSupervisorExpression(columns),
                HRCategoryExpression = hrCategoryExpression,
                EmailAddressExpression = BuildEmployeeTextExpression(columns, "EmailAddress", "''")
            };
        }

        private static string BuildEmployeeHpExpression(IReadOnlySet<string> columns)
        {
            var candidates = new[]
            {
                "HPNum",
                "HpNum",
                "HandPhone",
                "Mobile",
                "MobileNo",
                "Phone",
                "PhoneNo"
            };

            foreach (var candidate in candidates)
            {
                if (columns.Contains(candidate))
                {
                    return BuildEmployeeTextExpression(columns, candidate, "''");
                }
            }

            return "''";
        }

        private static string BuildSupervisorExpression(IReadOnlySet<string> columns)
        {
            if (columns.Contains("SupervisorDisplayName"))
            {
                return BuildEmployeeTextExpression(columns, "SupervisorDisplayName", "''");
            }

            if (columns.Contains("SupervisorID"))
            {
                return "ISNULL((SELECT TOP 1 ISNULL(NULLIF(CAST(s.DisplayName AS NVARCHAR(255)), ''), ISNULL(CAST(s.Username AS NVARCHAR(255)), '')) FROM BIZ_Employee s WHERE s.ID = e.SupervisorID), '')";
            }

            return "''";
        }

        private static string BuildEmployeeTextExpression(
            IReadOnlySet<string> columns,
            string columnName,
            string fallbackExpression)
        {
            if (!columns.Contains(columnName))
            {
                return fallbackExpression;
            }

            return $"ISNULL(CAST(e.[{columnName}] AS NVARCHAR(255)), '')";
        }

        private static string BuildEmployeeOrderByClause(string? sortColumn, string? sortDirection)
        {
            var direction = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            return sortColumn switch
            {
                "Id" => $"Id {direction}, DisplayName ASC",

                "Number" => $"Number {direction}, DisplayName ASC, Id ASC",

                "DisplayName" => $"DisplayName {direction}, Number ASC, Id ASC",

                "Department" => $"Department {direction}, DisplayName ASC, Id ASC",

                "RoleName" => $"RoleName {direction}, DisplayName ASC, Id ASC",

                "HPNum" => $"HPNum {direction}, DisplayName ASC, Id ASC",

                "SupervisorDisplayName" => $"SupervisorDisplayName {direction}, DisplayName ASC, Id ASC",

                "HRCategory" => $"HRCategory {direction}, DisplayName ASC, Id ASC",

                "EmailAddress" => $"EmailAddress {direction}, DisplayName ASC, Id ASC",

                "Inactive" => $"Inactive {direction}, DisplayName ASC, Id ASC",

                _ => "DisplayName ASC, Number ASC, Id ASC"
            };
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

        private static List<EmployeeTimeTableDayRequest> EnsureSevenDays(List<EmployeeTimeTableDayRequest>? days)
        {
            var defaultDays = BuildDefaultTimeTableDays();

            if (days == null || days.Count == 0)
            {
                return defaultDays;
            }

            foreach (var defaultDay in defaultDays)
            {
                var postedDay = days.FirstOrDefault(x => string.Equals(x.DayName, defaultDay.DayName, StringComparison.OrdinalIgnoreCase));

                if (postedDay == null)
                {
                    continue;
                }

                defaultDay.Working = postedDay.Working;
                defaultDay.LocationId = postedDay.LocationId;
                defaultDay.StartTime = postedDay.StartTime;
                defaultDay.EndTime = postedDay.EndTime;
            }

            return defaultDays;
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

        private static DateTime DefaultDate()
        {
            return new DateTime(1900, 1, 1);
        }

        private static DateTime? BuildTimeValue(DateTime date, TimeSpan? time)
        {
            return time.HasValue
                ? date.Date.Add(time.Value)
                : null;
        }

        private static TimeSpan? ReadTime(IDictionary<string, object> data, string columnName)
        {
            if (!data.TryGetValue(columnName, out var value) || value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }

            if (value is DateTime dateTime)
            {
                return dateTime.TimeOfDay;
            }

            if (TimeSpan.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }

            return null;
        }

        private static int ReadInt(IDictionary<string, object> data, string columnName)
        {
            if (!data.TryGetValue(columnName, out var value) || value == null || value == DBNull.Value)
            {
                return 0;
            }

            return int.TryParse(value.ToString(), out var result)
                ? result
                : 0;
        }

        private static bool ReadBool(IDictionary<string, object> data, string columnName)
        {
            if (!data.TryGetValue(columnName, out var value) || value == null || value == DBNull.Value)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            return value.ToString() == "1" || string.Equals(value.ToString(), "true", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> GetLookupTextAsync(IDbConnection conn, IDbTransaction transaction, string tableName, string textColumn, int id)
        {
            if (id <= 0)
            {
                return string.Empty;
            }

            if (!await TableExistsAsync(conn, tableName, transaction))
            {
                return string.Empty;
            }

            var sql = $"SELECT TOP 1 ISNULL(CAST([{textColumn}] AS NVARCHAR(255)), '') FROM {tableName} WHERE ID = @Id;";

            return await conn.ExecuteScalarAsync<string>(sql, new { Id = id }, transaction) ?? string.Empty;
        }

        private static async Task<string> GetEmployeeDisplayNameAsync(IDbConnection conn, IDbTransaction transaction, int employeeId)
        {
            if (employeeId <= 0)
            {
                return string.Empty;
            }

            const string sql = @"
            SELECT TOP 1 ISNULL(NULLIF(DisplayName, ''), ISNULL(Username, ''))
            FROM BIZ_Employee
            WHERE ID = @Id;";

            return await conn.ExecuteScalarAsync<string>(sql, new { Id = employeeId }, transaction) ?? string.Empty;
        }

        private static async Task SaveAuditAsync(IDbConnection conn, IDbTransaction transaction, string tableName, int recordId, int currentUserId, string currentUserName, string action, int hqId, int storeId)
        {
            if (!await TableExistsAsync(conn, "BIZ_AuditTrailLog", transaction))
            {
                return;
            }

            var auditColumns = await GetTableColumnsAsync(conn, "BIZ_AuditTrailLog", transaction);
            var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["LogDate"] = DateTime.Now,
                ["TableName"] = tableName,
                ["RecordID"] = recordId,
                ["EmployeeID"] = currentUserId,
                ["EmployeeName"] = currentUserName,
                ["Action"] = action,
                ["Description"] = action,
                ["HQID"] = hqId,
                ["StoreID"] = storeId
            }
            .Where(value => auditColumns.Contains(value.Key))
            .ToList();

            if (values.Count == 0)
            {
                return;
            }

            var parameters = new DynamicParameters();
            var columns = new List<string>();
            var parameterNames = new List<string>();

            for (var i = 0; i < values.Count; i++)
            {
                columns.Add($"[{values[i].Key}]");
                parameterNames.Add($"@p{i}");
                parameters.Add($"p{i}", values[i].Value);
            }

            var sql = $@"
            INSERT INTO BIZ_AuditTrailLog
            (
                {string.Join(", ", columns)}
            )
            VALUES
            (
                {string.Join(", ", parameterNames)}
            );";

            await conn.ExecuteAsync(sql, parameters, transaction);
        }

        private sealed class EmployeeQueryDefinition
        {
            public string NumberExpression { get; set; } = "''";

            public string DisplayNameExpression { get; set; } = "''";

            public string DepartmentExpression { get; set; } = "''";

            public string RoleNameExpression { get; set; } = "''";

            public string HPNumExpression { get; set; } = "''";

            public string SupervisorExpression { get; set; } = "''";

            public string HRCategoryExpression { get; set; } = "''";

            public string EmailAddressExpression { get; set; } = "''";
        }
    }
}
