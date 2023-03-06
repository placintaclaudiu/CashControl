using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CashControl.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        //Cheie straina Category
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        //
        public int Total { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Description { get; set; } // Not null "?" pentru a putea lasa fara descriere
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
