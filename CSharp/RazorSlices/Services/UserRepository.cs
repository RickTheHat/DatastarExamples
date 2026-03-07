using Bogus;
using DatastarExamples.RazorSlicesApp.Models;

namespace DatastarExamples.RazorSlicesApp.Services;

public sealed class UserRepository
{
    private readonly List<User> _users;

    public UserRepository()
    {
        var faker = new Faker<User>()
            .RuleFor(user => user.Id, fake => fake.IndexFaker)
            .RuleFor(user => user.Name, fake => fake.Name.FullName())
            .RuleFor(user => user.Email, fake => fake.Internet.Email())
            .RuleFor(user => user.Status, fake => fake.Random.Bool() ? "Active" : "Inactive");

        _users = faker.Generate(4);
    }

    public IReadOnlyList<User> GetAll() => _users;

    public void Activate(IEnumerable<int> selectedIndices) => UpdateStatuses(selectedIndices, "Active");

    public void Deactivate(IEnumerable<int> selectedIndices) => UpdateStatuses(selectedIndices, "Inactive");

    private void UpdateStatuses(IEnumerable<int> selectedIndices, string status)
    {
        foreach (var index in selectedIndices.Distinct())
        {
            if (index >= 0 && index < _users.Count)
            {
                _users[index].Status = status;
            }
        }
    }
}
