using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Cover Type")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Please Enter Atleast Five Characters")]
        public string Name { get; set; }

    }
}
