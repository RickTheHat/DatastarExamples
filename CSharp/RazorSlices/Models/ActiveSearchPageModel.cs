namespace DatastarExamples.RazorSlicesApp.Models;

public sealed class ActiveSearchPageModel
{
    public required IReadOnlyList<Note> Notes { get; init; }

    public required int CurrentCount { get; init; }

    public required int TotalCount { get; init; }

    public static ActiveSearchPageModel Create(Services.NoteRepository noteRepository)
    {
        var notes = noteRepository.Search(string.Empty);
        return new ActiveSearchPageModel
        {
            Notes = notes,
            CurrentCount = notes.Count,
            TotalCount = noteRepository.TotalCount
        };
    }
}
