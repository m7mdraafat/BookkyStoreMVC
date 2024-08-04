using Store.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="Category Name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Display(Name = "Display Order")]

        [Range(1, 100)]
        public int DisplayOrder { get; set; }
        //public ICollection<Product>? Products { get; set; } = new List<Product>();      
        
    }
}
