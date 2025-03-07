using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{

    private readonly ChatCompletionService _chatCompletionService;

    public ChatController(ChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessMessageAsync([FromBody] ChatRequest chatRequest)
    {
        if (chatRequest is { History.Length: > 0 })
        {
            var response = await _chatCompletionService.ProcessRequestAsync(chatRequest);
            return Ok(response);
        }
        return BadRequest("No messages provided");
    }
}
