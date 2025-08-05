namespace UPINS.Models.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public int code { get; set; }

        public string ImageUrl { get; set; }

        public int? QuantityInShoppingCart { get; set; }
        public int? TotalPriceInShoppingCart { get; set; }

        public Guid? ShoppingCartId { get; set; }

        public ICollection<BillProduct> Bills { get; set; }

    }
}
