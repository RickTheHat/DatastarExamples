using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Helpers;
using RazorPages.Models;

namespace RazorPages.Pages;

public class ActiveSearch : PageModel
{
    private static List<Note> _notes;
    private static int _totalNoteCount;
    private readonly Random _random = new();

    [BindProperty]
    public List<Note> Notes { get; set; }

    public void OnGet()
    {
        if (_notes == null)
        {
            var count = _random.Next(20, 983);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count);

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
        }

        ViewData["TotalCount"] = _totalNoteCount;
        Notes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = Notes.Count;
    }

    public async Task OnGetSearchAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            /*
            -- Request.Body is empty with GET
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var query = body.Replace("{\"input\":\"", "").Replace("\"}", "");
            */
            // TODO: Need a more robust way to handle datastar payloads. Probably a middleware that detects datastar requests (datastar sets a request header),
            //       parses the payload into a dictionary, and then puts on the HttpContext.Items collection.
            var body = Request.Query["datastar"].ToString();
            var query = body.Replace("{\"input\":\"", "").Replace("\"}", "");

            var filteredNotes = string.IsNullOrEmpty(query)
                ? _notes.Take(5).ToList()
                : _notes.Where(x => x.Content.Contains(query)).ToList();

            // Send the total count of notes and the count of notes being displayed
            var countsHtml = filteredNotes.Count == 0
                ? $"<p id=\"total-count\">No results found for \"{query}\" out of {_totalNoteCount} notes</p>"
                : $"<p id=\"total-count\">Showing {filteredNotes.Count} of {_totalNoteCount} notes</p>";
            await SseHelper.SendServerSentEventAsync(Response, countsHtml);

            // Send the notes list
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
        catch (Exception ex)
        {
            // TODO: Handle exceptions (e.g., log them)
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml);
        }
    }
}
