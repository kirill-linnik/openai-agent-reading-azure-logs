using Backend.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Backend.Services;

public class ChatCompletionService(
    Kernel kernel,
    ChatService chatService,

    ILogger<ChatCompletionService> logger,
    string resourceId)
{
    public async Task ProcessRequestAsync(ChatRequest chatRequest)
    {
        var question = chatRequest.Message ?? throw new InvalidOperationException("No user question found");
        var chatThreadId = chatRequest.ThreadId ?? throw new InvalidOperationException("No chat thread id found");
        await chatService.SendUserMessageAsync(chatThreadId, question);

        var chatHistory = await chatService.GetAllMessages(chatThreadId);

        chatHistory.AddUserMessage(question);

        var agent = new ChatCompletionAgent
        {
            Kernel = kernel,
            Instructions = @$"
You are helpful AI assitant that runs different workflows on behalf of the user. 
Your goal is to help user get the data from stored in Log Analyzer workspace tables. 
Ask clarifying questions if the intent is unclear and make decisions which parameters to use for workflow execution.
Always explain all steps you are going to take to the user. Include all relevant information in the explanation: queries, parameters, etc.

For reference, current time is {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ} UTC.
User resource id is {resourceId.ToLowerInvariant()}.",
            Arguments = new KernelArguments(
            new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            }),
        };

        // Create or retrieve an AgentThread instance
        var response = agent.InvokeAsync(chatHistory);

        await foreach (var content in response.ConfigureAwait(false))
        {
            logger.LogInformation($"Assistant response: {content.Content}");
            await chatService.SendAssistantMessageAsync(chatThreadId, content.Content!);
        }
    }
}
