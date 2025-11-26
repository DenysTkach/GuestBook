using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GuestBook.Models;
using GuestBook.Models.ViewModels;
using GuestBook.Data;
using Microsoft.EntityFrameworkCore;

namespace GuestBook.Controllers;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
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

    // User Management Actions
    public async Task<IActionResult> UserManagement()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                CreatedAt = user.CreatedAt
            });
        }

        return View(userViewModels);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(UserViewModel model)
    {
        if (string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError("Password", "Password is required for new users.");
            return View(model);
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign User role by default (role is seeded at startup)
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(UserManagement));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var model = new UserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles,
            CreatedAt = user.CreatedAt
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, UserViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Remove password validation for edit
        ModelState.Remove("Password");
        ModelState.Remove("ConfirmPassword");

        if (ModelState.IsValid)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Update password if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        var roles = await _userManager.GetRolesAsync(user);
                        model.Roles = roles;
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(UserManagement));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        model.Roles = currentRoles;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(UserManagement));
        }

        // Prevent deleting the current admin
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == user.Id)
        {
            TempData["ErrorMessage"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(UserManagement));
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete user.";
        }

        return RedirectToAction(nameof(UserManagement));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignEmployeeRole(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(UserManagement));
        }

        if (!await _roleManager.RoleExistsAsync("Employee"))
        {
            TempData["ErrorMessage"] = "Employee role does not exist.";
            return RedirectToAction(nameof(UserManagement));
        }

        if (!await _userManager.IsInRoleAsync(user, "Employee"))
        {
            await _userManager.AddToRoleAsync(user, "Employee");
            TempData["SuccessMessage"] = $"Employee role assigned to {user.Email}.";
        }
        else
        {
            TempData["ErrorMessage"] = "User already has the Employee role.";
        }

        return RedirectToAction(nameof(UserManagement));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployeeRole(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(UserManagement));
        }

        if (!await _roleManager.RoleExistsAsync("Employee"))
        {
            TempData["ErrorMessage"] = "Employee role does not exist.";
            return RedirectToAction(nameof(UserManagement));
        }

        if (await _userManager.IsInRoleAsync(user, "Employee"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Employee");
            TempData["SuccessMessage"] = $"Employee role removed from {user.Email}.";
        }
        else
        {
            TempData["ErrorMessage"] = "User does not have the Employee role.";
        }

        return RedirectToAction(nameof(UserManagement));
    }
}
