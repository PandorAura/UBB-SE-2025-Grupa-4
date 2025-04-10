using App1.Models;
using System.Collections.Generic;

public class User
{
    public int UserId { get; set; }
    public string EmailAddress { get; set; }
    public string FullName { get; set; }
    public int NumberOfDeletedReviews { get; set; }
    public bool HasSubmittedAppeal { get; set; }
    public List<Role> AssignedRoles { get; set; }

    public User() { }

    public User(int userId, string emailAddress, string fullName, int numberOfDeletedReviews, bool HasSubmittedAppeal, List<Role> assignedRoles)
    
        {
        UserId = userId;
        EmailAddress = emailAddress;
        FullName = fullName;
        NumberOfDeletedReviews = numberOfDeletedReviews;
        this.HasSubmittedAppeal = HasSubmittedAppeal;
        AssignedRoles = assignedRoles;

    }

    public override string ToString()
    {
        return "Id: " + UserId.ToString() + ", email: " + EmailAddress;
    }
}