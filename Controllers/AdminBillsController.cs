using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using UPINS.Models.Domain;
using UPINS.Models.ViewModels;
using UPINS.Repositories;

namespace UPINS.Controllers
{
    [Authorize]
    public class AdminBillsController : Controller
    {
        private readonly IBillRepository billRepository;
        private readonly IProductRepository productRepository;
        private readonly PdfAndEmailServiceController pdfAndEmailService;

        public AdminBillsController(IBillRepository billRepository, IProductRepository productRepository , PdfAndEmailServiceController pdfAndEmailService)
        {
            this.billRepository = billRepository;
            this.productRepository = productRepository;
            this.pdfAndEmailService = pdfAndEmailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBills(string? searchQuery, string? sortBy, string? sortDirection, int numberOfResultsPerPage = 3, int pageNumber = 1)
        {
            var totalRecords = await billRepository.CountAsync();
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


            var bills = await billRepository.GetBills(searchQuery, sortBy, sortDirection, numberOfResultsPerPage, pageNumber);

            var viewModel = new BillViewModel
            {
                Bills = bills
            };


            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> ViewBill (Guid Id)
        {
            var bill = await billRepository.GetBill(Id);

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

            return View(billViewModel);
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> DeleteBill (Guid Id)
        {
            await billRepository.DeleteBill(Id);
            return RedirectToAction("GetBills");
        }

        [HttpPost]
        public async Task<IActionResult> SendBillViaEmail(Guid Id)
        {
            var bill = await billRepository.GetBill(Id);

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
            

            return RedirectToAction("GetBills");
        }
       
    }
}
