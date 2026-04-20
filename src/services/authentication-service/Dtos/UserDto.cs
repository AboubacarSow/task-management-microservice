using System.Text.Json.Serialization;

namespace authentication_service.Dtos;

public record UserDto(
    [property: JsonPropertyName("id")]         string Id,
    [property: JsonPropertyName("email")]      string Email,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name")]  string LastName);
