using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;

namespace KaraYadak.Controllers.WebApi
{
    [Produces("application/json")]
    [Route("webapi/{action}")]
    [ApiController]
    public class WebApiController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public WebApiController(SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
        }
        [NonAction]
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
        [HttpPost]
        public IActionResult GetProductInfo([FromForm] string QrCode)
        {

            var groupByCodeProduct = (from a in _context.Products.Where(x => x.QR != null && x.ProductStatus == ProductStatus.آماده_برای_فروش)
                                                  .Join(_context.ProductCategories,
                                                      ac => ac.CategoryIdLvl1,
                                                      cc => cc.Id,
                                                      (ac, cc) => new
                                                      {
                                                          ac,
                                                          cc
                                                      })
                                                  .ToList()
                                      group a by a.ac.Code into pp
                                      select new ProductWithCategoryVM
                                      {
                                          Product = pp.FirstOrDefault().ac,
                                          Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
                                      })
                                    .ToList();




            var products = new List<List<ProductsDetail>>();
            var product = groupByCodeProduct.Where(x => x.Product.QR.Equals(QrCode)).Select(x => new ProductsDetail
            {
                Id = x.Product.Id,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Describe = x.Product.Description,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
                Rate = x.Product.Rate.GetValueOrDefault(),
                Color = x.Categories?.Replace(",", ""),

            }).OrderByDescending(x => x.CreatingDate).FirstOrDefault();
            if (product == null)
            {
                return new JsonResult(new { status = "0", result = "  محصولی یافت نشده است" });
            }

            return new JsonResult(new { status = "1", result = product });
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToBasket([FromForm] SalerAddProductVM addProduct) {

            var groupByCodeProduct = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
                                                  .Join(_context.ProductCategories,
                                                      ac => ac.CategoryIdLvl1,
                                                      cc => cc.Id,
                                                      (ac, cc) => new
                                                      {
                                                          ac,
                                                          cc
                                                      })
                                                  .ToList()
                                      group a by a.ac.Code into pp
                                      select new ProductWithCategoryVM
                                      {
                                          Product = pp.FirstOrDefault().ac,
                                          Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
                                      })
                                    .ToList();




            var product =await _context.Products.Where(x => x.Id.Equals(addProduct.ProductId)).FirstOrDefaultAsync();
            if (product == null)
            {
                return new JsonResult(new { status = "0", result = "  محصولی یافت نشده است" });
            }
            var cartitemwithThisQr =await _context.CartItems.Where(x => x.QR.Equals(addProduct.QrCode)).FirstOrDefaultAsync();
            if (cartitemwithThisQr != null)
            {
                return new JsonResult(new { status = "0", result = "اطلاعات قبلا اضافه شده است" });
            }
            try
            {

                var cartItem = new CartItem()
                {
                    ProductId = product.Id,
                    Date = DateTime.Now,
                    Price = product.Price.ToString(),
                    QR = addProduct.QrCode,
                    Quantity = addProduct.Meter,
                    UserName = ""
                };

               await _context.CartItems.AddAsync(cartItem);
               await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", result = "با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { status = "0", result = "خظایی رخ داده است" +ex.Message});
            }

        }

    }
}
