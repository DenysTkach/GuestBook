namespace GuestBook.Models;

public class GuestBookMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsApproved { get; set; } = false;
}
