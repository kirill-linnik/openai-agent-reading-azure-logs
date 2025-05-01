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

    public async Task<LogsQueryResult> GetLogsAsync(string query)
    {
        _logger.LogDebug($"Executing query: {query}");
        var response = await _client.QueryWorkspaceAsync(_workspaceId, query, new QueryTimeRange(TimeSpan.FromDays(1000)));
        return response.Value;
    }

    public string GetLogsSchemaAsync()
    {
        return @"
Table name: ACSEmailSendMailOperational
Description: This table contains the metrics data for email send operations. 
Columns:
- TimeGenerated - UTC time
- Location - Server location
- CorrelationId - Unique correlation id. Use to build relations with other tables
- Size - Email size in megabytes
- ToRecipientsCount - How many 'To' recipients email has
- CcRecipientsCount - How many 'Cc' recipients email has
- BccRecipientsCount - How many 'Bcc' recipients email has
- UniqueRecipientsCount - How many unique recipients email has
- AttachmentsCount - Count of files attached to emailType
- _ResourceId - Resource id of the email send operation

Table name: ACSEmailStatusUpdateOperational
Description: This table contains the metrics data for email status update operations. It has several operations per every email, therefore, you need to understand what operations user is interested in
Columns:
- TimeGenerated - UTC time
- Location - Server location
- CorrelationId - Unique correlation id. Use to build relations with other tables
- DeliveryStatus - Status of delivery. Possible values: 
-- 'Delivered' - this is the final status meaning the email was successfully delivered to the recipient
-- 'Failed' - this is the final status meaning the email was not deliveried due to an error
-- 'Bounced' - this is the final status meaning the email could not be delivered due to a permanent issue (e.g., invalid email address)
-- 'Suppressed' - this is the final status meaning yhe email was not sent due to suppression rules (e.g., recipient opted out).
-- 'OutForDelivery' - this is the interim status meaning the email is in the process of being delivered.
-- 'Queued' - this is the interim status meaning the email is queued for delivery.
- SenderDomain - Domain of the sender email address
- SenderUsername - Username of the sender email address
- IsHardBounce - Indicates if the email is a hard bounce
- _ResourceId - Resource id of the email status update operation

Table name: ACSBillingUsage
Description: This table contains the metrics data for billing usage.
Columns:
- TimeGenerated - UTC time
- CorrelationId - Unique correlation id. Use to build relations with other tables
- UsageType - Type of usage. Possible values: 'emailsize' - indiciates payment per emailsize, 'emailcount' - indicates payment per email count
- Quantity - Quantity of usage
- _ResourceId - Resource id of the billing usage";
    }
}
