using UPINS.Models.Domain;

namespace UPINS.Repositories
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAllProducts(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 10, int pageNumber = 1);

        public Task<Product> Add(Product product);

        public Task<Product> GetProduct(Guid Id);

        public Task<Product> Edit(Product product);

        public Task<Product> Delete(Guid Id);

        public Task<int> CountAsync();
        public Task<Product> AddQuantityInSCAndRemoveQuantityOverall(Product product, int quantityInShoppingCart);
        public Task<Product> RemoveQuantityInSCAndAddQuantityOverall(Product product, int quantityInShoppingCart);
        public Task<Product> EditSCIdAndQuantityInSC(Product product);
        public Task<Product> EditQuantityInSCAndTotalPriceinSC(Product product);
        

    }
}
