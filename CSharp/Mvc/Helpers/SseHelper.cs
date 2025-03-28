using System.Text;

namespace Mvc.Helpers;

/// <summary>
/// Provides static helper methods for handling Server-Sent Events (SSE) in ASP.NET Core controllers.
/// SSE allows a server to push data to a client asynchronously over a single, long-lived HTTP connection.
/// </summary>
public static class SseHelper
{
    /// <summary>
    /// Sets the necessary HTTP headers on the response to establish an SSE connection.
    /// These headers inform the client that the response is an event stream.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> object to modify.</param>
    public static async Task SetSseHeadersAsync(HttpResponse response)
    {
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("Content-Type", "text/event-stream");
        response.Headers.Append("Connection", "keep-alive");
        await response.Body.FlushAsync(); // Ensures headers are sent immediately.
    }

    /// <summary>
    /// Sends a formatted Server-Sent Event to the client.
    /// Can optionally include HTML fragment data for libraries like Datastar.js to merge into the DOM.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> object representing the SSE connection.</param>
    /// <param name="fragment">Optional HTML fragment to send. Will be cleaned of newlines.</param>
    /// <param name="selector">Optional CSS selector for targeting where the fragment should be merged (used by client-side JS).</param>
    /// <param name="mergeMode">Optional merge mode (e.g., 'append', 'prepend', 'replace') for the fragment (used by client-side JS).</param>
    /// <param name="settleDuration">Optional duration (in ms) for client-side settling/animations after merge.</param>
    /// <param name="useViewTransition">Optional flag to indicate if client-side should use View Transitions API.</param>
    /// <param name="end">If true, sends a final completion event and gracefully closes the SSE connection.</param>
    /// <param name="eventType">The type of the SSE event (defaults to 'datastar-merge-fragments').</param>
    public static async Task SendServerSentEventAsync(HttpResponse response
        , string fragment = ""
        , string selector = ""
        , string mergeMode = ""
        , int settleDuration = 300
        , bool useViewTransition = false
        , bool end = false
        , string eventType = "datastar-merge-fragments"
    )
    {
        var data = $"event: {eventType}\n";

        if (!string.IsNullOrEmpty(selector))
        {
            data += $"data: selector {selector}\n";
        }

        if (!string.IsNullOrEmpty(fragment))
        {
            // Clean the fragment by removing all newlines and extra spaces
            fragment = fragment
                .Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Trim();

            // Ensure fragment is a single line for SSE data field
            data += $"data: fragments {fragment}\n";
        }

        if (!string.IsNullOrEmpty(mergeMode))
        {
            data += $"data: mergeMode {mergeMode}\n";
        }

        if (settleDuration != 300)
        {
            data += $"data: settleDuration {settleDuration}\n";
        }

        if (useViewTransition)
        {
            data += $"data: useViewTransition {useViewTransition}\n";
        }

        // SSE events are terminated by a double newline.
        data += "\n";

        // Write the formatted event data to the response stream.
        await response.Body.WriteAsync(Encoding.UTF8.GetBytes(data));
        // Ensure the event is sent to the client immediately.
        await response.Body.FlushAsync();

        // Check if this is the final event in the stream.
        if (end)
        {
            // Optionally, send a specific event type to signal the end of the stream to the client.
            var endData = "event: datastar-merge-complete\n\n"; // Send event before completing
            await response.Body.WriteAsync(Encoding.UTF8.GetBytes(endData));
            await response.Body.FlushAsync(); // Ensure the final event is sent

            // Gracefully close the SSE connection.
            // This signals to the client that no more events will be sent.
            // Using CompleteAsync() is crucial for proper closure, especially over HTTP/2,
            // preventing potential protocol errors on the client side.
            await response.CompleteAsync();
        }
    }
}
