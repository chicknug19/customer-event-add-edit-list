using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Admin.StoreList.Request;
using JPP.Models.Admin.StoreList.Responses;

namespace JPP.Data.Repositories
{
    public class StoreListRepository : IStoreListRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public StoreListRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<List<StoreListItemDto>> GetStoreListAsync(StoreListFilterRequest filter)
        {
            var keyword = filter.Keyword?.Trim() ?? string.Empty;
            var status = string.IsNullOrWhiteSpace(filter.Status)
                ? "All"
                : filter.Status.Trim();

            var orderByClause = BuildStoreOrderByClause(filter.SortColumn, filter.SortDirection);

            var sql = $@"
            SELECT
                ID AS Id,
                ISNULL(CAST(Code AS NVARCHAR(100)), '') AS Code,
                ISNULL(CAST(StoreName AS NVARCHAR(255)), '') AS StoreName,
                ISNULL(CAST(Phone AS NVARCHAR(100)), '') AS Phone,
                ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') AS InChargeName,
                CAST(ISNULL(Inactive, 0) AS BIT) AS Inactive
            FROM BIZ_Stores
            WHERE
                (
                    @Keyword = ''
                    OR CAST(ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(Code AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(StoreName AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(Phone AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                )
                AND
                (
                    @Status = 'All'
                    OR (@Status = 'Active' AND ISNULL(Inactive, 0) = 0)
                    OR (@Status = 'Deleted' AND ISNULL(Inactive, 0) = 1)
                )
            ORDER BY
                {orderByClause}
            OFFSET @Skip ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<StoreListItemDto>(
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

        public async Task<StoreListSummaryDto> GetStoreListSummaryAsync(StoreListFilterRequest filter)
        {
            var keyword = filter.Keyword?.Trim() ?? string.Empty;
            var status = string.IsNullOrWhiteSpace(filter.Status)
                ? "All"
                : filter.Status.Trim();

            const string sql = @"
            SELECT
                COUNT(1) AS TotalStore,
                ISNULL(SUM(CASE WHEN ISNULL(Inactive, 0) = 0 THEN 1 ELSE 0 END), 0) AS ActiveStore,
                ISNULL(SUM(CASE WHEN ISNULL(Inactive, 0) = 1 THEN 1 ELSE 0 END), 0) AS DeletedStore
            FROM BIZ_Stores
            WHERE
                (
                    @Keyword = ''
                    OR CAST(ID AS NVARCHAR(50)) LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(Code AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(StoreName AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(Phone AS NVARCHAR(100)), '') LIKE '%' + @Keyword + '%'
                    OR ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') LIKE '%' + @Keyword + '%'
                )
                AND
                (
                    @Status = 'All'
                    OR (@Status = 'Active' AND ISNULL(Inactive, 0) = 0)
                    OR (@Status = 'Deleted' AND ISNULL(Inactive, 0) = 1)
                );";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QuerySingleOrDefaultAsync<StoreListSummaryDto>(
                sql,
                new
                {
                    Keyword = keyword,
                    Status = status
                });

            return result ?? new StoreListSummaryDto();
        }

        public async Task<StoreListItemDto?> GetStoreByIdAsync(int id)
        {
            const string sql = @"
            SELECT TOP 1
                ID AS Id,
                ISNULL(CAST(Code AS NVARCHAR(100)), '') AS Code,
                ISNULL(CAST(StoreName AS NVARCHAR(255)), '') AS StoreName,
                ISNULL(CAST(Phone AS NVARCHAR(100)), '') AS Phone,
                ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') AS InChargeName,
                CAST(ISNULL(Inactive, 0) AS BIT) AS Inactive
            FROM BIZ_Stores
            WHERE ID = @Id;";

            using var conn = _crmDbConnectionFactory.Create();

            return await conn.QuerySingleOrDefaultAsync<StoreListItemDto>(
                sql,
                new
                {
                    Id = id
                });
        }

        public async Task<bool> SoftDeleteStoreAsync(int id)
        {
            const string sql = @"
            UPDATE BIZ_Stores
            SET
                Inactive = 1,
                LastUpdated = GETDATE()
            WHERE ID = @Id;";

            using var conn = _crmDbConnectionFactory.Create();

            var affectedRows = await conn.ExecuteAsync(
                sql,
                new
                {
                    Id = id
                });

            return affectedRows > 0;
        }

        public async Task<StoreDetailDto?> GetStoreDetailByIdAsync(int id)
        {
            const string sql = @"
            SELECT TOP 1
                ID AS Id,
                ISNULL(CAST(Code AS NVARCHAR(100)), '') AS Code,
                ISNULL(CAST(StoreName AS NVARCHAR(255)), '') AS StoreName,
                CAST(ISNULL(IsPublished, 0) AS BIT) AS IsPublished,
                ISNULL(InChargeID, 0) AS InChargeId,
                ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') AS InChargeName,
                ISNULL(CAST(BIZRegNo AS NVARCHAR(100)), '') AS BizRegNo,
                ISNULL(CAST(BlockHouseNo AS NVARCHAR(100)), '') AS BlockHouseNo,
                ISNULL(CAST(UnitNo AS NVARCHAR(100)), '') AS UnitNo,
                ISNULL(CAST(Address1 AS NVARCHAR(255)), '') AS Address1,
                ISNULL(CAST(Address2 AS NVARCHAR(255)), '') AS Address2,
                ISNULL(CAST(City AS NVARCHAR(100)), '') AS City,
                ISNULL(CAST(State AS NVARCHAR(100)), '') AS State,
                ISNULL(CAST(Country AS NVARCHAR(100)), '') AS Country,
                ISNULL(CAST(PostalCode AS NVARCHAR(50)), '') AS PostalCode,
                ISNULL(CAST(Phone AS NVARCHAR(100)), '') AS Phone,
                ISNULL(CAST(Fax AS NVARCHAR(100)), '') AS Fax,
                ISNULL(CAST(IPAddress AS NVARCHAR(100)), '') AS IpAddress,
                ISNULL(CAST(PortNo AS NVARCHAR(100)), '') AS PortNo,
                ISNULL(CAST(DBName AS NVARCHAR(255)), '') AS DbName,
                ISNULL(CAST(DBLogin AS NVARCHAR(255)), '') AS DbLogin,
                ISNULL(CAST(DBPassword AS NVARCHAR(255)), '') AS DbPassword,
                REPLACE(CONVERT(VARCHAR(11), ISNULL(LastUpdated, GETDATE()), 106), ' ', '-') AS LastUpdatedText
            FROM BIZ_Stores
            WHERE ID = @Id;";

            using var conn = _crmDbConnectionFactory.Create();

            return await conn.QuerySingleOrDefaultAsync<StoreDetailDto>(
                sql,
                new
                {
                    Id = id
                });
        }

        public async Task<List<StoreEmployeeOptionDto>> GetEmployeeOptionsAsync()
        {
            const string sql = @"
            SELECT
                0 AS Value,
                '- None -' AS Text

            UNION ALL

            SELECT
                ID AS Value,
                ISNULL(NULLIF(DisplayName, ''), ISNULL(Username, '')) AS Text
            FROM BIZ_Employee
            WHERE ISNULL(IsDeleted, 0) = 0
              AND ISNULL(Inactive, 0) = 0
            ORDER BY Text;";

            using var conn = _crmDbConnectionFactory.Create();

            var result = await conn.QueryAsync<StoreEmployeeOptionDto>(sql);

            return result.ToList();
        }

        public async Task<int> CreateStoreAsync(StoreDetailRequest request, int hqId)
        {
            const string sql = @"
            INSERT INTO BIZ_Stores
            (
                HQID,
                Code,
                StoreName,
                BIZRegNo,
                IsPublished,
                InChargeID,
                InChargeName,
                BlockHouseNo,
                UnitNo,
                Address1,
                Address2,
                City,
                State,
                Country,
                PostalCode,
                Phone,
                Fax,
                IPAddress,
                PortNo,
                DBName,
                DBLogin,
                DBPassword,
                LastUpdated
            )
            VALUES
            (
                @HqId,
                @Code,
                @StoreName,
                @BizRegNo,
                @IsPublished,
                @InChargeId,
                @InChargeName,
                @BlockHouseNo,
                @UnitNo,
                @Address1,
                @Address2,
                @City,
                @State,
                @Country,
                @PostalCode,
                @Phone,
                @Fax,
                @IpAddress,
                @PortNo,
                @DbName,
                @DbLogin,
                @DbPassword,
                GETDATE()
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = _crmDbConnectionFactory.Create();

            var newStoreId = await conn.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    HqId = hqId,
                    Code = request.Code?.Trim(),
                    StoreName = request.StoreName?.Trim(),
                    BizRegNo = request.BizRegNo?.Trim(),
                    IsPublished = request.IsPublished,
                    InChargeId = request.InChargeId,
                    InChargeName = request.InChargeName?.Trim(),
                    BlockHouseNo = request.BlockHouseNo?.Trim(),
                    UnitNo = request.UnitNo?.Trim(),
                    Address1 = request.Address1?.Trim(),
                    Address2 = request.Address2?.Trim(),
                    City = request.City?.Trim(),
                    State = request.State?.Trim(),
                    Country = request.Country?.Trim(),
                    PostalCode = request.PostalCode?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Fax = request.Fax?.Trim(),
                    IpAddress = request.IpAddress?.Trim(),
                    PortNo = request.PortNo?.Trim(),
                    DbName = request.DbName?.Trim(),
                    DbLogin = request.DbLogin?.Trim(),
                    DbPassword = request.DbPassword?.Trim()
                });

            return newStoreId;
        }

        public async Task<bool> UpdateStoreAsync(StoreDetailRequest request)
        {
            const string sql = @"
            UPDATE BIZ_Stores
            SET
                Code = @Code,
                StoreName = @StoreName,
                BIZRegNo = @BizRegNo,
                IsPublished = @IsPublished,
                InChargeID = @InChargeId,
                InChargeName = @InChargeName,
                BlockHouseNo = @BlockHouseNo,
                UnitNo = @UnitNo,
                Address1 = @Address1,
                Address2 = @Address2,
                City = @City,
                State = @State,
                Country = @Country,
                PostalCode = @PostalCode,
                Phone = @Phone,
                Fax = @Fax,
                IPAddress = @IpAddress,
                PortNo = @PortNo,
                DBName = @DbName,
                DBLogin = @DbLogin,
                DBPassword = CASE 
                                WHEN NULLIF(@DbPassword, '') IS NULL THEN DBPassword 
                                ELSE @DbPassword 
                            END,
                LastUpdated = GETDATE()
            WHERE ID = @Id;";

            using var conn = _crmDbConnectionFactory.Create();

            var affectedRows = await conn.ExecuteAsync(
                sql,
                new
                {
                    Id = request.Id,
                    Code = request.Code?.Trim(),
                    StoreName = request.StoreName?.Trim(),
                    BizRegNo = request.BizRegNo?.Trim(),
                    IsPublished = request.IsPublished,
                    InChargeId = request.InChargeId,
                    InChargeName = request.InChargeName?.Trim(),
                    BlockHouseNo = request.BlockHouseNo?.Trim(),
                    UnitNo = request.UnitNo?.Trim(),
                    Address1 = request.Address1?.Trim(),
                    Address2 = request.Address2?.Trim(),
                    City = request.City?.Trim(),
                    State = request.State?.Trim(),
                    Country = request.Country?.Trim(),
                    PostalCode = request.PostalCode?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Fax = request.Fax?.Trim(),
                    IpAddress = request.IpAddress?.Trim(),
                    PortNo = request.PortNo?.Trim(),
                    DbName = request.DbName?.Trim(),
                    DbLogin = request.DbLogin?.Trim(),
                    DbPassword = request.DbPassword?.Trim()
                });

            return affectedRows > 0;
        }

        public async Task RegisterOpeningQuantityAsync(int storeId, int currentUserId)
        {
            const string sql = @"
            INSERT INTO InventoryTransferLog
            (
                StoreID,
                LocationID,
                Type,
                CashierID,
                DateTransferred,
                ItemID,
                DetailID,
                Quantity,
                Cost,
                HQID,
                OrderNumber,
                Origin,
                Destination,
                Price
            )
            SELECT
                @StoreId AS StoreID,
                @StoreId AS LocationID,
                'Opening Quantity' AS Type,
                @CurrentUserId AS CashierID,
                DateCreated AS DateTransferred,
                ID AS ItemID,
                0 AS DetailID,
                0 AS Quantity,
                Cost,
                1 AS HQID,
                '' AS OrderNumber,
                '' AS Origin,
                '' AS Destination,
                Price
            FROM Item
            WHERE ISNULL(Inactive, 0) = 0
              AND ISNULL(ItemType, 0) = 0;";

            using var conn = _crmDbConnectionFactory.Create();

            await conn.ExecuteAsync(
                sql,
                new
                {
                    StoreId = storeId,
                    CurrentUserId = currentUserId
                });
        }

        private static string BuildStoreOrderByClause(string? sortColumn, string? sortDirection)
        {
            var direction = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            return sortColumn switch
            {
                "Id" => $"ID {direction}, ISNULL(CAST(StoreName AS NVARCHAR(255)), '') ASC",

                "Code" => $"ISNULL(CAST(Code AS NVARCHAR(100)), '') {direction}, ID ASC",

                "StoreName" => $"ISNULL(CAST(StoreName AS NVARCHAR(255)), '') {direction}, ISNULL(CAST(Code AS NVARCHAR(100)), '') ASC, ID ASC",

                "Phone" => $"ISNULL(CAST(Phone AS NVARCHAR(100)), '') {direction}, ID ASC",

                "InChargeName" => $"ISNULL(CAST(InChargeName AS NVARCHAR(255)), '') {direction}, ID ASC",

                "Inactive" => $"ISNULL(Inactive, 0) {direction}, ISNULL(CAST(StoreName AS NVARCHAR(255)), '') ASC, ID ASC",

                _ => "ISNULL(CAST(StoreName AS NVARCHAR(255)), '') ASC, ISNULL(CAST(Code AS NVARCHAR(100)), '') ASC, ID ASC"
            };
        }


    }
}