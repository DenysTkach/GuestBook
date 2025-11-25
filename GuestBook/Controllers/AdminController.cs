using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuestBook.Models;
using GuestBook.Data;
using Microsoft.EntityFrameworkCore;

namespace GuestBook.Controllers;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _context.GuestBookMessages
            .Include(m => m.User)
            .Include(m => m.Category)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return View(messages);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var message = await _context.GuestBookMessages.FindAsync(id);
        if (message != null)
        {
            message.IsApproved = true;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Message approved successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _context.GuestBookMessages.FindAsync(id);
        if (message != null)
        {
            _context.GuestBookMessages.Remove(message);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Message deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var message = await _context.GuestBookMessages.FindAsync(id);
        if (message == null || !message.IsApproved)
        {
            return NotFound();
        }
        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Message")] GuestBookMessage editedMessage)
    {
        if (id != editedMessage.Id)
        {
            return NotFound();
        }

        var message = await _context.GuestBookMessages.FindAsync(id);
        if (message == null || !message.IsApproved)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            message.Name = editedMessage.Name;
            message.Message = editedMessage.Message;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Message updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(editedMessage);
    }
}
