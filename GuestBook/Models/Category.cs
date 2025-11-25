using System.ComponentModel.DataAnnotations;

namespace GuestBook.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property - one category can have many messages
    public ICollection<GuestBookMessage> Messages { get; set; } = new List<GuestBookMessage>();
}
