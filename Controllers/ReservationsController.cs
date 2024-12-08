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
        // Pobierz identyfikator zalogowanego użytkownika
        var userId = User.Identity.Name;

        // Pobierz rezerwacje przypisane do zalogowanego użytkownika
        var loans = await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.User.UserName == userId)
            .ToListAsync();

        return View(loans);
    }
}
