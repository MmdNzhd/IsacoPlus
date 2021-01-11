using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using KaraYadak.Data;
using KaraYadak.Helper;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]
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
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {

            //data for chart --order count
            var _orderForChart = await (from c in _context.ShoppingCarts
                                        group c by c.Date into g
                                        select new
                                        {
                                            Date = g.Key,
                                            PersianDate = g.Key.ToShamsi(),
                                            Count = g.Count()
                                        }).ToListAsync();

            var _orderForChartWithPerianDate = (from o in _orderForChart
                                                group o by o.PersianDate into g
                                                select new
                                                {
                                                    date = g.Key,
                                                    count = g.Count()
                                                }).ToList();

            ViewBag.orderCountForChart = Json(new
            {
                date = _orderForChartWithPerianDate.Select(x => x.date).ToList(),
                count = _orderForChartWithPerianDate.Select(x => x.count).ToList()
            });


            var sellCounts = await _context.CartItems.SumAsync(x => x.Quantity);
            var orders = await _context.ShoppingCarts.Where(x => x.Status.Equals(RequestStatus.Confirmed)).ToListAsync();
            var orderCounts = orders.Count;
            var product = await _context.Products.Where(x => x.ProductStatus.Equals(ProductStatus.آماده_برای_فروش)).ToListAsync();
            var productCount = product.Count;
            var user = await _context.Users.ToListAsync();
            var userCount = user.Count;


            //TopSellProduct
            var topSellProductIds = await (from a in _context.CartItems
                                           group a by a.ProductId into g
                                           select new
                                           {
                                               ProductId = g.Key,
                                               Count = g.Sum(x => x.Quantity)
                                           }).OrderByDescending(x => x.Count).Take(10).ToListAsync();
            var topSellProducts = new List<ProductForIndexVM>();
            foreach (var item in topSellProductIds)
            {
                var pr = await _context.Products.FindAsync(item.ProductId);
                var prtopsell = new ProductForIndexVM()
                {
                    Code = pr?.Code,
                    Title = pr?.Name,
                    Count = item.Count
                };
                topSellProducts.Add(prtopsell);
            }


            //Last Order
            var lastOrders = await _context.ShoppingCarts.OrderByDescending(x => x.Date).Take(5).ToListAsync();

            foreach (var item in lastOrders)
            {
                var userForThisOrde = _userManager.FindByNameAsync(item.UserName).Result;
                if (userForThisOrde != null)
                    item.UserName = userForThisOrde.FirstName + " " + userForThisOrde.LastName;
            }

            var finalModel = new DashboardVM()
            {
                CustomerCounts = userCount,
                ProductCounts = productCount,
                OrderCounts = orderCounts,
                SellerCounts = sellCounts,
                TopSellProducts = topSellProducts,
                LastOrders = lastOrders
            };

            return View(finalModel);
        }
    }
}