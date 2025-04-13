using App1.Models;
using System.Collections.Generic;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int UserId { get; set; }
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the number of reviews deleted by the user.
    /// </summary>
    public int NumberOfDeletedReviews { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has submitted an appeal.
    /// </summary>
    public bool HasSubmittedAppeal { get; set; }

    /// <summary>
    /// Gets or sets the list of roles assigned to the user.
    /// </summary>
    public List<Role> AssignedRoles { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with specified details.
    /// </summary>
    /// <param name="userId">The unique identifier for the user.</param>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="fullName">The full name of the user.</param>
    /// <param name="numberOfDeletedReviews">The number of reviews deleted by the user.</param>
    /// <param name="HasSubmittedAppeal">Indicates whether the user has submitted an appeal.</param>
    /// <param name="assignedRoles">The list of roles assigned to the user.</param>
    public User(int userId, string emailAddress, string fullName, int numberOfDeletedReviews, bool HasSubmittedAppeal, List<Role> assignedRoles)

    {
        UserId = userId;
        EmailAddress = emailAddress;
        FullName = fullName;
        NumberOfDeletedReviews = numberOfDeletedReviews;
        this.HasSubmittedAppeal = HasSubmittedAppeal;
        AssignedRoles = assignedRoles;

    }

    /// <summary>
    /// Returns a string representation of the user.
    /// </summary>
    /// <returns>A string containing the user's ID and email address.</returns>
    public override string ToString()
    {
        return "Id: " + UserId.ToString() + ", email: " + EmailAddress;
    }
}