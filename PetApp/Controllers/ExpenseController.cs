using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PetApp.DAL;
using PetApp.Models;

namespace PetApp.Controllers
{
    public class ExpenseController : Controller
    {
        ApplicationDbContext db;

        public ExpenseController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(string? month, string sortBy = "date", string sortDirection = "desc")
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var allExpenses = db.Expenses
                .Where(e => e.UserId == userId)
                .ToList();

            DateTime selectedMonth;
            if (!string.IsNullOrEmpty(month) &&
                DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedMonth))
            {
                // Użyj przekazanego miesiąca
            }
            else
            {
                var latestExpense = allExpenses.OrderByDescending(e => e.Date).FirstOrDefault();

                if (latestExpense != null)
                {
                    selectedMonth = new DateTime(latestExpense.Date.Year, latestExpense.Date.Month, 1);
                }
                else
                {
                    selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                }
            }

            sortBy = sortBy.ToLower() == "amount" ? "amount" : "date";
            sortDirection = sortDirection.ToLower() == "asc" ? "asc" : "desc";

            var filteredExpenses = allExpenses
                .Where(e => e.Date.Year == selectedMonth.Year && e.Date.Month == selectedMonth.Month).ToList();

            // Sortowanie
            if (sortBy == "amount")
            {
                filteredExpenses = sortDirection == "asc"
                    ? filteredExpenses.OrderBy(e => e.Amount).ToList()
                    : filteredExpenses.OrderByDescending(e => e.Amount).ToList();
            }
            else
            {
                filteredExpenses = sortDirection == "asc"
                    ? filteredExpenses.OrderBy(e => e.Date).ToList()
                    : filteredExpenses.OrderByDescending(e => e.Date).ToList();
            }

            var availableMonths = allExpenses
                .Select(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            float totalAmount = filteredExpenses.Sum(e => e.Amount);
            float averageAmount = filteredExpenses.Count > 0 ? filteredExpenses.Average(e => e.Amount) : 0;
            string? mostFrequentCategory = filteredExpenses
    .GroupBy(e => e.Category)
    .OrderByDescending(g => g.Sum(e => e.Amount))
    .Select(g => g.Key)
    .FirstOrDefault();

            DateTime previousMonth = selectedMonth.AddMonths(-1);
            var previousMonthExpenses = allExpenses
                .Where(e => e.Date.Year == previousMonth.Year && e.Date.Month == previousMonth.Month)
                .ToList();
            float previousMonthTotal = previousMonthExpenses.Sum(e => e.Amount);
            float changefromPreviousMonth = previousMonthTotal == 0 ? 0 : ((totalAmount - previousMonthTotal) / previousMonthTotal) * 100;

            var viewModel = new ExpenseListViewModel
            {
                Expenses = filteredExpenses,
                SelectedMonth = selectedMonth,
                AvailableMonths = availableMonths,
                TotalAmount = totalAmount,
                AverageDailyAmount = averageAmount,
                MostFrequentCategory = mostFrequentCategory,
                ChangeFromPreviousMonth = changefromPreviousMonth
            };

            viewModel.ExpensesByCategory = filteredExpenses
                .GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            viewModel.ExpensesByDate = filteredExpenses
                .GroupBy(e => e.Date.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("dd MMMM", CultureInfo.CurrentCulture), g => g.Sum(e => e.Amount));

            return View(viewModel);
        }

        public IActionResult Add(int id)
        {
            Expense expense;
            if (id == 0)
            {
                expense = new Expense();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                expense.UserId = userId;
                expense.Date = DateTime.Today;
            } else
            {
                expense = db.Expenses.Find(id);
                if (expense == null)
                {
                    return NotFound();
                }
            }

            return View(expense);
        }

        [HttpPost]
        public IActionResult Add(Expense expense)
        {

            if (ModelState.IsValid)
            {
                    if (expense.Id == 0)
                    {
                        db.Expenses.Add(expense);
                    } else
                    {
                        db.Expenses.Update(expense);
                    }

                     db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(expense);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var expenseRecord = db.Expenses.Find(id);
            if (expenseRecord != null)
            {
                db.Expenses.Remove(expenseRecord);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
