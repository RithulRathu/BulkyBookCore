using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Count {  get; set; }        

        public int? ProductId {  get; set; }

        [ForeignKey("ProductId")]
        public Product ProductItem { get; set; }

        public int? UsersId { get; set; }

        [ForeignKey("UsersId")]
        public RegisterModel RegisteredUserslist { get; set; }

        [NotMapped]
        public double TotalPrice {  get; set; }
    }
}
