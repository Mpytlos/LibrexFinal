using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using librex3.Data;
using librex3.ViewModels;
using librex3.Models;

namespace librex3.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Get loans for admin or specific user
            var loansQuery = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.LoanDate != null && !l.IsReturned); // Only loans

            if (!isAdmin)
            {
                loansQuery = loansQuery.Where(l => l.UserId == userId);
            }

            var loans = await loansQuery
                .Select(l => new LoanViewModel
                {
                    LoanId = l.Id,
                    BookTitle = l.Book.Title,
                    BookAuthor = l.Book.Author,
                    UserEmail = l.User.Email,
                    LoanDate = l.LoanDate.Value,
                    DueDate = l.DueDate,
                    IsOverdue = l.DueDate < DateTime.Now && !l.IsReturned,
                    IsReturned = l.IsReturned
                })
                .ToListAsync();

            ViewBag.IsAdmin = isAdmin;
            return View(loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var book = await _context.Books.FindAsync(bookId);
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

            // Check if the user has already reserved this book
            var existingReservation = await _context.Loans
                .FirstOrDefaultAsync(l => l.BookId == bookId && l.UserId == userId && l.LoanDate == null);

            if (existingReservation != null)
            {
                TempData["Error"] = "Już zarezerwowałeś tę książkę.";
                return RedirectToAction("Index");
            }

            // Create reservation
            var loan = new Loan
            {
                BookId = bookId,
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int loanId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservation = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId && l.LoanDate == null);

            if (reservation == null)
            {
                TempData["Error"] = "Nie znaleziono rezerwacji.";
                return RedirectToAction("Index");
            }

            if (reservation.UserId != userId)
            {
                TempData["Error"] = "Nie możesz anulować rezerwacji dokonanej przez innego użytkownika.";
                return RedirectToAction("Index");
            }

            reservation.Book.IsBlocked = false;
            _context.Loans.Remove(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezerwacja została anulowana.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrow()
        {
            var availableBooks = await _context.Books
                .Where(b => !b.IsBorrowed) // Only books available for borrowing
                .Select(b => new BookBorrowViewModel
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ReservedByUserId = _context.Loans
                        .Where(l => l.BookId == b.Id && l.LoanDate == null)
                        .Select(l => l.UserId)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var users = await _context.Users
                .Select(u => new UserViewModel
                {
                    UserId = u.Id,
                    Email = u.Email
                })
                .ToListAsync();

            var borrowFormViewModel = new BorrowFormViewModel
            {
                AvailableBooks = availableBooks,
                Users = users
            };

            return View(borrowFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int bookId, string userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["Error"] = "Nie znaleziono książki.";
                return RedirectToAction("Index");
            }

            if (book.IsBorrowed)
            {
                TempData["Error"] = "Książka jest już wypożyczona.";
                return RedirectToAction("Index");
            }

            var reservation = await _context.Loans
                .FirstOrDefaultAsync(l => l.BookId == bookId && l.LoanDate == null);

            if (reservation != null && reservation.UserId != userId)
            {
                TempData["Error"] = "Książka jest zarezerwowana przez innego użytkownika.";
                return RedirectToAction("Index");
            }

            var loan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                IsReturned = false
            };

            book.IsBorrowed = true;
            book.IsBlocked = false; // Cancel reservation
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Książka została wypożyczona.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OverdueLoans()
        {
            var overdueLoans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.DueDate < DateTime.Now && !l.IsReturned)
                .Select(l => new LoanViewModel
                {
                    LoanId = l.Id,
                    BookTitle = l.Book.Title,
                    BookAuthor = l.Book.Author,
                    UserEmail = l.User.Email,
                    LoanDate = l.LoanDate.Value,
                    DueDate = l.DueDate,
                    IsOverdue = true,
                    IsReturned = l.IsReturned
                })
                .ToListAsync();

            return View(overdueLoans);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                TempData["Error"] = "Nie znaleziono wypożyczenia.";
                return RedirectToAction(nameof(Index));
            }

            loan.IsReturned = true;
            loan.Book.IsBorrowed = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Książka została oznaczona jako zwrócona.";
            return RedirectToAction(nameof(Index));
        }
    }
}
