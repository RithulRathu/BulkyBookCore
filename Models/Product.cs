using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Please Enter Atleast Five Characters")]
        public string Title { get; set; }

        [Required,Display(Name ="ISBN")]
        public string Isbn { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Please Enter Atleast Five Characters")]
        public string Author {  get; set; }

        [Required]
        public string Description { get; set; }

        [Required, Display(Name = "List Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ListPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price50 { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price100 { get; set; }

        [ForeignKey("CategoryId"), Display(Name = "Category")]
        public int CategoryId {  get; set; }

        public virtual Category Category { get; set; }

        [ForeignKey("CoverType"), Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }

        public virtual CoverType CoverType { get; set; }

        [Required]
        public string Images { get; set; }
    }
}
