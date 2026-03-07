using DatastarExamples.RazorSlicesApp.Helpers;
using DatastarExamples.RazorSlicesApp.Models;
using DatastarExamples.RazorSlicesApp.Services;
using DatastarExamples.RazorSlicesApp.Slices;
using Microsoft.AspNetCore.Http.HttpResults;
using RazorSlices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddWebEncoders();
}

builder.Services.AddSingleton<NoteRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.MapGet("/", () => Results.Extensions.RazorSlice<DatastarExamples.RazorSlicesApp.Slices.Index>());
app.MapGet("/active-search", (NoteRepository noteRepository) =>
    Results.Extensions.RazorSlice<ActiveSearch, ActiveSearchPageModel>(ActiveSearchPageModel.Create(noteRepository)));

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

app.Run();

static async Task<string> RenderSliceAsync(RazorSlice slice, IServiceProvider services)
{
    slice.ServiceProvider = services;

    var buffer = new StringBuilder();
    await slice.RenderAsync(buffer);
    return buffer.ToString();
}
