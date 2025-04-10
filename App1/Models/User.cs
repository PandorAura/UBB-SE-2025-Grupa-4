using App1.Models;
using System.Collections.Generic;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public int NumberOfDeletedReviews { get; set; }
    public bool HasAppealed { get; set; }
    public List<Role> Roles { get; set; }

    public User() { }

    public User(int userId, string email, string name, int numberOfDeletedReviews, bool hasAppealed, List<Role> roles)

    {
        UserId = userId;
        Email = email;
        Name = name;
        NumberOfDeletedReviews = numberOfDeletedReviews;
        HasAppealed = hasAppealed;
        Roles = roles;

    }

    public override string ToString()
    {
        return "Id: " + UserId.ToString() + ", email: " + Email;
    }
}