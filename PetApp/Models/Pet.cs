using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PetApp.Areas.Identity.Data;

namespace PetApp.Models
{
    public class Pet
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Species { get; set; }

        public string? Breed { get; set; }
        public DateTime? BirthDate { get; set; }
        public float? Weight { get; set; }

        public string? PhotoUrl { get; set; }
        
        [NotMapped]
        public IFormFile? Photo { get; set; }

        public string UserId { get; set; }
        public PetAppUser User { get; set; }

        public ICollection<HealthRecord> HealthRecords { get; set; }
        public List<Activity> Activities { get; set; }
        public List<Medication> Medications { get; set; }
        public List<FoodInventory> FoodInventories { get; set; }
    }
}
