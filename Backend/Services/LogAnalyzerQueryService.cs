using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;

namespace Backend.Services;

public class LogAnalyzerQueryService
{
    private readonly LogsQueryClient _client;
    private readonly ILogger<LogAnalyzerQueryService> _logger;
    private readonly string _workspaceId;

    public LogAnalyzerQueryService(AzureLogApiTokenCredentialService azureMetricTokenCredentialService, ILogger<LogAnalyzerQueryService> logger, string workspaceId)
    {
        _client = new LogsQueryClient(azureMetricTokenCredentialService);
        _logger = logger;
        _workspaceId = workspaceId;
    }

    public async Task<LogsQueryResult?> GetLogsAsync(string query)
    {
        try
        {
            var response = await _client.QueryWorkspaceAsync(_workspaceId, query, new QueryTimeRange(TimeSpan.FromDays(1000)));
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying logs");
            return null;
        }
    }
}
