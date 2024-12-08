using librex3.Models;

namespace librex3.ViewModels
{
    public class BookReservationViewModel
    {
        public Book Book { get; set; }
        public bool IsReserved { get; set; }
        public bool IsReservedByCurrentUser { get; set; }
        public bool IsBorrowed { get; set; }
    }
}
