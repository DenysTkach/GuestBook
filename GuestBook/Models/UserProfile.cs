using System.ComponentModel.DataAnnotations;

namespace GuestBook.Models;

public class UserProfile
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Bio { get; set; }
    
    [StringLength(200)]
    public string? Location { get; set; }
    
    [Url]
    [StringLength(500)]
    public string? WebsiteUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property - one profile belongs to one user
    public ApplicationUser User { get; set; } = null!;
}
