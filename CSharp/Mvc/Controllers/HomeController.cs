using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Mvc.Helpers;
using Bogus;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private static List<Note> _notes;
    private static int _totalNoteCount;
    private static Contact _currentContact = new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };
    private static Contact _originalContact = new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };
    private readonly Random _random = new();

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ActiveSearch()
    {
        var count = _random.Next(25, 102);
        var todoFaker = new Faker<Note>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Content, f => f.Lorem.Sentence());

        _notes = todoFaker.Generate(count).ToList();

        // Add exactly 3 notes with "hello"
        for (var i = 0; i < 3; i++)
        {
            var randomIndex = _random.Next(_notes.Count);
            _notes[randomIndex] = new Note
            {
                Id = _notes[randomIndex].Id,
                Content = $"hello! {new Faker().Lorem.Sentence()}"
            };
        }

        _totalNoteCount = _notes.Count;
        ViewData["TotalCount"] = _totalNoteCount;

        var displayNotes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    public IActionResult Animations()
    {
        return View();
    }

    public IActionResult BulkUpdate()
    {
        return View();
    }

    public IActionResult ClickToEdit()
    {
        return View();
    }

    public async Task ClickToEditForm()
    {
        await SseHelper.SetSseHeadersAsync(Response);
        
        // Return the edit form with current contact data
        var editFormHtml = $@"
<div id=""demo"">
    <label>
        First Name
        <input
            type=""text""
            data-bind:first-name
            value=""{_currentContact.FirstName}""
            data-attr:disabled=""$_fetching""
        >
    </label>
    <label>
        Last Name
        <input
            type=""text""
            data-bind:last-name
            value=""{_currentContact.LastName}""
            data-attr:disabled=""$_fetching""
        >
    </label>
    <label>
        Email
        <input
            type=""email""
            data-bind:email
            value=""{_currentContact.Email}""
            data-attr:disabled=""$_fetching""
        >
    </label>
    <div role=""group"">
        <button
            class=""button success""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@put('/Home/ClickToEditSave')""
        >
            Save
        </button>
        <button
            class=""button error""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@get('/Home/ClickToEditCancel')""
        >
            Cancel
        </button>
    </div>
</div>";
        
        await SseHelper.SendServerSentEventAsync(Response, editFormHtml, "#demo");
    }

    [HttpPut]
    public async Task ClickToEditSave()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        
        // Parse the signals to extract contact data
        var signalData = JsonSerializer.Deserialize<JsonElement>(body);
        
        if (signalData.TryGetProperty("firstName", out var firstNameElement))
        {
            _currentContact.FirstName = firstNameElement.GetString() ?? _currentContact.FirstName;
        }
        if (signalData.TryGetProperty("lastName", out var lastNameElement))
        {
            _currentContact.LastName = lastNameElement.GetString() ?? _currentContact.LastName;
        }
        if (signalData.TryGetProperty("email", out var emailElement))
        {
            _currentContact.Email = emailElement.GetString() ?? _currentContact.Email;
        }

        // Sanitize the values (simple profanity filter for demo)
        _currentContact.FirstName = SanitizeInput(_currentContact.FirstName);
        _currentContact.LastName = SanitizeInput(_currentContact.LastName);

        // Return to display view
        var displayHtml = $@"
<div id=""demo"">
    <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
    <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
    <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
    <div role=""group"">
        <button
            class=""button info""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@get('/Home/ClickToEditForm')""
        >
            Edit
        </button>
        <button
            class=""button warning""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@patch('/Home/ClickToEditReset')""
        >
            Reset
        </button>
    </div>
</div>";

        await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
    }

    public async Task ClickToEditCancel()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        // Return to display view
        var displayHtml = $@"
<div id=""demo"">
    <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
    <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
    <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
    <div role=""group"">
        <button
            class=""button info""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@get('/Home/ClickToEditForm')""
        >
            Edit
        </button>
        <button
            class=""button warning""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@patch('/Home/ClickToEditReset')""
        >
            Reset
        </button>
    </div>
</div>";

        await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
    }

    [HttpPatch]
    public async Task ClickToEditReset()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        // Reset to original contact data
        _currentContact = new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

        // Return to display view
        var displayHtml = $@"
<div id=""demo"">
    <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
    <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
    <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
    <div role=""group"">
        <button
            class=""button info""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@get('/Home/ClickToEditForm')""
        >
            Edit
        </button>
        <button
            class=""button warning""
            data-indicator:_fetching
            data-attr:disabled=""$_fetching""
            data-on:click=""@@patch('/Home/ClickToEditReset')""
        >
            Reset
        </button>
    </div>
</div>";

        await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
    }

    private static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Simple profanity filter - replace common profanity with asterisks
        var profanityList = new[] { "badword1", "badword2" };
        var result = input;
        
        foreach (var word in profanityList)
        {
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                word,
                new string('*', word.Length),
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        return result;
    }

    public IActionResult ClickToLoad()
    {
        var count = _random.Next(5, 10);
        var todoFaker = new Faker<Note>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Content, f => f.Lorem.Sentence());

        _notes = todoFaker.Generate(count).ToList();

        // Add exactly 1 notes with "hello"
        for (var i = 0; i < 1; i++)
        {
            var randomIndex = _random.Next(_notes.Count);
            _notes[randomIndex] = new Note
            {
                Id = _notes[randomIndex].Id,
                Content = $"hello! {new Faker().Lorem.Sentence()}"
            };
        }

        _totalNoteCount = _notes.Count;
        ViewData["TotalCount"] = _totalNoteCount;

        var displayNotes = _notes.Take(2).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    public IActionResult DeleteRow()
    {
        var todoFaker = new Faker<Note>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Content, f => f.Lorem.Sentence());

        _notes = todoFaker.Generate(5).ToList();
        _totalNoteCount = _notes.Count;
        ViewData["TotalCount"] = _totalNoteCount;
        var displayNotes = _notes.Take(5).ToList();

        return View(displayNotes);
    }

    public IActionResult DialogsBrowser()
    {
        return View();
    }

    public IActionResult EditRow()
    {
        return View();
    }

    public IActionResult FileUpload()
    {
        return View();
    }

    public IActionResult Indicator()
    {
        return View();
    }

    public IActionResult InfiniteScroll()
    {
        return View();
    }

    public IActionResult InlineValidation()
    {
        return View();
    }

    public IActionResult LazyLoad()
    {
        return View();
    }

    public IActionResult LazyTabs()
    {
        return View();
    }

    public IActionResult ProgressBar()
    {
        return View();
    }

    public IActionResult Sortable()
    {
        return View();
    }

    public IActionResult ValueSelect()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Datastar Actions

    [HttpPut]
    public async Task Search()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var query = body.Replace("{\"input\":\"", "").Replace("\"}", "").Trim();

        // Cold-start guard: initialize notes if this endpoint is called directly
        if (_notes == null || _notes.Count == 0)
        {
            var count = _random.Next(25, 102);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count).ToList();

            // Ensure some predictable matches for "hello"
            for (var i = 0; i < 3 && _notes.Count > 0; i++)
            {
                var randomIndex = _random.Next(_notes.Count);
                _notes[randomIndex] = new Note
                {
                    Id = _notes[randomIndex].Id,
                    Content = $"hello! {new Faker().Lorem.Sentence()}"
                };
            }

            _totalNoteCount = _notes.Count;
        }

        var filteredNotes = string.IsNullOrEmpty(query)
            ? _notes.Take(5).ToList()
            : _notes.Where(x => x.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        var countsHtml = filteredNotes.Count == 0
            ? $"<p id=\"total-count\">No results found out of {_totalNoteCount} notes</p>"
            : $"<p id=\"total-count\">Showing {filteredNotes.Count} of {_totalNoteCount} notes</p>";
        await SseHelper.SendServerSentEventAsync(Response, countsHtml);

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
        await SseHelper.SendServerSentEventAsync(Response, notesListHtml);
    }

    public async Task Progress()
    {
        var actionType = Request.Query["actionType"];

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

                await SseHelper.SendServerSentEventAsync(Response, progressBarHtml);
                await SseHelper.SendServerSentEventAsync(Response, progressBarPercentageHtml);
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
    }

    // Endpoint that returns the image of a graph after delay
    public async Task LazyLoadImgGraph()
    {
        // Set SSE headers
        await SseHelper.SetSseHeadersAsync(Response);

        // Simulate initial delay
        await Task.Delay(2000);

        // Create HTML with matching ID to replace the loading element
        // The element must have the same ID as the target element it's replacing
        const string graphHtml = @"<div id=""graph""><img src=""/img/tokyo.png"" alt=""Tokyo Graph"" style=""max-width: 100%; height: auto;"" /></div>";

        // Send the HTML to the client - it will morph into the element with id="graph"
        // Set end=true to close the SSE connection properly
        await SseHelper.SendServerSentEventAsync(Response, graphHtml, end: true);
    }

    public async Task FetchIndicator()
    {
        // Set SSE headers
        await SseHelper.SetSseHeadersAsync(Response);

        // Send the empty greeting  
        var indicatorEmptyGreetingHtml = $"<p id=\"greeting\">No data yet, please wait</p>";
        await SseHelper.SendServerSentEventAsync(Response, indicatorEmptyGreetingHtml);

        // Wait for 2 seconds
        await Task.Delay(2000);

        // Send the greeting
        var indicatorGreetingHtml = $"<p id=\"greeting\">Hello, the time is {DateTimeOffset.UtcNow:O}</p>";
        await SseHelper.SendServerSentEventAsync(Response, indicatorGreetingHtml);
    }

    public async Task ClickToLoadMore()
    {
        // Set SSE headers
        await SseHelper.SetSseHeadersAsync(Response);
        
        // Cold-start guard: ensure notes exist
        if (_notes == null || _notes.Count == 0)
        {
            var count = _random.Next(5, 10);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count).ToList();
            _totalNoteCount = _notes.Count;
        }

        // Read signals from the request (Datastar automatically sends them for GET requests in query param)
        ClickToLoadSignals signals = new ClickToLoadSignals { Offset = 0, Limit = 2 };
        
        if (HttpContext.Request.Query.ContainsKey("datastar"))
        {
            var json = HttpContext.Request.Query["datastar"].ToString();
            var tempSignals = JsonSerializer.Deserialize<ClickToLoadSignals>(json);
            if (tempSignals != null)
            {
                signals = tempSignals;
            }
        }

        // Get the filtered notes starting from current offset
        var filteredNotes = _notes
            .Skip(signals.Offset)
            .Take(signals.Limit)
            .ToList();

        // Update the total counts
        var totalCount = _totalNoteCount;
        var currentCount = Math.Min(signals.Offset + filteredNotes.Count, totalCount);
        var countHtml = $"<p id=\"total-count\">Showing {currentCount} of {totalCount} notes</p>";
        await SseHelper.SendServerSentEventAsync(Response, countHtml);

        // Build the html for the new notes
        var notesHtml = "";
        foreach (var note in filteredNotes)
        {
            notesHtml += $@"
                 <div class=""note-item"">
                     <p>{note.Content}</p>
                 </div>";
        }
        await SseHelper.SendServerSentEventAsync(Response, notesHtml, "#notes-list", "append", 1000);

        // Calculate new offset
        var newOffset = signals.Offset + signals.Limit;
        
        // Check if we've loaded all notes
        if (currentCount >= totalCount)
        {
            // Remove the load more button when all items are loaded
            var disabledButtonHtml = @"
                <button 
                    id=""load-more-btn"" 
                    class=""button-disabled""
                    disabled>
                    No More Results
                </button>";
            await SseHelper.SendServerSentEventAsync(Response, disabledButtonHtml, "#load-more-btn", "outer");
        }
        else
        {
            // Patch signals with new offset
            var signalsJson = $"{{\"offset\": {newOffset}}}";
            await SseHelper.PatchSignalsAsync(Response, signalsJson);
        }
    }

    [HttpDelete]
    public async Task DeleteRowData(int id)
    {
        // Send SSE response
        await SseHelper.SetSseHeadersAsync(Response);

        // delete the note from the list
        _notes.RemoveAll(x => x.Id == id);

        // update counts
            var countsHtml = _notes.Count == 0
                ? $"<p id=\"total-count\">No more notes</p><button id=\"load-more-btn\" data-on:click=\"@get('/Home/DeleteRowReset')\" class=\"button\">Reset</button>"
                : $"<p id=\"total-count\">Showing {_notes.Count} notes</p>";
        await SseHelper.SendServerSentEventAsync(Response, countsHtml);

        // Send a Datastar remove fragment event
        await SseHelper.RemoveElementsAsync(Response, $"#note_{id}");
    }

    public async Task DeleteRowReset()
    {
        // clear the _notes
        _notes.Clear();

        await SseHelper.SetSseHeadersAsync(Response);

        var todoFaker = new Faker<Note>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.Content, f => f.Lorem.Sentence());

        _notes = todoFaker.Generate(5).ToList();
        _totalNoteCount = _notes.Count;

        // update the total counts
        var countHtml = $"<p id=\"total-count\">Showing {_totalNoteCount} notes</p><button id=\"load-more-btn\" class=\"button button-disabled\">Reset</button>";
        await SseHelper.SendServerSentEventAsync(Response, countHtml);

        // build the html for the new notes
        var notesHtml = "<div id=\"notes-list\" class=\"notes-list\">";
        foreach (var note in _notes)
        {
            notesHtml += $@"
                    <div id=""note_{note.Id}"" class=""note-item"">
                        <p>{note.Content}</p>
                        <button data-on:click=""@delete('/Home/DeleteRowData/{note.Id}')"">Delete</button>
                    </div>";
        }
        notesHtml += "</div>";
        await SseHelper.SendServerSentEventAsync(Response, notesHtml, "#notes-list");
    }

    #endregion
}