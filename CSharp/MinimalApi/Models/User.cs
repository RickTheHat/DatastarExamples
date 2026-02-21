namespace MinimalAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Status { get; set; } // "Active" or "Inactive"
}
