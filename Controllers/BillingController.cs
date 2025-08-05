using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.InteropServices;
using UPINS.Data;
using UPINS.Models.Domain;
using UPINS.Models.ViewModels;
using UPINS.Repositories;

namespace UPINS.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBillRepository billRepository;
        private readonly IBillProductRepository billProductRepository;
        private readonly PdfAndEmailServiceController pdfAndEmailService;

        public BillingController(IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository,
            UserManager<IdentityUser> userManager, IBillRepository billRepository, IBillProductRepository billProductRepository,
            PdfAndEmailServiceController pdfAndEmailService)
        {
            this.productRepository = productRepository;
            this.shoppingCartRepository = shoppingCartRepository;
            this.userManager = userManager;
            this.billRepository = billRepository;
            this.billProductRepository = billProductRepository;
            this.pdfAndEmailService = pdfAndEmailService;
        }
        public IActionResult Index(List<Product> productsInCart)
        {
            productsInCart.ToArray();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShoppingCart (bool? viewbagValue, string? searchQuery, int numberOfResultsPerPage = 3, int pageNumber = 1)
        {

            ViewBag.EnoughProduct = viewbagValue == null ? true : viewbagValue;
            var totalRecords = await productRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / numberOfResultsPerPage);

            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1)
            {
                pageNumber++;
            }

            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.NumberOfResultsPerPage = numberOfResultsPerPage;
            ViewBag.PageNumber = pageNumber;


            var products = await productRepository.GetAllProducts(searchQuery, null, null, numberOfResultsPerPage, pageNumber);
            ShoppingCart shoppingCart = await shoppingCartRepository.GetShoppingCart();
            var viewModel = new AddProductRequest
              {
                 Products = products
              };

            if (shoppingCart != null)
            {
                if (shoppingCart.Products.Any())
                {
                    viewModel.ProductsInCart = shoppingCart.Products;
                }
            }
            

                return View(viewModel);

        }

        //[HttpPost]
        //public async Task<IActionResult> AddProductToCart (Guid Id, int quantityInShoppingCart)
        //{
        //    bool viewBagValue = true;
        //    var shoppingCart = await shoppingCartRepository.GetShoppingCart();

        //    var product = await productRepository.GetProduct(Id);


        //    if (product.Quantity > 0)
        //    {
            
        //        if (shoppingCart != null)
        //        {
        //            var products = shoppingCart.Products;

        //            if (products.Contains(product))
        //            {
        //                await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, quantityInShoppingCart);
        //            }
        //            else
        //            {
        //                products.Add(product);
        //                await shoppingCartRepository.Add(products);
        //                await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, quantityInShoppingCart);
        //            }
               
        //        }
        //        else
        //        {
        //            List<Product> productsInCart = new List<Product>();
        //            productsInCart.Add(product);
        //            await shoppingCartRepository.Add(productsInCart);
        //            await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, quantityInShoppingCart);
        //        }
        //    }

        //    else
        //    {
        //        viewBagValue = false;
        //    }

        //        return RedirectToAction("ShoppingCart", new { viewbagValue = viewBagValue });


        //}

        //[HttpPost]
        //public async Task<IActionResult> RemoveProductFromShoppingCart(Guid Id)
        //{
        //    var product = await productRepository.GetProduct(Id);
        //    await productRepository.EditSCIdAndQuantityInSC(product);
        //    return RedirectToAction("ShoppingCart");
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddQuantityInShoppingCartForProduct (Guid Id, int quantityInShoppingCart)
        //{
            
        //    var product = await productRepository.GetProduct(Id);
        //    await productRepository.AddQuantityInSCAndRemoveQuantityOverall(product, quantityInShoppingCart);

        //    return RedirectToAction("ShoppingCart");
        //}

        //[HttpPost]
        //public async Task<IActionResult> RemoveQuantityInShoppingCartForProduct(Guid Id, int quantityInShoppingCart)
        //{
            
        //    var product = await productRepository.GetProduct(Id);
        //    await productRepository.RemoveQuantityInSCAndAddQuantityOverall(product, quantityInShoppingCart);

        //    return RedirectToAction("ShoppingCart");
        //}


        [HttpGet]
        public async Task<IActionResult> Bill()
        {
            ShoppingCart shoppingCart = await shoppingCartRepository.GetShoppingCart();
            AddProductRequest viewModel = new AddProductRequest();
            if (shoppingCart != null)
            {
                if (shoppingCart.Products.Any())
                {
                    viewModel.ProductsInCart = shoppingCart.Products;
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateBill()
        {
            ShoppingCart shoppingCart = await shoppingCartRepository.GetShoppingCart();
            var randomNumber = new Random();
            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                BillNumber = randomNumber.Next(0, 500),
                User = userManager.GetUserName(User),
                CreationDate = DateTime.Now

            };

            int totalProducts = 0;
            int totalAmount = 0;
            foreach (var product in shoppingCart.Products)
            {
                if(product.ShoppingCartId != null)
                {
                    totalProducts++;
                }

                totalAmount += (int)product.TotalPriceInShoppingCart;
                var billProduct = new BillProduct
                {
                    BillId = bill.Id,
                    Bill = bill,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = (int)product.QuantityInShoppingCart
                
                };
                

                await billProductRepository.Add(billProduct);
                var bpList = await billProductRepository.GetBillProduct(bill.Id);
                foreach (var bp in bpList)
                {
                    bill.Products.Add(bp);
                }
            }
                

            bill.Total = totalAmount;
            bill.Quantity = totalProducts;

            await billRepository.Add(bill);
            foreach(var product in shoppingCart.Products)
            {
                await productRepository.EditQuantityInSCAndTotalPriceinSC(product);
            }

            await shoppingCartRepository.Delete();
            
            BillViewModel billViewModel = new BillViewModel
            {
                Id = bill.Id,
                BillNumber = bill.BillNumber,
                BillId = bill.Id,
                User = bill.User,
                CreationDate = bill.CreationDate,
                Quantity = bill.Quantity,
                Total = bill.Total
            };

            List<Product> products = new List<Product>();
            foreach (var p in bill.Products)
            {
                var product = await productRepository.GetProduct(p.ProductId);
                products.Add(product);
            }

            billViewModel.Products = products;
            await pdfAndEmailService.GeneratePdfAndSendEmail(billViewModel, this.ControllerContext);

            return RedirectToAction("ShoppingCart");
        }

        
    }
}
