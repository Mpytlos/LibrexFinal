using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using librex3.Data;
using librex3.Models;
using librex3.ViewModels;

namespace librex3.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var books = await _context.Books
                .Select(book => new BookReservationViewModel
                {
                    Book = book,
                    IsReserved = _context.Loans.Any(l => l.BookId == book.Id && l.LoanDate == null),
                    IsReservedByCurrentUser = _context.Loans.Any(l => l.BookId == book.Id && l.UserId == userId && l.LoanDate == null)
                })
                .ToListAsync();

            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                TempData["Error"] = "Nie znaleziono książki.";
                return RedirectToAction("Index");
            }

            if (book.IsBlocked)
            {
                TempData["Error"] = "Książka jest już zarezerwowana.";
                return RedirectToAction("Index");
            }

            var existingReservation = await _context.Loans
                .FirstOrDefaultAsync(l => l.BookId == id && l.UserId == userId && l.LoanDate == null);

            if (existingReservation != null)
            {
                TempData["Error"] = "Już zarezerwowałeś tę książkę.";
                return RedirectToAction("Index");
            }

            var loan = new Loan
            {
                BookId = id,
                UserId = userId,
                LoanDate = null,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };

            book.IsBlocked = true;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Książka została zarezerwowana.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservation = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.BookId == id && l.UserId == userId && l.LoanDate == null);

            if (reservation == null)
            {
                TempData["Error"] = "Nie znaleziono rezerwacji do anulowania.";
                return RedirectToAction("Index");
            }

            reservation.Book.IsBlocked = false; 
            _context.Loans.Remove(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezerwacja została anulowana.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book); 
            }

            book.IsBlocked = false; 
            book.IsBorrowed = false; 

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }

    }
}

