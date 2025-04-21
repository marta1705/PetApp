using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PetApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Pet> Pets { get; set; }
        public List<Expense> Expenses { get; set; }
        public List<Reminder> Reminders { get; set; }

    }
}
