using DatastarExamples.RazorSlicesApp.Helpers;
using DatastarExamples.RazorSlicesApp.Models;
using DatastarExamples.RazorSlicesApp.Services;
using DatastarExamples.RazorSlicesApp.Slices;
using RazorSlices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddWebEncoders();
}

builder.Services.AddSingleton<NoteRepository>();
builder.Services.AddSingleton<UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

var currentContact = new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "joe@blow.com" };

app.MapGet("/", () => Results.Extensions.RazorSlice<DatastarExamples.RazorSlicesApp.Slices.Index>());
app.MapGet("/active-search", (NoteRepository noteRepository) =>
    Results.Extensions.RazorSlice<ActiveSearch, ActiveSearchPageModel>(ActiveSearchPageModel.Create(noteRepository)));
app.MapGet("/bulk-update", (UserRepository userRepository) =>
    Results.Extensions.RazorSlice<BulkUpdate, BulkUpdatePageModel>(BulkUpdatePageModel.Create(userRepository)));
app.MapGet("/click-to-edit", () =>
    Results.Extensions.RazorSlice<ClickToEdit, ClickToEditPageModel>(new ClickToEditPageModel { Contact = currentContact }));

app.MapPut("/api/active-search", async (HttpContext context, NoteRepository noteRepository) =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    var query = await DatastarPayloadReader.ReadInputAsync(context.Request);
    var filteredNotes = noteRepository.Search(query).ToList();

    var countsHtml = await RenderSliceAsync(
        _ActiveSearchCounts.Create(new ActiveSearchCountsModel(query, filteredNotes.Count, noteRepository.TotalCount)),
        context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, countsHtml);

    var notesHtml = await RenderSliceAsync(
        _ActiveSearchNotes.Create(new ActiveSearchResultsModel(query, filteredNotes)),
        context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, notesHtml);
});

app.MapPut("/api/bulk-update/activate", (HttpContext context, UserRepository userRepository) =>
    HandleBulkUpdateAsync(context, userRepository, userRepository.Activate));

app.MapPut("/api/bulk-update/deactivate", (HttpContext context, UserRepository userRepository) =>
    HandleBulkUpdateAsync(context, userRepository, userRepository.Deactivate));

app.MapGet("/api/clicktoedit/form", async (HttpContext context) =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);
    var formHtml = await RenderSliceAsync(_ClickToEditForm.Create(currentContact), context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, formHtml, "#demo");
});

app.MapPut("/api/clicktoedit/save", async (HttpContext context) =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);
    var (firstName, lastName, email) = await DatastarPayloadReader.ReadContactSignalsAsync(context.Request);
    if (!string.IsNullOrEmpty(firstName)) currentContact.FirstName = firstName;
    if (!string.IsNullOrEmpty(lastName)) currentContact.LastName = lastName;
    if (!string.IsNullOrEmpty(email)) currentContact.Email = email;
    var displayHtml = await RenderSliceAsync(_ClickToEditDisplay.Create(currentContact), context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.MapGet("/api/clicktoedit/cancel", async (HttpContext context) =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);
    var displayHtml = await RenderSliceAsync(_ClickToEditDisplay.Create(currentContact), context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.MapPatch("/api/clicktoedit/reset", async (HttpContext context) =>
{
    await SseHelper.SetSseHeadersAsync(context.Response);
    currentContact.FirstName = "John";
    currentContact.LastName = "Doe";
    currentContact.Email = "joe@blow.com";
    var displayHtml = await RenderSliceAsync(_ClickToEditDisplay.Create(currentContact), context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, displayHtml, "#demo");
});

app.Run();

static async Task<string> RenderSliceAsync(RazorSlice slice, IServiceProvider services)
{
    slice.ServiceProvider = services;

    var buffer = new StringBuilder();
    await slice.RenderAsync(buffer);
    return buffer.ToString();
}

static async Task HandleBulkUpdateAsync(
    HttpContext context,
    UserRepository userRepository,
    Action<IEnumerable<int>> updateUsers)
{
    await SseHelper.SetSseHeadersAsync(context.Response);

    var users = userRepository.GetAll();
    var selectedIndices = await DatastarPayloadReader.ReadSelectedIndicesAsync(context.Request, users.Count);

    updateUsers(selectedIndices);

    var rowsHtml = await RenderSliceAsync(
        _BulkUpdateRows.Create(new BulkUpdateRowsModel(users)),
        context.RequestServices);
    await SseHelper.SendServerSentEventAsync(context.Response, rowsHtml, "#bulk-update-table", "outer");

    var clearSelections = $"{{\"selections\":[{string.Join(",", Enumerable.Repeat("false", users.Count))}],\"_all\":false}}";
    await SseHelper.PatchSignalsAsync(context.Response, clearSelections);
}
