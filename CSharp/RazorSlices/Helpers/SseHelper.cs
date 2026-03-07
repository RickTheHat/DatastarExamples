using System.Text;

namespace DatastarExamples.RazorSlicesApp.Helpers;

public static class SseHelper
{
    public static async Task SetSseHeadersAsync(HttpResponse response)
    {
        if (!response.HasStarted)
        {
            response.Headers.Append("Cache-Control", "no-cache");
            response.Headers.Append("Content-Type", "text/event-stream");
            response.Headers.Append("Connection", "keep-alive");
        }

        await response.Body.FlushAsync();
    }

    public static async Task SendServerSentEventAsync(
        HttpResponse response,
        string elements,
        string selector = null,
        string mergeMode = null,
        int settleDuration = 300,
        bool useViewTransition = false,
        bool end = false)
    {
        elements = elements
            .Replace(Environment.NewLine, string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Trim();

        var data = "event: datastar-patch-elements\n";

        if (!string.IsNullOrEmpty(selector))
        {
            data += $"data: selector {selector}\n";
        }

        if (!string.IsNullOrEmpty(mergeMode))
        {
            data += $"data: mode {mergeMode}\n";
        }

        if (useViewTransition)
        {
            data += $"data: useViewTransition {useViewTransition}\n";
        }

        data += $"data: elements {elements}\n\n";

        await response.Body.WriteAsync(Encoding.UTF8.GetBytes(data));
        await response.Body.FlushAsync();

        if (end)
        {
            response.Body.Close();
        }
    }

    public static async Task PatchSignalsAsync(
        HttpResponse response,
        string signalsJson,
        bool onlyIfMissing = false,
        bool end = false)
    {
        var normalized = (signalsJson ?? string.Empty)
            .Replace(Environment.NewLine, string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Trim();

        var data = new StringBuilder();
        data.AppendLine("event: datastar-patch-signals");

        if (onlyIfMissing)
        {
            data.AppendLine("data: onlyIfMissing true");
        }

        data.AppendLine($"data: signals {normalized}");
        data.AppendLine();

        await response.Body.WriteAsync(Encoding.UTF8.GetBytes(data.ToString()));
        await response.Body.FlushAsync();

        if (end)
        {
            response.Body.Close();
        }
    }
}
