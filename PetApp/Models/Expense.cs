using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PetApp.Areas.Identity.Data;

namespace PetApp.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Kwota jest wymagana")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kwota musi być większa niż 0")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public PetAppUser? User { get; set; }

        [ForeignKey("HealthRecord")]
        public int? HealthRecordId { get; set; }
        public HealthRecord? HealthRecord { get; set; }
    }
}
