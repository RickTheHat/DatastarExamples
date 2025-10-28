using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Helpers;
using RazorPages.Models;

namespace RazorPages.Pages;

[IgnoreAntiforgeryToken]
public class EditRow : PageModel
{
    // Edit Row state
    private static List<Contact> _contacts = new()
    {
        new Contact { Id = 0, FirstName = "Joe", LastName = "Smith", Email = "joe@smith.org" },
        new Contact { Id = 1, FirstName = "Angie", LastName = "MacDowell", Email = "angie@macdowell.org" },
        new Contact { Id = 2, FirstName = "Fuqua", LastName = "Tarkenton", Email = "fuqua@tarkenton.org" },
        new Contact { Id = 3, FirstName = "Kim", LastName = "Yee", Email = "kim@yee.org" }
    };

    private static List<Contact> _originalContacts = new()
    {
        new Contact { Id = 0, FirstName = "Joe", LastName = "Smith", Email = "joe@smith.org" },
        new Contact { Id = 1, FirstName = "Angie", LastName = "MacDowell", Email = "angie@macdowell.org" },
        new Contact { Id = 2, FirstName = "Fuqua", LastName = "Tarkenton", Email = "fuqua@tarkenton.org" },
        new Contact { Id = 3, FirstName = "Kim", LastName = "Yee", Email = "kim@yee.org" }
    };

    private static int? _editingContactId = null;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetEditRowForm(int id)
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            if (id < 0 || id >= _contacts.Count)
            {
                return new EmptyResult();
            }

            _editingContactId = id;

            // Generate the entire table with the edit form for this row
            var tableHtml = GenerateContactsTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#contacts-table", "inner");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnGetEditRowForm: {ex.Message}");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnPatchEditRowSave(int id)
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            if (id < 0 || id >= _contacts.Count)
            {
                return new EmptyResult();
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            try
            {
                var signalData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (signalData != null)
                {
                    var contact = _contacts[id];

                    // Look for name{id} signal
                    if (signalData.TryGetValue($"name{id}", out var name) && name != null)
                    {
                        var fullName = name.ToString()?.Split(' ', 2);
                        if (fullName?.Length == 2)
                        {
                            contact.FirstName = fullName[0];
                            contact.LastName = fullName[1];
                        }
                        else if (fullName?.Length == 1)
                        {
                            contact.FirstName = fullName[0];
                        }
                    }

                    // Look for email{id} signal
                    if (signalData.TryGetValue($"email{id}", out var email) && email != null)
                    {
                        contact.Email = email.ToString() ?? contact.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing signals: {ex.Message}");
            }

            _editingContactId = null;

            // Return the updated table
            var tableHtml = GenerateContactsTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#contacts-table", "inner");
            
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnPatchEditRowSave: {ex.Message}");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnGetEditRowCancel(int id)
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            _editingContactId = null;

            // Return the table in view mode
            var tableHtml = GenerateContactsTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#contacts-table", "inner");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnGetEditRowCancel: {ex.Message}");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnGetEditRowReset()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            // Reset contacts to original values
            for (int i = 0; i < _contacts.Count && i < _originalContacts.Count; i++)
            {
                _contacts[i].FirstName = _originalContacts[i].FirstName;
                _contacts[i].LastName = _originalContacts[i].LastName;
                _contacts[i].Email = _originalContacts[i].Email;
            }

            _editingContactId = null;

            // Return the reset table
            var tableHtml = GenerateContactsTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#contacts-table", "inner");
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnGetEditRowReset: {ex.Message}");
            return new EmptyResult();
        }
    }

    // Helper method to generate contacts table HTML
    private string GenerateContactsTable()
    {
        var rows = new StringBuilder();

        for (int i = 0; i < _contacts.Count; i++)
        {
            var contact = _contacts[i];

            if (_editingContactId == i)
            {
                // Edit mode row
                rows.AppendLine($@"
                <tr id=""contact-{i}"">
                    <td>
                        <input 
                            type=""text"" 
                            data-bind:name{i}
                            value=""{contact.FirstName} {contact.LastName}""
                        >
                    </td>
                    <td>
                        <input 
                            type=""email"" 
                            data-bind:email{i}
                            value=""{contact.Email}""
                        >
                    </td>
                    <td>
                        <div class=""edit-row-actions"">
                            <button 
                                class=""button success""
                                data-on:click=""@patch('?handler=EditRowSave&id={i}')""
                            >
                                Save
                            </button>
                            <button 
                                class=""button error""
                                data-on:click=""@get('?handler=EditRowCancel&id={i}')""
                            >
                                Cancel
                            </button>
                        </div>
                    </td>
                </tr>");
            }
            else
            {
                // View mode row
                rows.AppendLine($@"
                <tr id=""contact-{i}"">
                    <td>{contact.FirstName} {contact.LastName}</td>
                    <td>{contact.Email}</td>
                    <td>
                        <button 
                            class=""button info""
                            data-on:click=""@get('?handler=EditRowForm&id={i}')""
                            {(_editingContactId.HasValue && _editingContactId != i ? "disabled" : "")}
                        >
                            Edit
                        </button>
                    </td>
                </tr>");
            }
        }

        return rows.ToString();
    }
}