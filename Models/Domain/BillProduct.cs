namespace UPINS.Models.Domain
{
    public class BillProduct
    {
        public Guid BillId { get; set; }
        public Bill? Bill { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; } 
    }
}
