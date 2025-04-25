using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp.DAL;
using PetApp.Models;

namespace PetApp.Controllers
{
    public class ActivityController : Controller
    {
        private ApplicationDbContext db;

        public ActivityController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Add(int id)
        {
            Activity activity;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pets = db.Pets.Where(p => p.UserId == userId).ToList();
            ViewBag.Pets = pets;

            if (id == 0)
            {
                activity = new Activity();
                activity.Date = DateTime.Today;
            }
            else
            {
                activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return NotFound();
                }
            }

            return View(activity);
        }

        [HttpPost]
        public IActionResult Add(Activity activity)
        {
            string durationInput = Request.Form["DurationInput"];

            if (!Regex.IsMatch(durationInput, @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                ModelState.AddModelError("DurationInput", "Wprowadź czas w formacie hh:mm");
            } else
            {
                var parts = durationInput.Split(':');
                int hours = int.Parse(parts[0]);
                int minutes = int.Parse(parts[1]);
                activity.Duration = hours + minutes / 60f;
                Console.WriteLine(hours + minutes / 60f);
            }

            if (ModelState.IsValid)
            {
                Pet pet = db.Pets.Find(activity.PetId);
                if (pet != null)
                {
                    activity.Pet = pet;
                }

                if (activity.Id == 0)
                {
                    db.Activities.Add(activity);
                }
                else
                {
                    db.Activities.Update(activity);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Błąd w polu {state.Key}: {error.ErrorMessage}");
                    }
                }
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Pets = db.Pets.Where(p => p.UserId == userId).ToList();
            return View(activity);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = db.Activities.Find(id);
            if (record != null)
            {
                db.Activities.Remove(record);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Index(string? month, string sortBy = "date", string sortDirection = "desc")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var allActivities = db.Activities
                .Include(a => a.Pet)
                .Where(a => a.Pet.UserId == userId)
                .ToList();

            DateTime selectedMonth;
            if (!string.IsNullOrEmpty(month) &&
            DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedMonth))
            {
                // użyj podanego miesiąca
            }
            else
            {
                var latestActivity = allActivities.OrderByDescending(a => a.Date).FirstOrDefault();
                selectedMonth = latestActivity != null
                    ? new DateTime(latestActivity.Date.Year, latestActivity.Date.Month, 1)
                    : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var filteredActivities = allActivities
                .Where(a => a.Date.Year == selectedMonth.Year && a.Date.Month == selectedMonth.Month)
                .ToList();

            if (sortBy == "distance")
            {
                filteredActivities = sortDirection == "asc"
                    ? filteredActivities.OrderBy(a => a.Distance).ToList()
                    : filteredActivities.OrderByDescending(a => a.Distance).ToList();
            }
            else if (sortBy == "date")
            {
                filteredActivities = sortDirection == "asc"
                    ? filteredActivities.OrderBy(a => a.Date).ToList()
                    : filteredActivities.OrderByDescending(a => a.Date).ToList();
            } else
            {
                filteredActivities = sortDirection == "asc"
                    ? filteredActivities.OrderBy(a => a.Duration).ToList()
                    : filteredActivities.OrderByDescending(a => a.Duration).ToList();
            }

                var availableMonths = allActivities
                    .Select(a => new DateTime(a.Date.Year, a.Date.Month, 1))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

            float totalDuration = filteredActivities.Sum(a => a.Duration);
            int totalHours = (int)totalDuration;
            int totalMinutes = (int)((totalDuration - totalHours) * 60);
            string formattedTotalDuration = $"{totalHours}h {totalMinutes}min";

            float totalDistance = filteredActivities.Sum(a => a.Distance);

            int today = DateTime.Today.Day;
            float averageDuration = filteredActivities.Count > 0 ? filteredActivities.Sum(a => a.Duration) / today : 0;
            int hours = (int)averageDuration;
            int minutes = (int)((averageDuration - hours) * 60);
            string formattedAverageDuration = $"{hours}h {minutes}min";

            DateTime prevMonth = selectedMonth.AddMonths(-1);
            var previousMonthActivities = allActivities
                .Where(a => a.Date.Year == prevMonth.Year && a.Date.Month == prevMonth.Month)
                .ToList();
            float prevTotalDuration = previousMonthActivities.Sum(a => a.Duration);
            float changeFromPrevious = prevTotalDuration == 0 ? 0 : ((totalDuration - prevTotalDuration) / prevTotalDuration) * 100;

            var viewModel = new ActivityListViewModel
            {
                Activities = filteredActivities,
                SelectedMonth = selectedMonth,
                AvailableMonths = availableMonths,
                TotalDuration = formattedTotalDuration,
                AverageDailyDuration = formattedAverageDuration,
                TotalDistance = totalDistance,
                ChangeFromPreviousMonth = changeFromPrevious,
                DurationByCategory = filteredActivities
                .GroupBy(a => a.Category)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.Duration)),
                DurationByDate = filteredActivities
                .GroupBy(a => a.Date.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("dd MMMM", CultureInfo.CurrentCulture), g => g.Sum(a => a.Duration)),
                DistanceByDate = filteredActivities
                .GroupBy(a => a.Date.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("dd MMMM", CultureInfo.CurrentCulture), g => g.Sum(a => a.Distance))
            };

            var currentYear = DateTime.Today.Year;

            var monthlyActivity = allActivities
                .Where(a => a.Date.Year == currentYear)
                .GroupBy(a => new DateTime(a.Date.Year, a.Date.Month, 1))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("yyyy-MM"), g => g.Sum(a => a.Duration));

            viewModel.MonthlyActivity = monthlyActivity;

            var userPets = db.Pets
                .Where(p => p.UserId == userId)
                .ToList();
            bool hasMultiplePets = userPets.Count > 1;
            ViewBag.HasMultiplePets = hasMultiplePets;

            var byPet = userPets
                .ToDictionary(
                    pet => pet.Name,
                    pet => filteredActivities
                        .Where(a => a.PetId == pet.Id)
                        .Sum(a => a.Duration)
                );

            viewModel.ActivityByPet = byPet;

            return View(viewModel);
        }
    }
}