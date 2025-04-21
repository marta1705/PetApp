using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetApp.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        public float Duration { get; set; }
        public float Distance { get; set; }
        public DateTime Date { get; set; }


        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public Pet Pet { get; set; }
    }
}
