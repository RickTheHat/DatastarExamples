namespace DatastarExamples.RazorSlicesApp.Models;

public sealed class BulkUpdatePageModel
{
    public required IReadOnlyList<User> Users { get; init; }

    public string SelectionDefaults => string.Join(", ", Enumerable.Repeat("false", Users.Count));

    public static BulkUpdatePageModel Create(Services.UserRepository userRepository) =>
        new()
        {
            Users = userRepository.GetAll()
        };
}
