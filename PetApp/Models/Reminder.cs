using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PetApp.Areas.Identity.Data;

namespace PetApp.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSent { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public PetAppUser? User { get; set; }
    }
}
