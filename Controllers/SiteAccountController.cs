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
using System.Net.WebSockets;
using DNTPersianUtils.Core;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin,User")]

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
        [AllowAnonymous]
        public IActionResult Register(string call, int id)
        {
            if (call != null)
                ViewBag.Call = call;
            else if (Request.Cookies["cart"] != null) ViewBag.Call = "/ShopProduct/ShopBasket";
            else
                ViewBag.Call = "/";
            return View();
        }
        [Route("GetCities")]
        public IActionResult GetCities(string province)
        {
            try
            {
                var cities = Iran.Cities.Where(x => x.ProvinceName.Equals(province)).Select(x => x.CityName).ToList();
                return new JsonResult(new { status=1,data= cities, error=new List<string>() });

            }
            catch (Exception ex)
            {
                var err = new List<string>();
                err.Add("خطایی رخ داده است");
                return new JsonResult(new { status = 1, data = "", error = err });


            }

        }
        public async Task<IActionResult> EditProfile(string call)
        {
            ViewBag.Cities = Iran.Cities.Select(x => x.CityName).ToList();
            ViewBag.Provinces = Iran.Cities.Select(x => x.ProvinceName).ToList();
            ViewBag.Transaction =await _context.ShoppingCarts.Where(x => x.UserName.Equals(User.Identity.Name)).OrderByDescending(x=>x.Id).ToListAsync();

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
                ImageProfile = item.AvatarUrl,
                City=item.City,
                PostalCode=item.PostalCode,
                Province=item.Province
            };
            //DateTime.Now.ToShortPersianDateString()
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SaveProfile(ProfileVM input, IFormFile file)
        {
            try
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
                    return new JsonResult(new { Status = 0, Message = errors });
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
                user.AvatarUrl = (file != null) ? upload(file) : "";
                user.CartNumber = input.CartNumber;
                user.City = input.City;
                user.Province = input.Province;
                user.PostalCode = input.PostalCode;
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
            catch (Exception ex)
            {

                return new JsonResult(new { Status = 0, message="خطایی رخ داده است" });

            }
        }
        [HttpGet]
        [Route("LogOut")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            returnUrl = returnUrl ?? Url.Content("/");
            return RedirectToAction("Index", "HomeSite");
        }

    }
}
