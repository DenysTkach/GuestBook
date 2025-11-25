using Microsoft.AspNetCore.Identity;

namespace GuestBook.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation property - one user can have many messages
    public ICollection<GuestBookMessage> Messages { get; set; } = new List<GuestBookMessage>();
    
    // Navigation property - one user has one profile
    public UserProfile? Profile { get; set; }
}
