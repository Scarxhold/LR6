namespace LR6.Models
{
    public class Order
    {
        public int Quantity { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
