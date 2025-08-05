using Microsoft.AspNetCore.Identity;

namespace UPINS.Models.Domain
{
    public class Bill
    {
        public Guid Id { get; set; }

        public int Total { get; set; }

        public int BillNumber { get; set; }

        public string User { get; set; }

        public DateTime CreationDate { get; set; }

        public int Quantity { get; set; }

        public ICollection<BillProduct> Products { get; set; }

        //I must try to add the join table here with the extra property called QuantityBilled for each product. 
    }
}
