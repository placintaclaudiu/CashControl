using CashControl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Inputs;
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
            ViewBag.VenitTotal = VenitTotal.ToString("C0");

            //Total cheltuieli

            int CheltuieliTotale = SelectedTransactions
                .Where(i => i.Category.Type == "Cheltuiala")
                .Sum(j => j.Total);
            ViewBag.CheltuieliTotale = CheltuieliTotale.ToString("C0");

            //Balanta 

            int Balanta = VenitTotal - CheltuieliTotale;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("ro-RO");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balanta = String.Format(culture, "{0:C0}", Balanta);

            //Numarul total de tranzactii

            int NumarTranzactii = SelectedTransactions.Count();
            ViewBag.NumarTranzactii = NumarTranzactii;

            //Donut chart -> Cheltuieli in fct de categorie

            ViewBag.DonutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Cheltuiala")
                .GroupBy(j => j.Category.CategoryID)
                .Select(k => new
                {
                    CategorieCuEmoji = k.First().Category.Emoji+" "+ k.First().Category.Name,
                    total = k.Sum(j => j.Total),
                    formatedTotal = k.Sum(j => j.Total).ToString("C0"),
                })
                .OrderByDescending(l => l.total)
                .ToList();

            //Spline chart -> Cheltuieli & Venituri

            //Venit
            List<SplineChartData> RezumatVenituri = SelectedTransactions
                .Where(i => i.Category.Type == "Venit")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    zi = k.First().Date.ToString("dd-MMM(ro-RO)"),
                    venit = k.Sum(l => l.Total)             
                })
                .ToList();

            //Cheltuiala
            List<SplineChartData> RezumatCheltuieli = SelectedTransactions
                .Where(i => i.Category.Type == "Cheltuiala")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    zi = k.First().Date.ToString("dd-MMM(ro-RO)"), // Modificare culturalizare
                    cheltuiala = k.Sum(l => l.Total)
                })
                .ToList();

            //Concatenare venituri si cheltuieli

            string[] Ultimele7Zile = Enumerable.Range(0, 7)   //// Pt a nu sari peste zile unde nu se realizeaza nici o tranzactie
                .Select(i => DataInceput.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from zi in Ultimele7Zile
                                      join venit in RezumatVenituri on zi equals venit.zi into VenitZilnic
                                      from venit in VenitZilnic.DefaultIfEmpty()       // Se foloseste left join pentru ca nu vrem sa se pastreze doar elementele care satisfac conditia
                                      join cheltuiala in RezumatCheltuieli on zi equals cheltuiala.zi into CheltuialaZilnica
                                      from cheltuiala in CheltuialaZilnica.DefaultIfEmpty()
                                      select new
                                      {
                                          zi = zi,
                                          venit = venit == null ? 0 : venit.venit, // se verifica daca venitul este null, daca da returneaza 0 daca nu returneaza venitul din tabela venit
                                          cheltuiala = cheltuiala == null ? 0 : cheltuiala.cheltuiala,
                                      };

            // Tranzactii recente (ultimele tranzactii)

            ViewBag.TranzactiiRecente = await _context.Transactions
              .Include(i => i.Category)
              .OrderByDescending(j => j.Date)
              .Take(9)
              .ToListAsync();

            return View();
        }
    }

    public class SplineChartData{
        public string zi;
        public int venit;
        public int cheltuiala;
    }
}
