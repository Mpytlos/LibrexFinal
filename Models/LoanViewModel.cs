public class LoanViewModel
{
    public int LoanId { get; set; }
    public string BookTitle { get; set; }
    public string BookAuthor { get; set; }
    public string UserEmail { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsReturned { get; set; }
    public string UserId { get; set; }
    public bool IsBorrowed { get; set; }
}

public class BookBorrowViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ReservedByUserId { get; set; }
}

public class UserViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
}

public class BorrowFormViewModel
{
    public List<BookBorrowViewModel> AvailableBooks { get; set; }
    public List<UserViewModel> Users { get; set; }
}
