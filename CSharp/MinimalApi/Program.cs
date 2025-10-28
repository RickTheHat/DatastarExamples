// Global variables

using Bogus;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using MinimalAPI.Models;
using MinimalAPI.Helpers;

var random = new Random();
List<Note> notes = null;
var totalNoteCount = 0;

// Click to Edit state
var currentContact = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };
var originalContact = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

var builder = WebApplication.CreateBuilder(args);

// Add services for browser refresh in development
if (builder.Environment.IsDevelopment()) builder.Services.AddWebEncoders();

var app = builder.Build();

// Configure browser refresh middleware in development
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

// Add after app builder but before other middleware
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";

            var errorHtml = "<div id=\"notes-list\">" +
                            "<span class=\"note-item\">" +
                            "<p>An error occurred. Please try again later.</p>" +
                            "</span></div>";

            await SseHelper.SendServerSentEventAsync(context.Response, errorHtml);
        });
    });

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy",
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    if (!app.Environment.IsDevelopment())
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    await next();
});

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot"))
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot"))
});

app.MapGet("/api/notes", async context =>
{
    // Generate fake notes if they don't exist
    if (notes == null)
    {
        var count = random.Next(20, 983);
        var todoFaker = new Faker<Note>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Content, f => f.Lorem.Sentence());

        notes = todoFaker.Generate(count).ToList();

        // Add exactly 3 notes with "hello"
        for (var i = 0; i < 3; i++)
        {
            var randomIndex = random.Next(notes.Count);
            notes[randomIndex] = new Note
            {
                Id = notes[randomIndex].Id,
                Content = $"Hello! {new Faker().Lorem.Paragraph()}"
            };
        }

        totalNoteCount = notes.Count;
    }

    await SseHelper.SetSseHeadersAsync(context.Response);

    // For initial load, just take first 5 notes
    var filteredNotes = notes.Take(5).ToList();

    var countsHtml =
        $"<p id=\"total-count\">Showing <span class=\"number\">{filteredNotes.Count}</span> of <span class=\"number\">{totalNoteCount}</span> notes</p>";
    await SseHelper.SendServerSentEventAsync(context.Response, countsHtml);

    var notesListHtml = "<div id=\"notes-list\" class=\"notes-list\">";
    foreach (var note in filteredNotes)
    {
        notesListHtml += "<div class=\"note-item\">";
        notesListHtml += $"<p>{note.Content}</p>";
        notesListHtml += "</div>";
    }
    notesListHtml += "</div>";
    await SseHelper.SendServerSentEventAsync(context.Response, notesListHtml);
});

app.MapPut("/api/search", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    // Clear any existing error message first
    // TODO finish up
    // await SseHelper.SendServerSentEvent(context.Response,
    //     "<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\"></span>");

    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var query = body.Replace("{\"input\":\"", "").Replace("\"}", "");

    // Check for minimum query length
    // TODO finish up
    // if (query.Length < 3 && query.Length > 0)
    // {
    //     var remainingChars = 3 - query.Length;
    //     var characterPlural = remainingChars == 1 ? "character" : "characters";
    //     var errorHtml = $"<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\">" +
    //                     $"Minimum search is 3 characters > please enter {remainingChars} more {characterPlural}</span>";
    //     await SseHelper.SendServerSentEvent(context.Response, errorHtml);
    //     return;
    // }

    var filteredNotes = string.IsNullOrEmpty(query) || query == "{}"
        ? notes.Take(5).ToList()
        : notes.Where(x => x.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

    var countsHtml = filteredNotes.Count == 0
        ? $"<p id=\"total-count\">No results found for \"{query}\" out of <span class=\"number\">{totalNoteCount}</span> notes</p>"
        : $"<p id=\"total-count\">Showing <span class=\"number\">{filteredNotes.Count}</span> of <span class=\"number\">{totalNoteCount}</span> notes</p>";
    await SseHelper.SendServerSentEventAsync(context.Response, countsHtml);

    var notesListHtml = "<div id=\"notes-list\" class=\"notes-list\">";
    if (filteredNotes.Count == 0)
    {
        notesListHtml += "<div class=\"note-item\">";
        notesListHtml += $"<p>No notes found matching \"{query}\"</p>";
        notesListHtml += "</div>";
    }
    else
    {
        foreach (var note in filteredNotes)
        {
            notesListHtml += "<div class=\"note-item\">";
            notesListHtml += $"<p>{note.Content}</p>";
            notesListHtml += "</div>";
        }
    }
    notesListHtml += "</div>";
    await SseHelper.SendServerSentEventAsync(context.Response, notesListHtml);
});

app.MapGet("/api/progress", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    var actionType = context.Request.Query["actionType"];

    var progressBarHtml = "<progress id=\"progressBar\" value=\"0\" max=\"100\" style=\"width: 100%;\"></progress>";
    var progressBarPercentageHtml =
        "<span id=\"progressBarPercentage\" style=\"position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); font-weight: bold;\">0%</span>";

    if (actionType == "start")
    {
        for (var i = 0; i <= 100; i++)
        {
            progressBarHtml +=
                $"<progress id=\"progressBar\" value=\"{i}\" max=\"100\" style=\"width: 100%;\"></progress>";
            progressBarPercentageHtml +=
                $"<span id=\"progressBarPercentage\" style=\"position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); font-weight: bold;\">{i}%</span>";

            await SseHelper.SendServerSentEventAsync(context.Response, progressBarHtml);
            await SseHelper.SendServerSentEventAsync(context.Response, progressBarPercentageHtml);
            await Task.Delay(100);
        }
    }
    else if (actionType == "repeat")
    {
        // TODO - Implement repeating progress bar
    }
    else if (actionType == "incremental")
    {
        // TODO - Implement incremental progress bar
    }
    else if (actionType == "reset")
    {
        // TODO - Implement reset progress bar
    }
});

// Click to Edit endpoints
app.MapGet("/api/clicktoedit/form", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    // Return the edit form with current contact data
    var editFormHtml = $@"
        <div id=""demo"" style=""display: flex; flex-direction: column; gap: 1rem;"">
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                First Name
                <input
                    type=""text""
                    data-bind:first-name
                    value=""{currentContact.FirstName}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                Last Name
                <input
                    type=""text""
                    data-bind:last-name
                    value=""{currentContact.LastName}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                Email
                <input
                    type=""email""
                    data-bind:email
                    value=""{currentContact.Email}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <div role=""group"" style=""display: flex; gap: 1rem;"">
                <button
                    class=""button success""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@put('/api/clicktoedit/save')""
                >
                    Save
                </button>
                <button
                    class=""button error""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@get('/api/clicktoedit/cancel')""
                >
                    Cancel
                </button>
            </div>
        </div>";

    await SseHelper.SendServerSentEventAsync(context.Response, editFormHtml, "#demo");
});

app.MapPut("/api/clicktoedit/save", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    try
    {
        // Parse the signals JSON to extract contact data
        var signalData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

        if (signalData != null)
        {
            if (signalData.TryGetValue("firstName", out var firstName) && firstName != null)
            {
                currentContact.FirstName = firstName.ToString() ?? currentContact.FirstName;
            }
            if (signalData.TryGetValue("lastName", out var lastName) && lastName != null)
            {
                currentContact.LastName = lastName.ToString() ?? currentContact.LastName;
            }
            if (signalData.TryGetValue("email", out var email) && email != null)
            {
                currentContact.Email = email.ToString() ?? currentContact.Email;
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error parsing signals: {ex.Message}");
    }

    // Return to display view
    var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{currentContact.Email}</span></p>
            <div role=""group"">
                <button
                    class=""button info""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@get('/api/clicktoedit/form')""
                >
                    Edit
                </button>
                <button
                    class=""button warning""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@patch('/api/clicktoedit/reset')""
                >
                    Reset
                </button>
            </div>
        </div>";

    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.MapGet("/api/clicktoedit/cancel", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    // Return to display view
    var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{currentContact.Email}</span></p>
            <div role=""group"">
                <button
                    class=""button info""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@get('/api/clicktoedit/form')""
                >
                    Edit
                </button>
                <button
                    class=""button warning""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@patch('/api/clicktoedit/reset')""
                >
                    Reset
                </button>
            </div>
        </div>";

    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.MapPatch("/api/clicktoedit/reset", async context =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    // Reset to original contact data
    currentContact = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

    // Return to display view
    var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{currentContact.Email}</span></p>
            <div role=""group"">
                <button
                    class=""button info""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@get('/api/clicktoedit/form')""
                >
                    Edit
                </button>
                <button
                    class=""button warning""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@patch('/api/clicktoedit/reset')""
                >
                    Reset
                </button>
            </div>
        </div>";

    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.Run();