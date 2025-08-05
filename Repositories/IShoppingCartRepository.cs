using UPINS.Models.Domain;
namespace UPINS.Repositories
{
    public interface IShoppingCartRepository
    {
       public Task<ShoppingCart> GetShoppingCart();

       public Task<ShoppingCart> Add(List<Product> products);
       public Task<ShoppingCart> Delete();
    }
}
