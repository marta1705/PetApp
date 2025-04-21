using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetApp.DAL;
using PetApp.Models;

namespace PetApp.Controllers;

public class HomeController : Controller
{
    ApplicationDbContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        this.db = db;
    }

    public IActionResult Index()
    {

        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        if (User.Identity.IsAuthenticated) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var pets = db.Pets.Where(p => p.UserId == userId).ToList(); 
            ViewBag.Pets = pets;
        }

        return View();
    }

    [HttpPost]
    public IActionResult Index(Pet pet)
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            pet.UserId = userId;
            
            if (pet.Photo != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + pet.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        pet.Photo.CopyTo(fileStream);
                    }
                    pet.PhotoUrl = "/uploads/" + uniqueFileName;
                

            }

            if (pet.Id == 0)
            {
                db.Pets.Add(pet);
            }
            else
            {
                db.Pets.Update(pet);
            }

            db.SaveChanges();
        }

        return RedirectToAction("Index");
    }


    public IActionResult Edit(int id)
    {
        var pet = new Pet();
        if (id > 0)
        {
            pet = db.Pets.Find(id);
        }
        return View(pet);
    }

    public IActionResult Health()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var petsWithHealthRecords = db.Pets
                .Where(p => p.UserId == userId) 
                .Include(p => p.HealthRecords) 
                .ToList();

            foreach (var pet in petsWithHealthRecords)
            {
                pet.HealthRecords = pet.HealthRecords
                    .OrderByDescending(hr => hr.StartDate)
                    .ToList();
            }

            ViewBag.PetsWithHealthRecords = petsWithHealthRecords;
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    public IActionResult Delete(int id)
    {
        var pet = db.Pets.Find(id);
        if (pet != null)
        {
            db.Pets.Remove(pet);
            db.SaveChanges();
        }

        return RedirectToAction("Index");
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
            } else
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
            return RedirectToAction("Health");
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
            db.HealthRecords.Remove(healthRecord);
            db.SaveChanges();
        }
        return RedirectToAction("Health");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
