namespace UPINS.Models.Domain
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }

        public List<Product>? Products { get; set; }


    }
}
