using System.ComponentModel.DataAnnotations.Schema;

namespace librex3.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime? LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOverdue
        {
            get
            {
                return DateTime.Now > DueDate;
            }
        }
        public bool IsReturned { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PenaltyFee { get; set; } // Opłata za przeterminowanie
        public bool IsBorrowed { get; set; }

    }
}
