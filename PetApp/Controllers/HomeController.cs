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
            var reminders = db.Reminders.Where(r => r.UserId == userId).OrderBy(r => r.IsCompleted).ThenBy(r => r.DueDate).ToList();
            ViewBag.Pets = pets;
            ViewBag.Reminders = reminders;

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


    [HttpPost]
    public IActionResult AddReminder(Reminder reminder)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        reminder.UserId = userId;
        reminder.IsCompleted = false;
        reminder.IsSent = true;


        if (ModelState.IsValid)
        {
            Console.WriteLine("Model jest poprawny");
            db.Reminders.Add(reminder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            Console.WriteLine("Model NIE jest poprawny");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return RedirectToAction("Index"); 
        }
    }

    [HttpPost]
    public IActionResult ToggleReminder(int id)
    {
        var reminder = db.Reminders.Find(id);

        if (reminder != null)
        {
            reminder.IsCompleted = !reminder.IsCompleted;
            db.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteReminder(int id)
    {
        var reminder = db.Reminders.Find(id);
        if (reminder != null)
        {
            db.Reminders.Remove(reminder);
            db.SaveChanges();
        }

        return RedirectToAction("Index");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
