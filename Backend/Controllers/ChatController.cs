using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

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
        var messages = await chatService.GetFreshMessagesAsync(threadId, lastMessageId);
        return Ok(messages);
    }
}
