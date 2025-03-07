using System.Text.Json.Serialization;

namespace Backend.Models;

public record class ChatRequest(
    [property: JsonPropertyName("messages")] ChatMessage[] History
    )
{
    public string? LastUserQuestion => History?.Last(m => m.Role == "user")?.Content;
}
