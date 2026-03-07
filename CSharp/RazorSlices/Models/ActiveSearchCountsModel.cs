namespace DatastarExamples.RazorSlicesApp.Models;

public sealed record ActiveSearchCountsModel(string Query, int CurrentCount, int TotalCount);
