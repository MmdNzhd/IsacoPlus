using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
namespace KaraYadak.Components
{
    public class MenuComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MenuComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await GetItemsAsync();
            return View(nameof(MenuComponent), items);
        }
        public async Task<SiteCartVM> Cart()
        {
            ViewBag.P = @Request.Headers["Referer"].ToString();
            var categories = await _context.ProductCategories.ToListAsync();
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
            var products = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                            group a by a.ac.Code into pp
                            select new ProductWithMeterForFactorVM
                            {
                                Code = pp.FirstOrDefault().ac.Code,
                                Name = pp.FirstOrDefault().ac.Name + " " + removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())).Replace(",", ""),
                                Price = pp.FirstOrDefault().ac.Price,
                                ImageUrl = pp.FirstOrDefault().ac.ImageUrl,
                                Id = pp.FirstOrDefault().ac.Id,

                            })
                    .ToList();
            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            if (user != null)
            {
                if (user.Phone != null)
                {
                    vm.IsProfileCompelete = true;
                }
            }
            var cart = Request.Cookies["cart"];
            if (cart != null)
            {
                vm.ProductIds = cart.Split("_").Select(x => int.Parse(x.Split("-")[0])).ToList();
                vm.Lengths = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
                vm.Products = products.Where(x => vm.ProductIds.Contains(x.Id)).ToList();
                foreach (var item in vm.ProductIds)
                {
                    var l = vm.Lengths.ElementAt(vm.ProductIds.IndexOf(item));
                    vm.Products.Where(x => x.Id.Equals(item)).SingleOrDefault().Meter = l / 100;
                    double productPricePerMeter = vm.Products.Where(x => x.Id.Equals(item)).SingleOrDefault().Price;
                    vm.Price += productPricePerMeter * l / 100;
                }
                vm.SendPrice = int.Parse(sendPrice.Value);
                vm.TotalPrice = vm.Price + vm.SendPrice;
            }
            return vm;
        }

    }

}
