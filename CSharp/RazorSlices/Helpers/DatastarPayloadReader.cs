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

    public static async Task<IReadOnlyList<int>> ReadSelectedIndicesAsync(HttpRequest request, int maxCount)
    {
        if (request.ContentLength is null or 0)
        {
            return [];
        }

        using var document = await JsonDocument.ParseAsync(request.Body);

        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("selections", out var selectionsElement) ||
            selectionsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var selectedIndices = new List<int>();
        var index = 0;

        foreach (var selection in selectionsElement.EnumerateArray())
        {
            if (index >= maxCount)
            {
                break;
            }

            if (selection.ValueKind == JsonValueKind.True)
            {
                selectedIndices.Add(index);
            }

            index++;
        }

        return selectedIndices;
    }
}
