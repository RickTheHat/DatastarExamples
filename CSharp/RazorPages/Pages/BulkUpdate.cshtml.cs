using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Helpers;
using RazorPages.Models;
using Bogus;

namespace RazorPages.Pages;

[IgnoreAntiforgeryToken]
public class BulkUpdate : PageModel
{
    private static List<User> _users = new();
    private static List<User> _originalUsers = new();

    public List<User> Users { get; set; } = new();

    public void OnGet()
    {
        if (_users.Count == 0)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker)
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Status, f => f.Random.Bool() ? "Active" : "Inactive");

            _users = userFaker.Generate(4).ToList();
            _originalUsers = _users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Status = u.Status
            }).ToList();
        }

        Users = _users;
    }

    public async Task<IActionResult> OnPutActivateAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var selectedIndices = new List<int>();

            try
            {
                var signalData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                if (signalData != null && signalData.TryGetValue("selections", out var selectionsObj))
                {
                    if (selectionsObj is JsonElement selectionsElement)
                    {
                        var selections = JsonSerializer.Deserialize<List<bool>>(selectionsElement.GetRawText());
                        if (selections != null)
                        {
                            for (int i = 0; i < selections.Count && i < _users.Count; i++)
                            {
                                if (selections[i])
                                {
                                    selectedIndices.Add(i);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing selections: {ex.Message}");
            }

            foreach (var index in selectedIndices)
            {
                if (index >= 0 && index < _users.Count)
                {
                    _users[index].Status = "Active";
                }
            }

            var tableHtml = GenerateBulkUpdateTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#bulk-update-table", "inner");

            var clearSelections = $"{{\"selections\": [{string.Join(",", Enumerable.Repeat("false", _users.Count))}], \"_all\": false}}";
            await SseHelper.PatchSignalsAsync(Response, clearSelections);

            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#bulk-update-table");
            return new EmptyResult();
        }
    }

    public async Task<IActionResult> OnPutDeactivateAsync()
    {
        try
        {
            await SseHelper.SetSseHeadersAsync(Response);

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var selectedIndices = new List<int>();

            try
            {
                var signalData = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                if (signalData != null && signalData.TryGetValue("selections", out var selectionsObj))
                {
                    if (selectionsObj is JsonElement selectionsElement)
                    {
                        var selections = JsonSerializer.Deserialize<List<bool>>(selectionsElement.GetRawText());
                        if (selections != null)
                        {
                            for (int i = 0; i < selections.Count && i < _users.Count; i++)
                            {
                                if (selections[i])
                                {
                                    selectedIndices.Add(i);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing selections: {ex.Message}");
            }

            foreach (var index in selectedIndices)
            {
                if (index >= 0 && index < _users.Count)
                {
                    _users[index].Status = "Inactive";
                }
            }

            var tableHtml = GenerateBulkUpdateTable();
            await SseHelper.SendServerSentEventAsync(Response, tableHtml, "#bulk-update-table", "inner");

            var clearSelections = $"{{\"selections\": [{string.Join(",", Enumerable.Repeat("false", _users.Count))}], \"_all\": false}}";
            await SseHelper.PatchSignalsAsync(Response, clearSelections);

            return new EmptyResult();
        }
        catch (Exception ex)
        {
            var errorHtml = $"<p>Error: {ex.Message}</p>";
            await SseHelper.SendServerSentEventAsync(Response, errorHtml, "#bulk-update-table");
            return new EmptyResult();
        }
    }

    private string GenerateBulkUpdateTable()
    {
        var rows = new System.Text.StringBuilder();

        for (int i = 0; i < _users.Count; i++)
        {
            var user = _users[i];
            rows.AppendLine($@"
                <tr>
                    <td>
                        <input
                            type=""checkbox""
                            data-bind:selections
                            data-attr:disabled=""$_fetching""
                        />
                    </td>
                    <td>{user.Name}</td>
                    <td>{user.Email}</td>
                    <td>{user.Status}</td>
                </tr>");
        }

        return rows.ToString();
    }
}
