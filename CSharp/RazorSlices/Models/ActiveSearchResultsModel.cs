namespace DatastarExamples.RazorSlicesApp.Models;

public sealed record ActiveSearchResultsModel(string Query, IReadOnlyList<Note> Notes);
