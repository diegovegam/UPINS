using UPINS.Models.Domain;

namespace UPINS.Repositories
{
    public interface IBillRepository
    {
        public Task<Bill> Add(Bill bill);

        public Task<List<Bill>> GetBills(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 10, int pageNumber = 1);

        public Task<int> CountAsync();

        public Task<Bill> GetBill(Guid Id);

        public Task<Bill> DeleteBill(Guid Id);
    }
}
