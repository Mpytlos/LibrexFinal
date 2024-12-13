using librex3.Data;
using librex3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Panel administracyjny
    public async Task<IActionResult> IndexAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.UserName = currentUser?.UserName;
        return View();
    }

    // Blokowanie książki
    [HttpPost]
    public async Task<IActionResult> BlockBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            book.IsBlocked = true;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Książka '{book.Title}' została zablokowana.";
        }
        else
        {
            TempData["Error"] = "Nie znaleziono książki o podanym ID.";
        }
        return RedirectToAction("Index", "Books");
    }

    // Anulowanie kary
    [HttpPost]
    public async Task<IActionResult> CancelPenalty(int loanId)
    {
        var loan = await _context.Loans.Include(l => l.User).Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == loanId);
        if (loan != null)
        {
            loan.PenaltyFee = 0;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Kara za książkę '{loan.Book.Title}' została anulowana.";
        }
        else
        {
            TempData["Error"] = "Nie znaleziono wypożyczenia o podanym ID.";
        }
        return RedirectToAction("Index", "Loans");
    }
    // Widok dodawania książki
    public IActionResult AddBook()
    {
        return View();
    }

    // Akcja dodawania książki
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(Book model)
    {
        if (ModelState.IsValid)
        {
            _context.Books.Add(model);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Książka została pomyślnie dodana!";
            return RedirectToAction(nameof(ManageBooks));
        }

        return View(model);
    }

    // Zarządzaj książkami
    public async Task<IActionResult> ManageBooks()
    {
        var books = await _context.Books.ToListAsync();
        return View(books);
    }

    // Zarządzaj karami
    public async Task<IActionResult> ManagePenalties()
    {
        var loans = await _context.Loans
            .Include(l => l.User)
            .Include(l => l.Book)
            .Where(l => DateTime.Now > l.DueDate)
            .ToListAsync();

        return View(loans);
    }

    // Usuń książkę
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Książka została usunięta.";
        }

        return RedirectToAction(nameof(ManageBooks));
    }


}

