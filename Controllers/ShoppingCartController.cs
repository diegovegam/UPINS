using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UPINS.Models.Domain;
using UPINS.Models.ViewModels;
using UPINS.Repositories;

namespace UPINS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IProductRepository productRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
        }

        [HttpPost]
        [Route("AddProductToCart")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddProductSCViewModel addProductSCViewModel)
        {
            bool viewBagValue = true;
            var shoppingCart = await shoppingCartRepository.GetShoppingCart();

            var product = await productRepository.GetProduct(addProductSCViewModel.ProductId);


            if (product.Quantity > 0)
            {

                if (shoppingCart != null)
                {
                    var products = shoppingCart.Products;

                    if (products.Contains(product))
                    {
                        await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, addProductSCViewModel.QuantityInSC);
                    }
                    else
                    {
                        products.Add(product);
                        await shoppingCartRepository.Add(products);
                        await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, addProductSCViewModel.QuantityInSC);
                    }

                }
                else
                {
                    List<Product> productsInCart = new List<Product>();
                    productsInCart.Add(product);
                    await shoppingCartRepository.Add(productsInCart);
                    await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, addProductSCViewModel.QuantityInSC);
                }
            }

            else
            {
                viewBagValue = false;
            }

            return Ok(viewBagValue);

        }


        [HttpGet]
        [Route("GetProductsFromSC")]
        public async Task<IActionResult> GetProductsFromSC()
        {
            ShoppingCart shoppingCart = await shoppingCartRepository.GetShoppingCart();
            var viewModel = new AddProductRequest
            {
                ProductsInCart = shoppingCart.Products
            };

            var jsonFile = JsonSerializer.Serialize(viewModel.ProductsInCart);

            return Ok(jsonFile);
        }

        [HttpPost]
        [Route("{productId:Guid}/RemoveProductFromSC")]
        public async Task<IActionResult> RemoveProductFromShoppingCart(Guid productId)
        {
            var product = await productRepository.GetProduct(productId);
            await productRepository.EditSCIdAndQuantityInSC(product);
            return Ok();
        }

        [HttpPost]
        [Route("{productId:Guid}/AddQuantityInSC")]
        public async Task<IActionResult> AddQuantityInShoppingCartForProduct(Guid productId)
        {

            var product = await productRepository.GetProduct(productId);
            await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, 1);

            return Ok();
        }

        [HttpPost]
        [Route("{productId:Guid}/RemoveQuantityInSC")]
        public async Task<IActionResult> RemoveQuantityInShoppingCartForProduct(Guid productId)
        {

            var product = await productRepository.GetProduct(productId);
            await productRepository.RemoveQuantityInSCAndAddQuantityOverall(product, 1);

            return RedirectToAction("ShoppingCart");
        }
    }
}
