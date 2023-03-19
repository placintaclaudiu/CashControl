using CashControl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CashControl.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context) { 
                _context= context;
        }
        public async Task<ActionResult> Index()
        {
            //Tranzactiile pe ultimele 7 zile
            DateTime DataInceput = DateTime.Today.AddDays(-6);
            DateTime DataFinal = DateTime.Today;


            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= DataInceput && y.Date<=DataFinal)
                .ToListAsync();

            //Total venituri

            int VenitTotal = SelectedTransactions
                .Where(i => i.Category.Type == "Venit")
                .Sum(j => j.Total);
            ViewBag.VenitTotal = VenitTotal.ToString("C2");

            //Total cheltuieli

            int CheltuieliTotale = SelectedTransactions
                .Where(i => i.Category.Type == "Cheltuiala")
                .Sum(j => j.Total);
            ViewBag.CheltuieliTotale = CheltuieliTotale.ToString("C2");

            //Balanta 

            int Balanta = VenitTotal - CheltuieliTotale;
            ViewBag.Balanta = Balanta.ToString("C2");
        
            //Numarul total de tranzactii

            int NumarTranzactii = SelectedTransactions.Count();
            CultureInfo culture= CultureInfo.CreateSpecificCulture("ro-RO");
            culture.NumberFormat.CurrencyNegativePattern = 1; 
            ViewBag.NumarTranzactii = String.Format(culture, "{0:C2}", Balanta);


            return View();
        }
    }
}
