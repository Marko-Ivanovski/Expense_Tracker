using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public TransactionsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.transactions.Include(t => t.category);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Transactions/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            populateCategories();
            if (id == 0) {
                return View(new Transaction());
            } else {
                return View(_context.transactions.Find(id));
            }
        }

        // POST: Transactions/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("transactionID,categoryID,amount,note,date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.transactionID == 0) {
                    _context.Add(transaction);
                } else {
                    _context.Update(transaction);
                }
             
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            populateCategories();
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void populateCategories() {
            var categoryCollection = _context.categories.ToList();
            Category defaultCategory = new Category() { categoryID = 0, title  = "Choose a category"};
            categoryCollection.Insert(0, defaultCategory);
            ViewBag.Categories = categoryCollection;
        }
    }
}
