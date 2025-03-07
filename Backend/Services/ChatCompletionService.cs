using Backend.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Services;

public class ChatCompletionService(
    Kernel kernel,
    LogAnalyzerQueryService logAnalyzerQueryService,
    ConversationContextService conversationContextService,
    ILogger<ChatCompletionService> logger,
    string resourceId)
{
    public async Task<ResponseMessage> ProcessRequestAsync(ChatRequest chatRequest)
    {
        var question = chatRequest.LastUserQuestion ?? throw new InvalidOperationException("No user question found");

        // step 1: update conversation context with the last user message
        var updatedConversationContext = await GetNewConversationContext(question);
        conversationContextService.SetConversationContext(updatedConversationContext);

        // step 2: get query
        var query = await GetQuery(question, updatedConversationContext);

        // step 3: get data
        var data = string.Empty;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var response = await logAnalyzerQueryService.GetLogsAsync(query);
            if (response != null)
            {
                data = JsonSerializer.Serialize(response.Table, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            }
            logger.LogInformation($"Data: {data}");
        }


        // step 4: get reply
        var answer = await GetResponse(chatRequest.History, updatedConversationContext, data);

        return new ResponseMessage("assistant", answer);
    }

    private async Task<string> GetNewConversationContext(string lastUserMessage)
    {
        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var conversationContextChat = new ChatHistory(@$"
You are an assistant tasked with analyzing the conversation context and accurately updating it based on the user's queries. Follow the detailed instructions below to accurately update the conversation context.

## Instructions ##
The user is querying metrics data. Identify the specific data points the user is requesting regarding the metrics and add these to the conversation context. 
Update the conversation with the new requests user has.
If the user asks about a different time range, update the conversation context with the new time range. 
If the user does not specify a new time range, retain the last time range specified in current conversation context.
Ensure time ranges mentioned by the user are accurately updated and retained in the conversation context.

If the user doesn't ask additional questions, ensure the updated conversation context remains the same as the current conversation context.

You should reply only with the content of updated conversation context.
## End of Instructions ##

## Current conversation context ##
{conversationContextService.GetConversationContext()}
## End of current conversation context ##

## Examples ##

Current conversation context is empty
User message: How many emails were sent last week?
Your response: Users asks how many emails were sent last week.

Current conversation context: Users asks how many emails were sent last week.
User message: How many emails were not delivered?
Your response: User asks how many emails were not delivered last week.

Current conversation context: Users asks how many emails were not delivered last week.
User message: And what about last month?
Your response: User asks how many emails were not delivered last month.

## End of examples ##
");
        conversationContextChat.AddUserMessage(lastUserMessage);
        var conversationContextResponse = await chat.GetChatMessageContentAsync(conversationContextChat);
        var updatedConversationContext = conversationContextResponse.Content ?? throw new InvalidOperationException("Failed to get conversation context response");
        logger.LogInformation($"Updated conversation context: {updatedConversationContext}");

        return updatedConversationContext;
    }

    private async Task<string> GetQuery(string lastUserMessage, string updatedConversationContext)
    {
        var fixedResourceId = resourceId.ToLowerInvariant();
        var now = DateTimeOffset.UtcNow;

        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var instructionsChat = new ChatHistory(@$"
You are helpful assitant providing query using Kusto Query Language (KQL) to get the metrics data. Use the provided instructions to get the metrics data and provide the user with the requested information.

## Instructions ##
You may use the following tables to get email-related metrics data:

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
- _ResourceId - Resource id of the billing usage

User specified their resource id as: '{fixedResourceId}'. Always use this resource id to get the metrics data.

In case you need to use filtering by time, use the current time as a base: {now:yyyy-MM-ddTHH:mm:ssZ}

Always consider conversation context and build your query based on it.

If user doesn't provide time range in their last message, analyze the conversation and make clear what time range they are talking about. Insert it into your query. 
For example, if the user asks about emails sent last month, all your consequent queries should include a TimeGenerated filter until the user asks about another time period. 
Always respect and incorporate the time range specified by the conversation context in your queries. You cannot ignore it.

Never apply formatting to your response; respond only with the query in plain text format. Always check that columns belong to tables you use.

You cannot return more than just one query. If you want to query multiple tables, you must join it in one query.
If you are unable to provide a query, respond with an empty message. 
## End of Instructions ##

## Conversation context ##
{updatedConversationContext}
## End of conversation context ##

## Examples ##
Conversation context: User asks for a summary of all emails they sent.
User message: Provide me a summary of all emails I sent

Your response: ACSEmailSendMailOperational
| where _ResourceId == '{fixedResourceId}'
| summarize
    TotalMessageCount = dcount(CorrelationId),
    TotalSize = sum(Size),
    AverageSizePerMessage = avg(Size),
    AverageRecipientsPerMessage = avg(UniqueRecipientsCount),
    AverageAttachmentsPerMessage = avg(AttachmentsCount),
    AverageSize = avg(Size)

Conversation context: User asks how many emails were delivered this month.
User message: How many emails were delivered?

Your response: ACSEmailStatusUpdateOperational
| where DeliveryStatus == 'Delivered'
| where TimeGenerated >= startofmonth(datetime({now:yyyy-MM-ddTHH:mm:ssZ}))
| where _ResourceId == '{fixedResourceId}'
| summarize
    TotalDelivered = count()

Conversation context: User asks how many emails were delivered this month.
User message: How many emails were not delivered?

Your response: ACSEmailStatusUpdateOperational
| where DeliveryStatus == 'Failed' or DeliveryStatus == 'Bounced' or DeliveryStatus == 'Suppressed'
| where TimeGenerated >= startofmonth(datetime({now:yyyy-MM-ddTHH:mm:ssZ}))
| where _ResourceId == '{fixedResourceId}'
| summarize
    TotalDeliveryFailures = count()

Conversation context: User asks how many emails were sent last hour.
User message: How many emails were sent last hour?

Your response: ACSEmailSendMailOperational
| where TimeGenerated > datetime({now.AddHours(-1):yyyy-MM-ddTHH:mm:ssZ})
| where _ResourceId == '{fixedResourceId}'
| summarize
    TotalMessagesSent = count()

Conversation context: User asks what domains were used this month.
User message: What domains were used?

Your response: ACSEmailStatusUpdateOperational
| where _ResourceId == '{fixedResourceId}'
| where DeliveryStatus == 'Failed' or DeliveryStatus == 'Bounced' or DeliveryStatus == 'Suppressed' or DeliveryStatus == 'Delivered'
| where TimeGenerated >= startofmonth(datetime({now:yyyy-MM-ddTHH:mm:ssZ}))
| summarize TotalCount = count() by SenderDomain
| order by TotalCount desc

Conversation context: User asks how is going.
User message: Hey, how is going?
Your response is empty
## End of examples ##
");
        instructionsChat.AddSystemMessage(updatedConversationContext);
        var instructionResponse = await chat.GetChatMessageContentAsync(instructionsChat);
        var query = instructionResponse.Content ?? throw new InvalidOperationException("Failed to get query response");
        logger.LogInformation($"Query: {query}");

        return query;
    }

    private async Task<string> GetResponse(ChatMessage[] history, string updatedConversationContext, string data)
    {
        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var metricsChat = new ChatHistory(@$"
You are an assistant providing response to user message based on the metrics, instructions and conversation context. 
Make sense of this metric data and provide the user with the requested information.

## Metrics data ##
{data}
## End of Metrics data ##

## Conversation context ##
{updatedConversationContext}
## End of conversation context ##

## Instructions ##
If metrics data is empty then it means no data was found. For example, is user asks what emails were bounced, and there is no single of them, data will be empty.
Be as precise and short as possible. Provide the user with the requested information. Use markdown language in your response where needed
## End of Instructions ##
");
        for (var i = 0; i < history.Length; i++)
        {
            var message = history[i];
            if (message.Role == "user")
            {
                metricsChat.AddUserMessage(message.Content);
            }
            else
            {
                metricsChat.AddAssistantMessage(message.Content);
            }
        }
        var metricsResponse = await chat.GetChatMessageContentAsync(metricsChat);
        var answer = metricsResponse.Content ?? throw new InvalidOperationException("Failed to get metrics response");

        logger.LogInformation($"response: {answer}");

        return answer;
    }
}
