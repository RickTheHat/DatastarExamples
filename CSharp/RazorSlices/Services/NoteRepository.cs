using Bogus;
using DatastarExamples.RazorSlicesApp.Models;

namespace DatastarExamples.RazorSlicesApp.Services;

public sealed class NoteRepository
{
    private readonly List<Note> _notes;

    public NoteRepository()
    {
        var random = new Random();
        var count = random.Next(20, 983);
        var faker = new Faker<Note>()
            .RuleFor(note => note.Id, fake => fake.IndexFaker + 1)
            .RuleFor(note => note.Content, fake => fake.Lorem.Sentence());

        _notes = faker.Generate(count);

        for (var i = 0; i < 3; i++)
        {
            var randomIndex = random.Next(_notes.Count);
            _notes[randomIndex] = new Note
            {
                Id = _notes[randomIndex].Id,
                Content = $"Hello! {new Faker().Lorem.Sentence()}"
            };
        }
    }

    public int TotalCount => _notes.Count;

    public IReadOnlyList<Note> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return _notes.Take(5).ToList();
        }

        return _notes
            .Where(note => note.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
