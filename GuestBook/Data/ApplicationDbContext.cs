using Microsoft.EntityFrameworkCore;
using GuestBook.Models;

namespace GuestBook.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GuestBookMessage> GuestBookMessages { get; set; }
}
