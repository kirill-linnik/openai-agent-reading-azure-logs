using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Backend.Controllers;

[ApiController]
[Route("chat")]
public class ChatController(ChatCompletionService chatCompletionService, ChatService chatService) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> StartConversationAsync()
    {
        var chatThreadId = await chatService.CreateChatAsync();
        return Ok(chatThreadId);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessMessageAsync([FromBody] ChatRequest chatRequest)
    {
        await chatCompletionService.ProcessRequestAsync(chatRequest);
        return Ok();
    }

    [HttpGet("{threadId}")]
    public async Task<IActionResult> GetMessagesAsync([FromRoute] string threadId, [FromQuery] string? lastMessageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId, nameof(threadId));
        var chatHistory = await chatService.GetFreshMessagesAsync(threadId, lastMessageId);

        var messages = new List<ChatMessage>();
        foreach (var message in chatHistory)
        {
            if (message.Metadata is not null &&
                message.Metadata.TryGetValue("id", out var idObj) &&
                idObj is string id &&
                message.Metadata.TryGetValue("createdOn", out var createdOnObj) &&
                createdOnObj is DateTimeOffset createdOn &&
                !string.IsNullOrWhiteSpace(message.Content) &&
                (message.Role == AuthorRole.Assistant || message.Role == AuthorRole.User))
            {
                messages.Add(new ChatMessage(
                    Id: id,
                    Role: message.Role.Label.ToUpperInvariant(),
                    CreatedOn: createdOn,
                    Content: message.Content
                ));
            }
        }
        return Ok(messages);
    }
}
