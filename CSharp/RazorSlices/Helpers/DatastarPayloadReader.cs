using System.Text.Json;

namespace DatastarExamples.RazorSlicesApp.Helpers;

public static class DatastarPayloadReader
{
    public static async Task<string> ReadInputAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0)
        {
            return string.Empty;
        }

        using var document = await JsonDocument.ParseAsync(request.Body);

        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("input", out var inputElement) ||
            inputElement.ValueKind != JsonValueKind.String)
        {
            return string.Empty;
        }

        return inputElement.GetString()?.Trim() ?? string.Empty;
    }
}
