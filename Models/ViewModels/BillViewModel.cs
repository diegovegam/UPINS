using UPINS.Models.Domain;

namespace UPINS.Models.ViewModels
{
    public class BillViewModel
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public int Total { get; set; }

        public int BillNumber { get; set; }

        public string User { get; set; }

        public DateTime CreationDate { get; set; }

        public int Quantity { get; set; }

        public ICollection<Product>? Products { get; set; }
        public ICollection<Bill>? Bills { get; set; }
    }
}
