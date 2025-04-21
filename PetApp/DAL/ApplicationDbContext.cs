using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetApp.Areas.Identity.Data;
using PetApp.Models;

namespace PetApp.DAL
{
    public class ApplicationDbContext : IdentityDbContext<PetAppUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<FoodInventory> FoodInventory { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
