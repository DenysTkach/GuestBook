using System.ComponentModel.DataAnnotations;

namespace GuestBook.Models;

public class GuestBookMessage
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsApproved { get; set; } = false;
    
    // Foreign key for User (optional - for anonymous messages)
    public string? UserId { get; set; }
    
    // Navigation property - message belongs to a user
    public ApplicationUser? User { get; set; }
    
    // Foreign key for Category (optional)
    public int? CategoryId { get; set; }
    
    // Navigation property - message belongs to a category
    public Category? Category { get; set; }
    
    // Employee reply functionality
    [StringLength(2000)]
    public string? Reply { get; set; }
    
    public DateTime? RepliedAt { get; set; }
    
    // Foreign key for the employee who replied
    public string? RepliedByUserId { get; set; }
    
    // Navigation property - reply made by an employee
    public ApplicationUser? RepliedByUser { get; set; }
}
