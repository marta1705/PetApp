using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetApp.Models
{
    public class HealthRecord
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public float? Price { get; set; }


        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public Pet? Pet { get; set; }

        [NotMapped]
        public bool AddToExpenses { get; set; }


    }
}
