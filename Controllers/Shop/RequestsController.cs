using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DNTPersianUtils.Core;
using KaraYadak.ViewModels;
using System.Text;

namespace Zarpoosh.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        static string Meter_CentiMeter(string s)
        {
            var list = s.ToList();
            var newS = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 2)
                {
                    newS.Add(".");
                }
                newS.Add(list[i].ToString());
            }
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            var j = 0;
            foreach (string value in newS.ToArray())
            {
                j++;
                builder.Append(value);
                if (j != newS.ToArray().Length)
                {
                    builder.Append(' ');
                }
            }
            return builder.ToString().Replace(" ", "");
        }
        public RequestsController(ApplicationDbContext context, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var carts = await _context.ShoppingCarts.Include(c => c.CartItems).OrderByDescending(i => i.Date).ToListAsync();
            return View(carts);
        }
        public IActionResult Reject(int id)
        {
            if (id == 0)
                return NotFound();
            var request = _context.ShoppingCarts.SingleOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound();
            request.Status = RequestStatus.Rejected;
            _context.SaveChanges();
            return Json(new { status = "1", message = "با موفقیت انجام شد" });
        }
        public IActionResult Confirm(int id)
        {
            if (id == 0)
                return NotFound();
            var request = _context.ShoppingCarts.SingleOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound();
            request.Status = RequestStatus.Confirmed;
            _context.SaveChanges();
            return Json(new { status = "1", message = "با موفقیت انجام شد" , item = request.Id});
        }
        public IActionResult Deliver(int id)
        {
            if (id == 0)
                return NotFound();
            var request = _context.ShoppingCarts.SingleOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound();
            request.Status = RequestStatus.Finished;
            _context.SaveChanges();
            return Json(new { status = "1", message = "با موفقیت انجام شد" });
        }
        public async Task<IActionResult> Cart(int id)
        {
            if (id == 0)
                return NotFound();
            var carts = await _context.ShoppingCarts.Include(c => c.CartItems).OrderByDescending(i => i.Date).ToListAsync();
            var cart = carts.SingleOrDefault(x => x.Id == id);
            if (cart == null)
                return NotFound();

            var products = await _context.Products.ToListAsync();
            var list = new List<CartItemVM>();
            foreach (var item in cart.CartItems)
            {
                var vm = new CartItemVM
                {
                    Product = products.FirstOrDefault(x => x.Code == item.ProductId.ToString()).Name,
                    Price = string.Format("{0:n0}", int.Parse(item.Price)).ToPersianNumbers(),
                    Quantity = item.Quantity.ToString().ToPersianNumbers()
                };
                list.Add(vm);
            }
            return Json(new { cart = cart, date = cart.Date.ToPersianDateTextify(), payment = "آنلاین", status = cart.Status.ToString(), price = string.Format("{0:n0}", int.Parse(cart.Price)).ToPersianNumbers(), items = list });
        }
        public async Task<IActionResult> Filter(RequestStatus status, PaymentType paymentType, string priceRange, DateTime? date1, DateTime? date2)
        {
            var carts = await _context.ShoppingCarts.Include(c => c.CartItems).OrderByDescending(i => i.Date).ToListAsync();
            var price1 = int.Parse(priceRange.Split("_")[0]);
            var price2 = int.Parse(priceRange.Split("_")[1]);
            var items = carts.Where(x => x.Status == status && x.PaymentType == paymentType && int.Parse(x.Price) >= price1 && int.Parse(x.Price) <= price2).ToList();
            var list = new List<ShoppingCartVM>();
            if (date1 != null && date2 != null)
            {
                items = items.Where(x => x.Date > date1 && x.Date < date2).ToList();
            }
            foreach (var item in items)
            {
                var vm = new ShoppingCartVM
                {
                    Id = item.Id,
                    RequestCode = item.RequestCode,
                    UserName = item.UserName,
                    Address = item.Address.ToPersianNumbers(),
                    Price = string.Format("{0:n0}", int.Parse(item.Price)).ToPersianNumbers(),
                    Date = item.Date.ToPersianDateTextify(),
                    PaymentType ="آنلاین",
                    Status = item.Status.ToString(),
                    Payment = item.PaymentType,
                    RequestStatus = item.Status,
                };
                list.Add(vm);
            }
            if (list.Count == 0)
                return Json(new { status = '0', message = "رکوردی پیدا نشد" });

            return Json(new { status = '1', items = list.OrderByDescending(i => i.Date) });
        }
        public async Task<IActionResult> ResetList()
        {
            var carts = await _context.ShoppingCarts.Include(c => c.CartItems).OrderByDescending(i => i.Date).ToListAsync();
            var list = new List<ShoppingCartVM>();
            foreach (var item in carts)
            {
                var vm = new ShoppingCartVM
                {
                    Id = item.Id,
                    RequestCode = item.RequestCode,
                    UserName = item.UserName,
                    Address = item.Address,
                    Price = string.Format("{0:n0}", int.Parse(item.Price)).ToPersianNumbers(),
                    Date = item.Date.ToPersianDateTextify(),
                    PaymentType = "آنلاین",
                    Status = item.Status.ToString(),
                    Payment = item.PaymentType,
                    RequestStatus = item.Status,
                };
                list.Add(vm);
            }
            return Json(new { status = '1', items = list });
        }
    }
}