using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPINS.Models.Domain;
using UPINS.Models.ViewModels;
using UPINS.Repositories;

namespace UPINS.Controllers
{
    [Authorize]
    public class AdminProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        public AdminProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 2, int pageNumber = 1)
        {

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
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NumberOfResultsPerPage = numberOfResultsPerPage;
            ViewBag.PageNumber = pageNumber;
            var products = await productRepository.GetAllProducts(searchQuery, sortBy, sortDirection, numberOfResultsPerPage, pageNumber);
            var viewModel = new AddProductRequest
            {
                Products = products
            };
            return View(viewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> Add(AddProductRequest productViewModel)
        {
            var product = new Product
            {
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Quantity = productViewModel.Quantity,
                code = productViewModel.Code,
                ImageUrl = productViewModel.ImageUrl
            };

            await productRepository.Add(product);

            return RedirectToAction("List");
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id/*, bool? Name = false, bool? Price = false, bool? Quantity = false, bool? Code = false*/)
        {
            var product = await productRepository.GetProduct(Id);
            ViewBag.Name = false;
            ViewBag.Price = false;
            ViewBag.Quantity = false;
            ViewBag.Code = false;

            var viewModel = new AddProductRequest
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                Code = product.code,
                ImageUrl = product.ImageUrl
            };

            return View(viewModel);

        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(AddProductRequest viewModel)
        {
            var currentProduct = await productRepository.GetProduct(viewModel.Id);


            ViewBag.Name = currentProduct.Name == viewModel.Name ? false : true;
            ViewBag.Price = currentProduct.Price == viewModel.Price ? false : true;
            ViewBag.Quantity = currentProduct.Quantity == viewModel.Quantity ? false : true;
            ViewBag.Code = currentProduct.code == viewModel.Code ? false : true;


            var product = new Product
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Price = viewModel.Price,
                Quantity = viewModel.Quantity,
                code = viewModel.Code,
                ImageUrl = viewModel.ImageUrl
            };

            await productRepository.Edit(product);

            var viewModelUpdated = new AddProductRequest()
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Price = viewModel.Price,
                Quantity = viewModel.Quantity,
                Code = viewModel.Code,
                ImageUrl = viewModel.ImageUrl
            };

            return View(viewModelUpdated);
        }

        [HttpPost]
        public async Task<IActionResult> Delete (Guid Id)
        {
            await productRepository.Delete(Id);
            return RedirectToAction("List");  
        }
    }
}
