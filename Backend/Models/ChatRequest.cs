using System.Text.Json.Serialization;

namespace Backend.Models;

public record class ChatRequest(
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("threadId")] string ThreadId
    )
{
}
