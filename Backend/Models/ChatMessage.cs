using System.Text.Json.Serialization;

namespace Backend.Models;

public record ChatMessage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("createdOn")] DateTimeOffset CreatedOn,
    [property: JsonPropertyName("content")] string Content)
{
}

