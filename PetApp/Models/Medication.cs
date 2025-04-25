using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetApp.Models
{
    public class Medication
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string TimesOfDay { get; set; } // oddzielone przecinkiem godziny w formacie HH:mm

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public Pet? Pet { get; set; }
    }
}
