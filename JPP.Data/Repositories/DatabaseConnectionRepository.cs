using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Shared.Responses;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace JPP.Data.Repositories
{
    public class DatabaseConnectionRepository : IDatabaseConnectionRepository
    {
        private readonly IAiDbConnectionFactory _aiDb;
        private readonly ICrmDbConnectionFactory _crmDb;
        private readonly ILogger<DatabaseConnectionRepository> _logger;

        public DatabaseConnectionRepository(
            IAiDbConnectionFactory aiDb,
            ICrmDbConnectionFactory crmDb,
            ILogger<DatabaseConnectionRepository> logger)
        {
            _aiDb = aiDb ?? throw new ArgumentNullException(nameof(aiDb));
            _crmDb = crmDb ?? throw new ArgumentNullException(nameof(crmDb));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResult> CheckAiConnectionAsync()
        {
            return await CheckConnectionAsync(
                connectionName: "AiConnection",
                createConnection: () => _aiDb.Create());
        }

        public async Task<BaseResult> CheckCrmConnectionAsync()
        {
            return await CheckConnectionAsync(
                connectionName: "CrmConnection",
                createConnection: () => _crmDb.Create());
        }

        public async Task<BaseResult> CheckAllConnectionsAsync()
        {
            var aiResult = await CheckAiConnectionAsync();

            if (aiResult.StatusCode != 200)
            {
                return aiResult;
            }

            var crmResult = await CheckCrmConnectionAsync();

            if (crmResult.StatusCode != 200)
            {
                return crmResult;
            }

            return BaseResult.Ok(
                statusCode: 200,
                statusMessage: "AiConnection and CrmConnection connected successfully.");
        }

        private async Task<BaseResult> CheckConnectionAsync(
            string connectionName,
            Func<IDbConnection> createConnection)
        {
            try
            {
                using var conn = createConnection();

                if (conn == null)
                {
                    return BaseResult.Fail(
                        statusCode: 500,
                        statusMessage: $"{connectionName} factory returned null connection.");
                }

                if (string.IsNullOrWhiteSpace(conn.ConnectionString))
                {
                    return BaseResult.Fail(
                        statusCode: 500,
                        statusMessage: $"{connectionName} connection string is empty.");
                }

                if (conn.State != ConnectionState.Open)
                {
                    if (conn is DbConnection dbConnection)
                    {
                        await dbConnection.OpenAsync();
                    }
                    else
                    {
                        conn.Open();
                    }
                }

                var result = await conn.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        commandText: "SELECT 1",
                        commandTimeout: 30));

                if (result != 1)
                {
                    return BaseResult.Fail(
                        statusCode: 500,
                        statusMessage: $"{connectionName} test query failed.");
                }

                _logger.LogInformation("{ConnectionName} connected successfully.", connectionName);

                return BaseResult.Ok(
                    statusCode: 200,
                    statusMessage: $"{connectionName} connected successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ConnectionName} connection failed.", connectionName);

                return BaseResult.Fail(
                    statusCode: 500,
                    statusMessage: $"{connectionName} connection failed. {ex.Message}");
            }
        }
    }
}