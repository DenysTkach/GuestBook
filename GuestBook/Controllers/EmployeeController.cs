using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuestBook.Models;
using GuestBook.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GuestBook.Controllers;

[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _context.GuestBookMessages
            .Include(m => m.User)
            .Include(m => m.Category)
            .Include(m => m.RepliedByUser)
            .Where(m => m.IsApproved)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return View(messages);
    }

    public async Task<IActionResult> Reply(int id)
    {
        var message = await _context.GuestBookMessages
            .Include(m => m.User)
            .Include(m => m.Category)
            .Include(m => m.RepliedByUser)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsApproved);
            
        if (message == null)
        {
            return NotFound();
        }
        
        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string reply)
    {
        var message = await _context.GuestBookMessages.FindAsync(id);
        
        if (message == null || !message.IsApproved)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(reply))
        {
            ModelState.AddModelError("reply", "Reply cannot be empty.");
            return View(await _context.GuestBookMessages
                .Include(m => m.User)
                .Include(m => m.Category)
                .Include(m => m.RepliedByUser)
                .FirstOrDefaultAsync(m => m.Id == id));
        }

        if (reply.Length > 2000)
        {
            ModelState.AddModelError("reply", "Reply cannot exceed 2000 characters.");
            return View(await _context.GuestBookMessages
                .Include(m => m.User)
                .Include(m => m.Category)
                .Include(m => m.RepliedByUser)
                .FirstOrDefaultAsync(m => m.Id == id));
        }

        message.Reply = reply;
        message.RepliedAt = DateTime.UtcNow;
        message.RepliedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Reply added successfully.";
        
        return RedirectToAction(nameof(Index));
    }
}
