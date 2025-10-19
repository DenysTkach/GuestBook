using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return View(approvedMessages);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Message")] GuestBookMessage guestBookMessage)
    {
        if (ModelState.IsValid)
        {
            guestBookMessage.CreatedAt = DateTime.Now;
            guestBookMessage.IsApproved = false;
            _context.Add(guestBookMessage);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thank you! Your message has been submitted and is awaiting approval.";
            return RedirectToAction(nameof(Index));
        }
        return View(guestBookMessage);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
