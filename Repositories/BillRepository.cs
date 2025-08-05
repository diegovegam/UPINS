
using Microsoft.EntityFrameworkCore;
using UPINS.Models.Domain;
using UPINS.Data;

namespace UPINS.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly UpinsDBContext upinsDBContext;

        public BillRepository(UpinsDBContext upinsDBContext)
        {
            this.upinsDBContext = upinsDBContext;
        }
        public async Task<Bill> Add(Bill bill)
        {
            var existingBill = await GetBill(bill.Id);
            if (existingBill != null)
            {
                existingBill.Total = bill.Total;
                existingBill.Quantity = bill.Quantity;
                await upinsDBContext.SaveChangesAsync();
            }
            else
            {
                await upinsDBContext.Bill.AddAsync(bill);
                await upinsDBContext.SaveChangesAsync();
            }
                
            return bill;
        }

        public async Task<List<Bill>> GetBills(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 10, int pageNumber = 1)
        {

            var query = upinsDBContext.Bill.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(bill => bill.BillNumber.ToString().Contains(searchQuery) | bill.User.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                if (sortBy == "CreationDate")
                {
                    if (sortDirection == "Asc")
                    {
                        query = query.OrderBy(bill => bill.CreationDate);
                    }
                    else
                    {
                        query = query.OrderByDescending(bill => bill.CreationDate);
                    }
                }

            }

            var skipResults = (pageNumber - 1) * numberOfResultsPerPage;
            query = query.Skip(skipResults).Take(numberOfResultsPerPage);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await upinsDBContext.Bill.CountAsync();
        }

        public async Task<Bill> GetBill(Guid Id)
        {
            return await upinsDBContext.Bill.Include(b => b.Products).FirstOrDefaultAsync(bill => bill.Id == Id);
        }

        public async Task<Bill> DeleteBill(Guid Id)
        {
            var bill = await upinsDBContext.Bill.FindAsync(Id);
            upinsDBContext.Bill.Remove(bill);
            await upinsDBContext.SaveChangesAsync();
            return bill;
        }
    }
}
