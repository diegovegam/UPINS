using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using UPINS.Models.ViewModels;
namespace UPINS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            this.signInManager = signInManager;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {

            ViewBag.IsLoginSuccessful = true;
            var viewModel = new LoginViewModel
            {
                ReturnedUrl = returnUrl,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var signInResult = await signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, false, false);
            if (signInResult.Succeeded)
            {
                if (viewModel.ReturnedUrl != null)
                {
                    return Redirect(viewModel.ReturnedUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            else
            {
                ViewBag.IsLoginSuccessful = false;
            }

                return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
