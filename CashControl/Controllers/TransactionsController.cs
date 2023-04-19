using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CashControl.Models;
using Microsoft.AspNetCore.Authorization;

namespace CashControl.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Transactions.Include(t => t.Category);
            //return View(await applicationDbContext.ToListAsync());


            string userName = User.Identity.Name; // Get currently logged-in user name

            var transactions = await _context.Transactions
                .Where(t => t.ApplicationUser.UserName == userName) // Filter by user name
                .Include(t => t.Category)
                .ToListAsync();

            return View(transactions);


            //string userName = User.Identity.Name;

            //var transactions = await _context.Transactions
            //    .Where(t => t.ApplicationUser.UserName == userName)
            //    .Include(t => t.Category)
            //    .ToListAsync();

            //return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID");
            PopulareCategorii();
            return View(new Transaction());
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionID,CategoryID,Total,Description,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                string userName = User.Identity.Name; // Get currently logged-in user name
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName); // Get user from AspNetUsers table

                transaction.ApplicationUserId = user.Id; // Set the user ID
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulareCategorii();
            return View(transaction);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("TransactionID,CategoryID,Total,Description,Date")] Transaction transaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(transaction);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
        //    PopulareCategorii();
        //    return View(transaction);
        //}

        // GET: Transactions/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            string userName = User.Identity.Name; // Get currently logged-in user name

            var transaction = await _context.Transactions
                .Where(t => t.ApplicationUser.UserName == userName) // Filter by user name
                .FirstOrDefaultAsync(m => m.TransactionID == id);

            if (transaction == null)
            {
                return NotFound();
            }

            PopulareCategorii();
            return View(transaction);
        }

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Transactions == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transactions.FindAsync(id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }
        //    //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
        //    PopulareCategorii();
        //    return View(transaction);
        //}

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,CategoryID,Total,Description,Date")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
            PopulareCategorii();
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userName = User.Identity.Name; // Get currently logged-in user name

            var transaction = await _context.Transactions
                .Where(t => t.ApplicationUser.UserName == userName) // Filter by user name
                .FirstOrDefaultAsync(m => m.TransactionID == id);

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Transactions == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
        //    }
        //    var transaction = await _context.Transactions.FindAsync(id);
        //    if (transaction != null)
        //    {
        //        _context.Transactions.Remove(transaction);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool TransactionExists(int id)
        {
            return (_context.Transactions?.Any(e => e.TransactionID == id)).GetValueOrDefault();
        }


        // AM salvat colectia de categorii intr-un ViewBag pentru a putea fi apelat in dropdownlist in create
        [NonAction]
        public void PopulareCategorii()
        {
            var ColectieCategorii = _context.Categories.ToList();
            Category CategorieDefault = new Category() { CategoryID = 0, Name = "Alege o categorie" };
            ColectieCategorii.Insert(0, CategorieDefault);
            ViewBag.Categories = ColectieCategorii;
        }
    }
}







//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using CashControl.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace CashControl.Controllers
//{
//    [Authorize]
//    public class TransactionsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public TransactionsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Transactions
//        public async Task<IActionResult> Index()
//        {
//            var applicationDbContext = _context.Transactions.Include(t => t.Category);
//            return View(await applicationDbContext.ToListAsync());
//        }

//        // GET: Transactions/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null || _context.Transactions == null)
//            {
//                return NotFound();
//            }

//            var transaction = await _context.Transactions
//                .Include(t => t.Category)
//                .FirstOrDefaultAsync(m => m.TransactionID == id);
//            if (transaction == null)
//            {
//                return NotFound();
//            }

//            return View(transaction);
//        }

//        // GET: Transactions/Create
//        public IActionResult Create()
//        {
//            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID");
//            PopulareCategorii(); 
//            return View(new Transaction());
//        }

//        // POST: Transactions/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("TransactionID,CategoryID,Total,Description,Date")] Transaction transaction)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(transaction);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
//            PopulareCategorii();
//            return View(transaction);
//        }

//        // GET: Transactions/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null || _context.Transactions == null)
//            {
//                return NotFound();
//            }

//            var transaction = await _context.Transactions.FindAsync(id);
//            if (transaction == null)
//            {
//                return NotFound();
//            }
//            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
//            PopulareCategorii();
//            return View(transaction);
//        }

//        // POST: Transactions/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,CategoryID,Total,Description,Date")] Transaction transaction)
//        {
//            if (id != transaction.TransactionID)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(transaction);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!TransactionExists(transaction.TransactionID))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", transaction.CategoryID);
//            PopulareCategorii();
//            return View(transaction);
//        }

//        // GET: Transactions/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null || _context.Transactions == null)
//            {
//                return NotFound();
//            }

//            var transaction = await _context.Transactions
//                .Include(t => t.Category)
//                .FirstOrDefaultAsync(m => m.TransactionID == id);
//            if (transaction == null)
//            {
//                return NotFound();
//            }

//            return View(transaction);
//        }

//        // POST: Transactions/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            if (_context.Transactions == null)
//            {
//                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
//            }
//            var transaction = await _context.Transactions.FindAsync(id);
//            if (transaction != null)
//            {
//                _context.Transactions.Remove(transaction);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool TransactionExists(int id)
//        {
//          return (_context.Transactions?.Any(e => e.TransactionID == id)).GetValueOrDefault();
//        }


//        // AM salvat colectia de categorii intr-un ViewBag pentru a putea fi apelat in dropdownlist in create
//        [NonAction]
//        public void PopulareCategorii()
//        {
//            var ColectieCategorii = _context.Categories.ToList();
//            Category CategorieDefault = new Category() { CategoryID = 0, Name = "Alege o categorie" };
//            ColectieCategorii.Insert(0, CategorieDefault);
//            ViewBag.Categories = ColectieCategorii;
//        }
//    }
//}
