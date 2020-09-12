using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTPersianUtils.Core.IranCities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace KaraYadak.Controllers
{

    public class SiteAccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private static Random random = new Random();
        private readonly IWebHostEnvironment _env;
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/UserImg/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }

        public SiteAccountController(IWebHostEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public IActionResult Register(string call, int id)
        {
            if (call != null)
                ViewBag.Call = call;
            else if (Request.Cookies["cart"] != null) ViewBag.Call = "/ShopProduct/ShopBasket";
            else
                ViewBag.Call = "/";
            return View();
        }
        [Authorize]
        public IActionResult EditProfile(string call)
        {
            ViewBag.Transaction = _context.ShoppingCarts.Where(x => x.UserName.Equals(User.Identity.Name)).ToList();

            ViewBag.Provinces = Iran.Provinces.OrderBy(i => i.ProvinceName);
            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
            var vm = new ProfileVM
            {
                Address = item.Address,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Phone = item.Phone,
                PhoneNumber = item.PhoneNumber,
                CallbackUrl = call,
                Email = item.Email,
                NationalCode = item.NationalCode,
                CartNumber = item.CartNumber,
                Gender = item.Gender,
                ImageProfile = item.AvatarUrl
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SaveProfile(ProfileVM input,IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            user.Address = input.Address;
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.NationalCode = input.NationalCode;
            user.PhoneNumber = input.PhoneNumber;
            user.Phone = input.Phone;
            user.Email = input.Email;
            user.Gender = input.Gender;
            user.AvatarUrl = (file!=null)? upload(file):""; 
            user.CartNumber = input.CartNumber;
            await _context.SaveChangesAsync();
            if (input.CallbackUrl == null)
            {
                return new JsonResult(new { Status = 1, result = Url.Action("index", "HomeSite") });
            }
            else
            {
                return new JsonResult(new { Status = 1, result = input.CallbackUrl });

            }
        }
        [Authorize]
        public IActionResult Purchase()
        {
            ViewBag.P = Request.Headers["Referer"].ToString();
            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
            var vm = new RequestFormVM
            {
                Address = item.Address,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Phone = item.Phone,
                PhoneNumber = item.PhoneNumber,
                Email = item.Email,
            };
            return View(vm);
        }
        [HttpGet]
        [Route("LogOut")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            returnUrl = returnUrl ?? Url.Content("/");
            return LocalRedirect(returnUrl);
        }

    }
}
