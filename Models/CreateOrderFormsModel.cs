namespace LR6.Models
{
    public class CreateOrderFormsModel
    {
        public int Quantity { get; set; }
        public List<Product> AvailableProducts { get; set; }
    }
}
