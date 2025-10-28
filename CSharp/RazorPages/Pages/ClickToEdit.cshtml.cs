using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Helpers;
using RazorPages.Models;

namespace RazorPages.Pages;

[IgnoreAntiforgeryToken]
public class ClickToEdit : PageModel
{
    private static Contact _currentContact = new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };
    private static Contact _originalContact = new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetFormAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            var editFormHtml = $@"
        <div id=""demo"" style=""display: flex; flex-direction: column; gap: 1rem;"">
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                First Name
                <input
                    type=""text""
                    data-bind:first-name
                    value=""{_currentContact.FirstName}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                Last Name
                <input
                    type=""text""
                    data-bind:last-name
                    value=""{_currentContact.LastName}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <label style=""display: flex; flex-direction: column; gap: 0.5rem;"">
                Email
                <input
                    type=""email""
                    data-bind:email
                    value=""{_currentContact.Email}""
                    data-attr:disabled=""$_fetching""
                    style=""padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px;""
                >
            </label>
            <div role=""group"" style=""display: flex; gap: 1rem;"">
                <button
                    class=""button success""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@post('./ClickToEdit?handler=Save')""
                >
                    Save
                </button>
                <button
                    class=""button error""
                    data-indicator:_fetching
                    data-attr:disabled=""$_fetching""
                    data-on:click=""@get('./ClickToEdit?handler=Cancel')""
                >
                    Cancel
                </button>
            </div>
        </div>";

            await SseHelper.SendServerSentEventAsync(Response, editFormHtml, "#demo");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#demo");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            try
            {
                var signalData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (signalData != null)
                {
                    if (signalData.TryGetValue("firstName", out var firstName) && firstName != null)
                        _currentContact.FirstName = firstName.ToString() ?? _currentContact.FirstName;
                    if (signalData.TryGetValue("lastName", out var lastName) && lastName != null)
                        _currentContact.LastName = lastName.ToString() ?? _currentContact.LastName;
                    if (signalData.TryGetValue("email", out var email) && email != null)
                        _currentContact.Email = email.ToString() ?? _currentContact.Email;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing signals: {ex.Message}");
            }

            var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
            <div role=""group"">
                <button class=""button info"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@get('./ClickToEdit?handler=Form')"">Edit</button>
                <button class=""button warning"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@post('./ClickToEdit?handler=Reset')"">Reset</button>
            </div>
        </div>";

            await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#demo");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnGetCancelAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
            <div role=""group"">
                <button class=""button info"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@get('./ClickToEdit?handler=Form')"">Edit</button>
                <button class=""button warning"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@post('./ClickToEdit?handler=Reset')"">Reset</button>
            </div>
        </div>";

            await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#demo");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnPostResetAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            _currentContact = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

            var displayHtml = $@"
        <div id=""demo"">
            <p>First Name: <span id=""display-first-name"">{_currentContact.FirstName}</span></p>
            <p>Last Name: <span id=""display-last-name"">{_currentContact.LastName}</span></p>
            <p>Email: <span id=""display-email"">{_currentContact.Email}</span></p>
            <div role=""group"">
                <button class=""button info"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@get('./ClickToEdit?handler=Form')"">Edit</button>
                <button class=""button warning"" data-indicator:_fetching data-attr:disabled=""$_fetching"" data-on:click=""@post('./ClickToEdit?handler=Reset')"">Reset</button>
            </div>
        </div>";

            await SseHelper.SendServerSentEventAsync(Response, displayHtml, "#demo");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#demo");
            return new EmptyResult();
        }
    }
}