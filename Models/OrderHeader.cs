using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id {  get; set; }

        [Required]
        public string Name {  get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required] 
        public string State { get; set; }

        [Required]
        public string PostalCode {  get; set; }

        [Required]
        public string Email {  get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public string Carrier { get; set; }

        public double Tracking {  get; set; }

        public DateTime ShippingDate {  get; set; }

        public string TransationId { get; set; }

        public DateTime PaymentDueDate { get; set; }                

        [Required]
        public string PaymentStatus { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public RegisterModel RegisterdUserList { get; set; }

    }
}
