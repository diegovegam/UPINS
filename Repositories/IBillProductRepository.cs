using UPINS.Models.Domain;

namespace UPINS.Repositories
{
    public interface IBillProductRepository
    {
        public Task<BillProduct> Add(BillProduct billProduct);

        public Task<List<BillProduct>> GetBillProduct(Guid Id);
    }
}
