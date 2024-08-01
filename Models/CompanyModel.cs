using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class CompanyModel
    {
        [Key]
        public int Id { get; set; }

        [Required,Display(Name="Company Name")]
        public string Name { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public bool IsAuthorizesd { get; set; }

    }
}
