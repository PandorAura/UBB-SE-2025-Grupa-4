public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public int NumberOfDeletedReviews { get; set; }
    public int PermissionID { get; set; }
    public bool HasAppealed { get; set; }

    public User() { }

    public User(int userId, string email, string name, int numberOfDeletedReviews, int permissionID, bool hasAppealed)
    {
        UserId = userId;
        Email = email;
        Name = name;
        NumberOfDeletedReviews = numberOfDeletedReviews;
        PermissionID = permissionID;
        HasAppealed = hasAppealed;
    }

    public override string ToString()
    {
        return "Id: " + userId.ToString() + ", email: " + email;
    }
}