using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UPINS.Data;
using UPINS.Models.Domain;

namespace UPINS.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly UpinsDBContext upinsDBContext;

        public ShoppingCartRepository(UpinsDBContext upinsDBContext)
        {
            this.upinsDBContext = upinsDBContext;
        }

        public async Task<ShoppingCart> GetShoppingCart()
        {
            return await upinsDBContext.ShoppingCart.Include(p => p.Products).FirstOrDefaultAsync();
        }

        public async Task<ShoppingCart> Add(List<Product> products)
        {
            ShoppingCart existingShoppingCart = await GetShoppingCart();

            if (existingShoppingCart == null)
            {
                ShoppingCart shoppingCart = new ShoppingCart
                {
                    Products = products
                };

                await upinsDBContext.ShoppingCart.AddAsync(shoppingCart);

                await upinsDBContext.SaveChangesAsync();
            }

            else
            {
                existingShoppingCart.Products = products;
                await upinsDBContext.SaveChangesAsync();
            }
            return existingShoppingCart!;
        }

        public async Task<ShoppingCart> Delete()
        {
            var shoppingCart = await GetShoppingCart();
            upinsDBContext.Remove(shoppingCart);
            await upinsDBContext.SaveChangesAsync();

            return shoppingCart;

        }
    }
}
