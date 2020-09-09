using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var findUser = await _userManager.FindByNameAsync(User.Identity.Name);

            return Json(new { Status = 1, Message = User.Identity.Name });
            //return View(new EditProfileViewModel
            //{
            //    FirstName = findUser.FirstName ?? "",
            //    LastName = findUser.LastName ?? "",
            //    Email = findUser.Email ?? "",
            //    Phone = findUser.Phone ?? "",
            //    Username = findUser.UserName ?? ""
            //});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel input)
        {
            var findUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                findUser.FirstName = input.FirstName;
                findUser.LastName = input.LastName;
                findUser.Email = input.Email;
                findUser.Phone = input.Phone;
                findUser.UserName = input.Username;
                if (input.Password != null)
                    findUser.PasswordHash = _userManager.PasswordHasher.HashPassword(findUser, input.Password);
                var res = await _userManager.UpdateAsync(findUser);
                if (res.Succeeded)
                {
                    return Json(new { Status = 1, Message = "" });
                }
                else
                {
                    return Json(new { Status = 0, Message = "لطفا دوباره تلاش کنید" });
                }
            }
            var errors = new List<string>();
            foreach (var item in ModelState.Values)
            {
                foreach (var err in item.Errors)
                {
                    errors.Add(err.ErrorMessage);
                }
            }
            return Json(new { Status = 0, Message = errors.First() });
        }

    }
}