using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
