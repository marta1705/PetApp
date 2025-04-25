using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp.DAL;
using PetApp.Models;

namespace PetApp.Controllers
{
    public class HealthController : Controller
    {
        private ApplicationDbContext db;

        public HealthController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Health(int? petId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var petsWithHealthRecords = db.Pets
                    .Where(p => p.UserId == userId)
                    .Include(p => p.HealthRecords)
                    .Include(p => p.Medications)
                    .ToList();

                foreach (var pet in petsWithHealthRecords)
                {
                    pet.HealthRecords = pet.HealthRecords
                        .OrderByDescending(hr => hr.StartDate)
                        .ToList();
                }

                ViewBag.PetsWithHealthRecords = petsWithHealthRecords;

                var selectedPet = petsWithHealthRecords.FirstOrDefault(p => p.Id == petId) ?? petsWithHealthRecords.FirstOrDefault();
                if (selectedPet != null)
                {
                    ViewBag.SelectedPet = selectedPet;
                }


            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult AddHealthRecord(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pets = db.Pets.Where(p => p.UserId == userId).ToList();
            ViewBag.Pets = pets;

            HealthRecord healthRecord;

            if (id == 0)
            {
                healthRecord = new HealthRecord();
                healthRecord.StartDate = DateTime.Today;
            }
            else
            {
                healthRecord = db.HealthRecords.Find(id);
                if (healthRecord == null)
                {
                    return NotFound();
                }
            }
            return View(healthRecord);
        }

        [HttpPost]
        public IActionResult AddHealthRecord(HealthRecord healthRecord)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                if (healthRecord.Price == null)
                {
                    healthRecord.Price = 0;
                }

                if (healthRecord.Id == 0)
                {
                    db.HealthRecords.Add(healthRecord);
                }
                else
                {
                    db.HealthRecords.Update(healthRecord);
                }
                db.SaveChanges();


                if (healthRecord.AddToExpenses && healthRecord.Price != null)
                {
                    var expense = new Expense
                    {
                        Amount = (float)healthRecord.Price,
                        Category = "Weterynarz",
                        Date = healthRecord.StartDate,
                        UserId = userId,
                        HealthRecordId = healthRecord.Id
                    };
                    db.Expenses.Add(expense);
                    db.SaveChanges();
                }
                return RedirectToAction("Health", new {petId = healthRecord.PetId});
            }
            else
            {
                Console.WriteLine("ModelState jest niepoprawny!");

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            var pets = db.Pets.Where(p => p.UserId == userId).ToList();
            ViewBag.Pets = pets;

            return View(healthRecord);
        }

        [HttpPost]
        public IActionResult DeleteHealthRecord(int id)
        {
            var healthRecord = db.HealthRecords.Find(id);
            if (healthRecord != null)
            {
                int petId = healthRecord.PetId;
                db.HealthRecords.Remove(healthRecord);
                db.SaveChanges();

                return RedirectToAction("Health", new { petId = petId });
            }
            return RedirectToAction("Health");
        }

        public IActionResult AddMedication(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pets = db.Pets.Where(p => p.UserId == userId).ToList();
            ViewBag.Pets = pets;

            Medication medication;

            if (id == 0)
            {
                medication = new Medication();
            } else
            {
                medication = db.Medications.Find(id);
                if (medication == null)
                {
                    return NotFound();
                }
            }
            return View(medication);
        }

        [HttpPost]
        public IActionResult AddMedication(Medication medication, bool addReminders)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool validTimes = medication.TimesOfDay.Split(',').All(t => TimeSpan.TryParse(t.Trim(), out _));

            if (!validTimes)
            {
                ModelState.AddModelError("TimesOfDay", "Godziny muszą być w formacie HH:mm i oddzielone przecinkiem.");
            }

            if (ModelState.IsValid)
            {
                if (medication.Id == 0)
                {
                    var pet = db.Pets.Find(medication.PetId);
                    medication.Pet = pet;
                    db.Medications.Add(medication);
                } else
                {
                    db.Medications.Update(medication);
                }
                db.SaveChanges();

                if (addReminders)
                {
                    AddMedicationReminders(medication);
                }

                return RedirectToAction("Health", new { petId = medication.PetId });
            } else
            {
                Console.WriteLine("ModelState jest niepoprawny!");

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }


            var pets = db.Pets.Where(p => p.UserId == userId).ToList();
            ViewBag.Pets = pets;

            return View(medication);
        }

        private void AddMedicationReminders(Medication medication)
        {
            var reminders = new List<DateTime>();
            DateTime currentDate = medication.StartDate;

            var times = medication.TimesOfDay.Split(',').Select(t => TimeSpan.Parse(t.Trim())).ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            for (var date = medication.StartDate; date <= (medication.EndDate ?? medication.StartDate).Date; date = date.AddDays(1))
            {
                foreach (var time in times)
                {
                    var reminderTime = date.Add(time);
                    var reminder = new Reminder
                    {
                        UserId = userId,
                        DueDate = reminderTime,
                        Type = "Podaj " + medication.Pet?.Name + ", lek: " + medication.Name,
                        IsSent = false,
                        IsCompleted = false
                    };
                    db.Reminders.Add(reminder);
                }
            }

            db.SaveChanges();
        }

    }
}
