using Microsoft.EntityFrameworkCore;

namespace CashControl.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)// Constructor pentru clasa ApllicationDbContext
        {

        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
