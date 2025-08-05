
using Microsoft.EntityFrameworkCore;
using UPINS.Models.Domain;
using UPINS.Data;

namespace UPINS.Repositories
{
    public class BillProductRepository : IBillProductRepository
    {
        private readonly UpinsDBContext dbContext;

        public BillProductRepository(UpinsDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BillProduct> Add(BillProduct billProduct)
        {
            await dbContext.BillProduct.AddAsync(billProduct);
            await dbContext.SaveChangesAsync();
            return billProduct;

        }

        public async Task<List<BillProduct>> GetBillProduct(Guid Id)
        {
            return await dbContext.BillProduct.Include(bp => bp.Product).Where(bp => bp.BillId == Id).ToListAsync();
        }
    }
}
