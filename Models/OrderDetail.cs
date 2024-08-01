using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }        
        public int? OrderId {  get; set; }

        [ForeignKey("OrderId")]
        public OrderHeader Orderlist {  get; set; }        
        public int? ProductId {  get; set; }

        [ForeignKey("ProductId")]
        public Product Productlist { get; set; }

        [Required]
        public double Quantity {  get; set; }                
    }
}
