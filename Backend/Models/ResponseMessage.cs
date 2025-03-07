using System.Text.Json.Serialization;

namespace Backend.Models;

public record ResponseMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content)
{
}
