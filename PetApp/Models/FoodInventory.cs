using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetApp.Models
{
    public class FoodInventory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int CurrentQuantity { get; set; }
        public int AlertThreshold { get; set; }


        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public Pet Pet { get; set; }
    }
}
