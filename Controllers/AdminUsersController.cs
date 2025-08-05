using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using UPINS.Models.ViewModels;
using UPINS.Repositories;

namespace UPINS.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository usersRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUsersController(IUserRepository usersRepository, UserManager<IdentityUser> userManager)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await usersRepository.GetUsers();
            var viewModel = new UsersViewModel();
            viewModel.Users = new List<UserViewModel>();

            foreach (var user in users)
            {
                viewModel.Users.Add(new UserViewModel
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName,
                    Email = user.Email

                });
            }
            return View(viewModel);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(UsersViewModel viewModel)
        {
            var identityUser = new IdentityUser
            {
                UserName = viewModel.Username,
                Email = viewModel.Email,
            };

            identityUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(identityUser, viewModel.Password);
            var identityResult = await userManager.CreateAsync(identityUser);

            if (identityResult.Succeeded)
            {
                var roles = new List<string> { "User" };
                if(viewModel.AdminRole)
                {
                    roles.Add("Admin");
                }

                var roleAssignmentResult = await userManager.AddToRolesAsync(identityUser, roles);

                if (roleAssignmentResult is not null && roleAssignmentResult.Succeeded)
                {
                    return RedirectToAction("List");
                }
            }

            return View(viewModel);

        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());
            if (user is not null)
            {
                var identityResult = await userManager.DeleteAsync(user);
                if (identityResult is not null && identityResult.Succeeded)
                {
                    return RedirectToAction("List");
                }
            }
            return RedirectToAction("List");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());

            ViewBag.Username = false;
            ViewBag.Email = false;
            ViewBag.Password = false;
            ViewBag.AdminRole = false;

            var viewModel = new UserViewModel
            {
                Id = Guid.Parse(user.Id),
                Username = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash
            };

            IList<IdentityUser> usersInRole = await userManager.GetUsersInRoleAsync("admin");

            foreach (var userInRole in usersInRole)
            {
                if (userInRole.Equals(user))
                {
                    viewModel.AdminRole = true;
                }
            }
            
            return View(viewModel);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel viewModel)
        {

            string adminRole = "admin";
            var user = await userManager.FindByIdAsync(viewModel.Id.ToString());

            IList<IdentityUser> usersInRole = await userManager.GetUsersInRoleAsync(adminRole);

            ViewBag.Username = user.UserName == viewModel.Username ? false : true;
            ViewBag.Email = user.Email == viewModel.Email ? false : true;
            ViewBag.Password = viewModel.Password == null ? false : true;
            ViewBag.AdminRole = viewModel.AdminRole == false ? false : true;


            user.UserName = viewModel.Username;
            user.Email = viewModel.Email;
            if(viewModel.Password != null)
            {
                user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, viewModel.Password);
            }
            

            await userManager.UpdateAsync(user);

            

            if (viewModel.AdminRole)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, adminRole);
            }

            return View(viewModel);

        }


    }
}
