using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required,Display(Name= "Category Name")]
        [StringLength(100, MinimumLength = 5,ErrorMessage ="Please Enter Atleast Five Characters")]
        public string Name { get; set; }    

    }
}
