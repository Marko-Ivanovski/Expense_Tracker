using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Syncfusion.EJ2.Charts;

namespace ExpenseTracker.Controllers {
    public class DashboardController : Controller {
        private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<ActionResult> Index() {
            DateTime startDate = DateTime.Today.AddDays(-6);
            DateTime endDate = DateTime.Today;

            List<Transaction> selectedTransaction = await _context.transactions
                .Include(x => x.category)
                .Where(y => y.date >= startDate && y.date <= endDate)
                .ToListAsync();


            //TOTAL INCOME
            int totalIncome = selectedTransaction
                .Where(i => i.category.type == "Income")
                .Sum(j => j.amount);
            ViewBag.totalIncome = totalIncome.ToString("C0");

            //TOTAL EXPENSE
            int totalExpense = selectedTransaction
                .Where(i => i.category.type == "Expense")
                .Sum(j => j.amount);
            ViewBag.totalExpense = totalExpense.ToString("C0");

            //BALANCE
            int balance = totalIncome - totalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.balance = String.Format(culture, "{0:C0}", balance);


            //DONUT CHART - EXPENSE BY CATEGORY
            ViewBag.DonutChartData = selectedTransaction
                .Where(i => i.category.type == "Expense")
                .GroupBy(j => j.category.categoryID)
                .Select(k => new {
                    categoryTitleWithIcon = k.First().category.icon + " " + k.First().category.title,
                    amount = k.Sum(j => j.amount),
                    formattedAmount = k.Sum(j => j.amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            //INCOME VS EXPENSE
            //INCOME
            List<SplineChartData> incomeSummary = selectedTransaction
                .Where(i => i.category.type == "Income")
                .GroupBy(j => j.date)
                .Select(k => new SplineChartData() {
                    day = k.First().date.ToString("dd-MMM"),
                    income = k.Sum(l => l.amount)
                })
                .ToList();

            //EXPENSE
            List<SplineChartData> expenseSummary = selectedTransaction
                .Where(i => i.category.type == "Expense")
                .GroupBy(j => j.date)
                .Select(k => new SplineChartData() {
                    day = k.First().date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.amount)
                })
                .ToList();

            // COMBINATION
            string[] last7Days = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in last7Days
                                      join income in incomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in expenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            // RECENT TRANSACTIONS
            ViewBag.recentTransactions = await _context.transactions
                .Include(i => i.category)
                .OrderByDescending(j => j.date)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChartData {
        public string day;
        public int income;
        public int expense;
    }
}
