using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CashControl.Areas.Identity.Data;

namespace CashControl.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        //Cheie straina user + Navigation Property
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        //Cheie straina Category + Navigation Property
        [Range(1, int.MaxValue, ErrorMessage = "Selectati o categorie!")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }
        //
        [Range(1, int.MaxValue, ErrorMessage = "Suma trebuie sa fie mai mare decat 0!")]
        public int Total { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Description { get; set; } // Not null "?" pentru a putea lasa fara descriere
        public DateTime Date { get; set; } = DateTime.Now;


        //Daca categoria este goala va returna un string cu spatiu gol altfel se va returna emoji cu denumirea concatenate
        [NotMapped] // pt a nu fi stocat in BD
        public string? CategorieCuEmoji {
            get
            {
                return Category == null ? "" : Category.Emoji + " " + Category.Name;
            }
        }

        // Pentru a formata suma in functie de tipul ei: pt venit cu + si pt cheltuiala cu -
        [NotMapped] 
        public string? SumaCorecta {
            get
            {
                return ((Category == null || Category.Type == "Cheltuiala") ? "- " : "+ ") + Total.ToString("C0");
            }
        }
    }
}
