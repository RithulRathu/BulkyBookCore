using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BulkyBook.Models
{
    public class RegisterModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Name must be 50 digits long.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Please enter a valid email address")]
        [Required, Display(Name = "Email Address")]     
        public string Email { get; set; }

        [Required, Display(Name = "Phone Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage ="Invalid Phone Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number must be 10 digits long.")]
        public string PhoneNumber { get; set; }

        [Required, Display(Name="Street Address")]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal Code must be 6 digits long.")]
        public string PostalCode { get; set; }

        [Required]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8}$",ErrorMessage ="Password must contain 8 digit,one number, one alphabet upper and lower case")]
        public string Password {  get; set; }

        [Required]
        [NotMapped]
        public string ConformPassword { get; set; }

        public string Role { get; set; } = "User";

     
        public int? CompanyModelId { get; set; }

        [ForeignKey("CompanyModelId")]
        public CompanyModel CompanyModel { get; set; }

        public string AccountStatus { get; set; } = "Unlock";

        public bool EmailConfirmed { get; set; } = false;
    }
}
