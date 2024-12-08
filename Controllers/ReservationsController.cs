using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using librex3.Data;
using librex3.Models;

[Authorize]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
      
        var userId = User.Identity.Name;

        var loans = await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.User.UserName == userId)
            .ToListAsync();

        return View(loans);
    }
}
