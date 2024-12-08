using System.ComponentModel.DataAnnotations;

namespace librex3.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(100, ErrorMessage = "Tytuł może mieć maksymalnie 100 znaków.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Autor jest wymagany.")]
        [StringLength(100, ErrorMessage = "Autor może mieć maksymalnie 100 znaków.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Gatunek jest wymagany.")]
        [StringLength(50, ErrorMessage = "Gatunek może mieć maksymalnie 50 znaków.")]
        public string Genre { get; set; }

        public bool IsBlocked { get; set; } //Rezerwacja
        public bool IsReturned { get; set; }
        public bool IsBorrowed { get; set; } // Wypożyczenie
    }
}
