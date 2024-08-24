
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class ProductDT
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public String Brand { get; set; } = "";
        [Required, MaxLength(100)]
        public String Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
    }
}
