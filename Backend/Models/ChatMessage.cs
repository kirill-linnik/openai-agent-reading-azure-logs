using System.Text.Json.Serialization;

namespace Backend.Models;

public record ChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content)
{
    public bool IsUser => Role == "user";
}

