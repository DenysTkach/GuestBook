using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GuestBook.Models;
using GuestBook.Data;
using Microsoft.EntityFrameworkCore;

namespace GuestBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var approvedMessages = await _context.GuestBookMessages
            .Where(m => m.IsApproved)
            .Include(m => m.User)
            .Include(m => m.Category)
            .Include(m => m.RepliedByUser)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return View(approvedMessages);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Message,CategoryId")] GuestBookMessage guestBookMessage)
    {
        if (ModelState.IsValid)
        {
            guestBookMessage.CreatedAt = DateTime.Now;
            guestBookMessage.IsApproved = false;
            
            // Associate message with logged-in user if authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                guestBookMessage.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            
            _context.Add(guestBookMessage);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thank you! Your message has been submitted and is awaiting approval.";
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", guestBookMessage.CategoryId);
        return View(guestBookMessage);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
