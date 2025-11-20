using System.Text;

namespace MinimalAPI.Helpers;

public static class SseHelper
{
    public static async Task SetSseHeadersAsync(HttpResponse response)
    {
        // Only set headers if the response hasn't started yet
        if (!response.HasStarted)
        {
            response.Headers.Append("Cache-Control", "no-cache");
            response.Headers.Append("Content-Type", "text/event-stream");
            response.Headers.Append("Connection", "keep-alive");
        }
        await response.Body.FlushAsync();
    }

    /// <summary>
    /// Send a Datastar "datastar-patch-elements" SSE event.
    /// </summary>
    /// <param name="response">HTTP response to write the SSE to.</param>
    /// <param name="elements">One or more complete HTML elements (must have an id if no selector is provided).</param>
    /// <param name="selector">Optional CSS selector for the target element.</param>
    /// <param name="mergeMode">Patch mode: outer|inner|replace|prepend|append|before|after|remove. Defaults to outer.</param>
    /// <param name="settleDuration">Deprecated. No longer used in RC.6; kept for call-site compatibility.</param>
    /// <param name="useViewTransition">Whether to wrap the patch in a View Transition.</param>
    /// <param name="end">Whether to close the response stream after sending.</param>
    public static async Task SendServerSentEventAsync(
        HttpResponse response
        , string elements
        , string selector = null
        , string mergeMode = null
        , int settleDuration = 300
        , bool useViewTransition = false
        , bool end = false)
    {
        // Normalize elements by removing all newlines and extra spaces
        elements = elements
            .Replace(Environment.NewLine, "")
            .Replace("\n", "")
            .Replace("\r", "")
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

    /// <summary>
    /// Send a Datastar "datastar-patch-signals" SSE event.
    /// </summary>
    /// <param name="response">HTTP response to write the SSE to.</param>
    /// <param name="signalsJson">RFC 7386 JSON Merge Patch payload as a JSON string (e.g., {"foo":1}).</param>
    /// <param name="onlyIfMissing">If true, only sets signals that do not already exist.</param>
    /// <param name="end">Whether to close the response stream after sending.</param>
    public static async Task PatchSignalsAsync(
        HttpResponse response,
        string signalsJson,
        bool onlyIfMissing = false,
        bool end = false)
    {
        // Normalize signals JSON to a single line
        var normalized = (signalsJson ?? string.Empty)
            .Replace(Environment.NewLine, "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim();

        var sb = new StringBuilder();
        sb.AppendLine("event: datastar-patch-signals");
        if (onlyIfMissing)
        {
            sb.AppendLine("data: onlyIfMissing true");
        }
        sb.AppendLine($"data: signals {normalized}");
        sb.AppendLine();

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        await response.Body.WriteAsync(bytes);
        await response.Body.FlushAsync();

        if (end)
        {
            response.Body.Close();
        }
    }

    /// <summary>
    /// Convenience helper to remove an element using patch-elements with mode remove.
    /// </summary>
    public static async Task RemoveElementsAsync(HttpResponse response, string selector, bool end = false)
    {
        var sb = new StringBuilder();
        sb.AppendLine("event: datastar-patch-elements");
        sb.AppendLine("data: mode remove");
        if (!string.IsNullOrWhiteSpace(selector))
        {
            sb.AppendLine($"data: selector {selector}");
        }
        sb.AppendLine();

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        await response.Body.WriteAsync(bytes);
        await response.Body.FlushAsync();

        if (end)
        {
            response.Body.Close();
        }
    }
}