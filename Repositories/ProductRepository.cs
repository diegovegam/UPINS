using Microsoft.EntityFrameworkCore;
using UPINS.Data;
using UPINS.Models.Domain;

namespace UPINS.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly UpinsDBContext upinsDbContext;

        public ProductRepository(UpinsDBContext upinsDbContext)
        {
            this.upinsDbContext = upinsDbContext;
        }

        public async Task<Product> Add(Product product)
        {
            await upinsDbContext.Product.AddAsync(product);
            await upinsDbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> GetProduct(Guid Id)
        {
            return await upinsDbContext.Product.Include(bp => bp.Bills).FirstOrDefaultAsync(product => product.Id == Id);
        }

        public async Task<List<Product>> GetAllProducts(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 10, int pageNumber = 1)
        {
  
            var query = upinsDbContext.Product.AsQueryable();
            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(product => product.Name.Contains(searchQuery) | product.code.ToString().Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && !string.IsNullOrWhiteSpace(sortDirection))
            {
                if(sortBy == "Name")
                {
                    if(sortDirection == "Asc")
                    {
                        query = query.OrderBy(product => product.Name); 
                    }
                    else 
                    {
                        query = query.OrderByDescending(product => product.Name);
                    }  
                }

                if (sortBy == "Price")
                {
                    if (sortDirection == "Asc")
                    {
                        query = query.OrderBy(product => product.Price);
                    }
                    else
                    {
                        query = query.OrderByDescending(product => product.Price);
                    }
                }

                if (sortBy == "Quantity")
                {
                    if (sortDirection == "Asc")
                    {
                        query = query.OrderBy(product => product.Quantity);
                    }
                    else
                    {
                        query = query.OrderByDescending(product => product.Quantity);
                    }
                }

            }

            var skipResults = (pageNumber - 1) * numberOfResultsPerPage;
            query = query.Skip(skipResults).Take(numberOfResultsPerPage);

            return await query.ToListAsync();
        }

        public async Task<Product> Edit(Product viewModel)
        {
            var product = await upinsDbContext.Product.FirstOrDefaultAsync(Product => Product.Id == viewModel.Id);
           
            product.Name = viewModel.Name;
            product.Price = viewModel.Price;
            product.Quantity = viewModel.Quantity;
            product.code = viewModel.code;
            product.ImageUrl = viewModel.ImageUrl;

            await upinsDbContext.SaveChangesAsync();

            return product;
            
        }

        public async Task<Product> AddQuantityInSCAndRemoveQuantityOverall(Product product, int quantityInShoppingCart)
        {
            if (product.QuantityInShoppingCart is not null)
            {
                product.QuantityInShoppingCart += quantityInShoppingCart;
                product.Quantity -= quantityInShoppingCart;
                product.TotalPriceInShoppingCart = (product.Price * product.QuantityInShoppingCart);
            }
            else
            {
                product.QuantityInShoppingCart = quantityInShoppingCart;
                product.Quantity -= quantityInShoppingCart;
                product.TotalPriceInShoppingCart = (product.Price * product.QuantityInShoppingCart);
            }
                

            await upinsDbContext.SaveChangesAsync();

            return product;

        }

        public async Task<Product> RemoveQuantityInSCAndAddQuantityOverall(Product product, int quantityInShoppingCart)
        {

            product.QuantityInShoppingCart -= quantityInShoppingCart;
            if (product.QuantityInShoppingCart < 0 )
            {
                product.QuantityInShoppingCart = 0;
            }
            if (product.QuantityInShoppingCart != 0)
            {
                product.Quantity += quantityInShoppingCart;
            }
            product.TotalPriceInShoppingCart = (product.Price * product.QuantityInShoppingCart);
            
            await upinsDbContext.SaveChangesAsync();

            return product;

        }

        public async Task<Product> EditSCIdAndQuantityInSC(Product product)
        {
            product.ShoppingCartId =  null;
            product.QuantityInShoppingCart = 0;
            product.TotalPriceInShoppingCart = 0;
            await upinsDbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> Delete(Guid Id)
        {
            var existingProduct = await upinsDbContext.Product.FindAsync(Id);
            upinsDbContext.Product.Remove(existingProduct);
            await upinsDbContext.SaveChangesAsync();
            return existingProduct;

        }

        public async Task<int> CountAsync()
        {
            return await upinsDbContext.Product.CountAsync();
        }

        public async Task<Product> EditQuantityInSCAndTotalPriceinSC(Product product)
        {
            product.QuantityInShoppingCart = 0;
            product.TotalPriceInShoppingCart = 0;
            await upinsDbContext.SaveChangesAsync();

            return product;
        }

    }
    
}
