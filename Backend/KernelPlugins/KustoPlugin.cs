using Backend.Services;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace Backend.KernelPlugins;

[McpServerToolType]
public class KustoPlugin(LogAnalyzerQueryService queryService, ILogger<KustoPlugin> logger)
{

    [KernelFunction("execute_logs_query")]
    [Description(@"Executes a Kusto Query Language (KQL) query to fetch Log Analyzer logs.
Consider the limitiations and syntax of KQL when writing the query.for fetching data from Log Analyzer tables.
The logs for different operations stored in different tables. 
Always limit the query to at most 100 rows. 
Always fetch Log Analyzer schema for tables for grounding, before executing a query.
This function returns JSON containing the results. 
")]
    [McpServerTool]
    public async Task<string> ExecuteLogsQueryAsync(
        [Description("The KQL query to execute.")]
        string query
        )
    {
        logger.LogInformation($"Executing query: {query}");
        var results = await queryService.GetLogsAsync(query);

        return JsonSerializer.Serialize(results);
    }

    [KernelFunction("get_log_analyzer_schema")]
    [Description("Get the Log Analyzer schema for tables.")]
    [McpServerTool]
    public string GetTableSchemaAsync()
    {
        logger.LogInformation("Getting Log Analyzer schema");
        var schema = queryService.GetLogsSchemaAsync();

        return schema;
    }
}
