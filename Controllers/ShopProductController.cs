using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;

namespace KaraYadak.Controllers
{

    public class ShopProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ShopProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuiopasdfghjklzxcvbnm";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static List<string> removeDuplicateForId(string[] str)
        {
            var sArray = str;
            var list = new List<string>();
            foreach (var item in sArray)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return list;
        }
        static String removeDuplicate(string str)
        {
            var sArray = str.Split(",").ToList();
            var list = new List<string>();
            foreach (var item in sArray)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return String.Join(",", list);
        }

        public IActionResult shopBasket()
        {
            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
            ViewBag.profileIsComplete = false;
            if (item != null && !string.IsNullOrWhiteSpace(item.PhoneNumber))
            {
                ViewBag.profileIsComplete = true;
            }
            if (item != null)
            {
                var vm = new ProfileVM
                {
                    Address = item.Address,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Phone = item.Phone,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    NationalCode = item.NationalCode,
                    CartNumber = item.CartNumber,
                    Gender = item.Gender,
                    ImageProfile = item.AvatarUrl
                };
                return View(vm);
            }
            return View(new ProfileVM()
            {
                Address = "",
                FirstName = "",
                LastName = "",
                Phone = "",
                PhoneNumber = "",
                Email = "",
                NationalCode = "",
                CartNumber = "",
                Gender = "",
                ImageProfile = null
            });

        }
        public async Task<JsonResult> GetProductsForBasket(string[] items)
        {


            var products = new List<ShopProductsVM>();
            var NewItems = removeDuplicateForId(items);
            foreach (var item in NewItems)
            {
                var code = item;
                try
                {
                    var product = await (from p in _context.Products.Where(x => x.Code.Equals(code))
                                         join c in _context.ProductCategories.DefaultIfEmpty() on p.CategoryIdLvl1 equals c.Id
                                         into cpTbles
                                         from cp in cpTbles.DefaultIfEmpty()
                                         join ct in _context.ProductCategoryTypes
                                         on cp.ProductCategoryType equals ct.Id
                                         into table2
                                         from t in table2.DefaultIfEmpty()
                                         where t.Id.Equals(11)
                                         select new ShopProductsVM
                                         {
                                             Id = p.Id,
                                             Title = p.Name,
                                             Brand = cp.Name ?? "بدون برند",
                                             Picture = p.ImageUrl,
                                             Price = p.Price,
                                             Code = p.Code
                                         }
                                   ).FirstOrDefaultAsync();
                    products.Add(product);

                }
                catch (Exception e)
                {

                    return new JsonResult(new { message = e.Message });
                }

            }
            return new JsonResult(new
            {
                result = products
            });
        }

        public async Task<IActionResult> Cart()
        {
            ViewBag.P = @Request.Headers["Referer"].ToString();
            var categories = await _context.ProductCategories.ToListAsync();
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);





            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            if (user != null)
            {
                if (user.PhoneNumber != null)
                {
                    vm.IsProfileCompelete = true;
                }
            }
            var cart = Request.Cookies["tempcart"];
            if (cart != null)
            {
                vm.ProductIds = cart.Split("_").Select(x => x.Split("-")[0]).ToList();
                vm.Count = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
                var products = _context.Products.Where(x => vm.ProductIds.Contains(x.Code)).ToLookup(p => p.Code, p => new ProductWithMeterForFactorVM
                {
                    Name = p.Name,
                    Code = p.Code,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Id = p.Id,

                }).ToList();
                vm.Products = new List<ProductWithMeterForFactorVM>();
                foreach (IGrouping<string, ProductWithMeterForFactorVM> item in products)
                {
                    var x = item.FirstOrDefault();
                    vm.Products.Add(item.FirstOrDefault());

                }
                foreach (var item in vm.ProductIds)
                {
                    int count = vm.Count.ElementAt(vm.ProductIds.IndexOf(item));
                    vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault().Count = count;
                    var pr = vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault();
                    vm.Price += (pr.Price * count) - ((pr.Discount * pr.Price / 100) * pr.Count);
                }
                vm.SendPrice = (sendPrice != null) ? int.Parse(sendPrice.Value) : 25000;
                vm.TotalPrice = vm.Price + vm.SendPrice;
            }
            return PartialView(vm);
        }
   
        public async Task<IActionResult> SubmitBasket()
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return new JsonResult(new { status = 0, message = "پروفایل خود را تکمیل کنید" });
            }
            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            var cart = Request.Cookies["tempcart"];
            var listOfFactorItems = new List<CartItem>();
            if (cart != null)
            {
                vm.ProductIds = cart.Split("_").Select(x => x.Split("-")[0]).ToList();
                vm.Count = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
                var products = _context.Products.Where(x => vm.ProductIds.Contains(x.Code)).ToLookup(p => p.Code, p => new ProductWithMeterForFactorVM
                {
                    Name = p.Name,
                    Code = p.Code,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Id = p.Id,

                }).ToList();
                vm.Products = new List<ProductWithMeterForFactorVM>();
                foreach (IGrouping<string, ProductWithMeterForFactorVM> item in products)
                {
                    var x = item.FirstOrDefault();
                    vm.Products.Add(item.FirstOrDefault());

                }
                foreach (var item in vm.ProductIds)
                {
                    int count = vm.Count.ElementAt(vm.ProductIds.IndexOf(item));
                    vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault().Count = count;
                    var pr = vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault();
                    var factorItem = new CartItem()
                    {
                        ProductId=pr.Id,
                        Price= ((pr.Price ) - (pr.Discount * pr.Price / 100)  ).ToString(),
                        UserName=user.UserName,
                        Date=DateTime.Now,
                        Quantity=count,
                };
                    listOfFactorItems.Add(factorItem);
                    vm.Price += (pr.Price * count) - ((pr.Discount * pr.Price / 100) * pr.Count);
                    vm.DiscountPrice += (pr.Discount * pr.Price / 100) * pr.Count;

                }
                vm.SendPrice = (sendPrice != null) ? int.Parse(sendPrice.Value) : 25000;
                vm.TotalPrice = vm.Price + vm.SendPrice;
            }
               
            else return new JsonResult(new { status = 0, message = "مشکلی در سبد خرید شما به وجود آمده است" });
            var factor = new ShoppingCart()
            {
                UserName = user.UserName,
                Date=DateTime.Now,
                Price=vm.Price.ToString(),
                Address=user.Address,
                CartItems=listOfFactorItems,
                DiscountPercent=vm.DiscountPrice.ToString(),
                Name=user.FirstName+" "+user.LastName,
                PaymentType=PaymentType.Online,
                Phone=user.Phone,
                PhoneNumber=user.PhoneNumber,
                RequestCode=RandomString(5),
                SendPrice=vm.SendPrice.ToString(),
                Status=RequestStatus.Confirmed,
            };
            await _context.CartItems.AddRangeAsync(listOfFactorItems);
            await _context.ShoppingCarts.AddRangeAsync(factor);
            await _context.SaveChangesAsync();
            return new JsonResult(new { status = 1, message = "با موفقیت انجام شد" });
        }
    }
}
