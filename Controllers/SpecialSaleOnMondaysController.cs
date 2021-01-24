using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using KaraYadak.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]

    public class SpecialSaleOnMondaysController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public SpecialSaleOnMondaysController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
        public async Task<IActionResult> DeleteImage(string imageurl)
        {
            _context.Images.Remove(_context.Images.FirstOrDefault(i => i.Url == imageurl));
            await _context.SaveChangesAsync();
            var fullPath = _env.WebRootPath + Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", imageurl);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            return Content("ok");
        }
        [HttpPost]
        public async Task<IActionResult> GetProduct()
        {


            var items = (from a in _context.Products.Where(x => x.MondaySpecialSale)
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
                         select new
                         {
                             Product = pp.FirstOrDefault().ac,
                             Categories = String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray()),
                         })
                   .ToList();
            var products = items.Select(x => new SpecialMondayProduct
            {
                CategoryIdLvl1 = x.Product.CategoryIdLvl1,
                CategoryIdLvl2 = x.Product.CategoryIdLvl2,
                CategoryIdLvl3 = x.Product.CategoryIdLvl3,
                Id = x.Product.Id,
                Name = x.Product.Name,
                Categories = x.Categories,
                Code = x.Product.Code,
                Discount=x.Product.Discount
            }).ToList();
            return Json(new { data = products });

        }
        public async Task<IActionResult> Index()
        {


            var groupByCodeProduct = _context.Products.Where(x=>!x.MondaySpecialSale).ToLookup(p => p.Code, p => new ProductList
            {
                Code = p.Code,
                Name = p.Name

            }).ToList();
            ViewBag.ProductList = groupByCodeProduct.Select(x => new ProductList
            {
                Code = x.Key.Trim(),
                Name = x.FirstOrDefault().Name
            }).ToList();

            var settings = await _context.Settings.ToListAsync();
            var specialMondayInfo = new SpecialMondayInfo();
            specialMondayInfo.SpecialMondayBanner = new SpecialMondayBanner()
            {
                LeftPic = settings.Where(x => x.Key == "MondayLeftBaner").FirstOrDefault().Value,
                RightPic = settings.Where(x => x.Key == "MondayRightBaner").FirstOrDefault().Value,
                MondayVerticalBaner1 = settings.Where(x => x.Key == "MondayVerticalBaner1").FirstOrDefault().Value,
                MondayVerticalBaner2 = settings.Where(x => x.Key == "MondayVerticalBaner2").FirstOrDefault().Value,
                MondayVerticalBaner3 = settings.Where(x => x.Key == "MondayVerticalBaner3").FirstOrDefault().Value,
                Title = settings.Where(x => x.Key == "MondayTitle").FirstOrDefault().Value,
                TopSliderPic1 = settings.Where(x => x.Key == "MondayTopBaner1").FirstOrDefault().Value,
                TopSliderPic2 = settings.Where(x => x.Key == "MondayTopBaner2").FirstOrDefault().Value,
                TopSliderPic3 = settings.Where(x => x.Key == "MondayTopBaner3").FirstOrDefault().Value,
                TopSliderPic4 = settings.Where(x => x.Key == "MondayTopBaner4").FirstOrDefault().Value,
                TopSliderPic5 = settings.Where(x => x.Key == "MondayTopBaner5").FirstOrDefault().Value,
                TopSliderPic6 = settings.Where(x => x.Key == "MondayTopBaner6").FirstOrDefault().Value,
            };
            return View(specialMondayInfo);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductFromMonday(string code)
        {
            try
            {
                var product = await _context.Products.Where(x => x.Code == code).ToListAsync();
                if (product == null || product.Count <= 0)
                {
                    return Json(new { status = 0, message = "محصولی یافت نشد" });
                }
                foreach (var pr in product)
                {
                    pr.MondaySpecialSale = false;
                    pr.Discount = 0;

                    pr.UpdatedAt = DateTime.Now;
                }
                _context.Products.UpdateRange(product);
                await _context.SaveChangesAsync();
                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "خطایی رخ داده است" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddProductFromMonday([FromBody] AddProductInMondayViewModel model)
        {
            try
            {
                var product = await _context.Products.Where(x => x.Code == model.Code).ToListAsync();
                if (product == null || product.Count <= 0)
                {
                    return Json(new { status = 0, message = "محصولی یافت نشد" });
                }
                foreach (var pr in product)
                {
                    pr.MondaySpecialSale = true;
                    pr.Discount = Convert.ToInt32(model.Discount);
                    pr.UpdatedAt = DateTime.Now;
                }
                _context.Products.UpdateRange(product);
                await _context.SaveChangesAsync();
                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "خطایی رخ داده است" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TopSlider([FromForm] SpecialMonday model)
        {
            try
            {
                //MondayTopBaner1
                if (model.TopSliderPic1 != null)
                {
                    Setting topSlider1 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner1")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider1.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider1.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider1.Value = upload(model.TopSliderPic1);
                    topSlider1.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider1).State = EntityState.Modified;

                }

                //MondayTopBaner2
                if (model.TopSliderPic2 != null)
                {
                    Setting topSlider2 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner2")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider2.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider2.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider2.Value = upload(model.TopSliderPic2);
                    topSlider2.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider2).State = EntityState.Modified;

                }

                //MondayTopBaner3
                if (model.TopSliderPic3 != null)
                {
                    Setting topSlider3 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner3")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider3.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider3.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider3.Value = upload(model.TopSliderPic3);
                    topSlider3.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider3).State = EntityState.Modified;

                }

                //MondayTopBaner4
                if (model.TopSliderPic4 != null)
                {
                    Setting topSlider4 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner4")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider4.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider4.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider4.Value = upload(model.TopSliderPic4);
                    topSlider4.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider4).State = EntityState.Modified;

                }


                //MondayTopBaner5
                if (model.TopSliderPic5 != null)
                {
                    Setting topSlider5 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner5")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider5.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider5.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider5.Value = upload(model.TopSliderPic5);
                    topSlider5.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider5).State = EntityState.Modified;

                }


                //MondayTopBaner6
                if (model.TopSliderPic6 != null)
                {
                    Setting topSlider6 = await _context.Settings.Where(x => x.Key.Equals("MondayTopBaner6")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(topSlider6.Value))
                    {
                        try
                        {
                            await DeleteImage(topSlider6.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    topSlider6.Value = upload(model.TopSliderPic6);
                    topSlider6.UpdatedAt = DateTime.Now;
                    _context.Entry(topSlider6).State = EntityState.Modified;

                }




                //MondayLeftBaner
                if (model.LeftPic != null)
                {
                    Setting leftpicture = await _context.Settings.Where(x => x.Key.Equals("MondayLeftBaner")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(leftpicture.Value))
                    {
                        try
                        {
                            await DeleteImage(leftpicture.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    leftpicture.Value = upload(model.LeftPic);
                    leftpicture.UpdatedAt = DateTime.Now;
                    _context.Entry(leftpicture).State = EntityState.Modified;

                }




                //MondayRightBaner
                if (model.RightPic != null)
                {
                    Setting rightpicture = await _context.Settings.Where(x => x.Key.Equals("MondayRightBaner")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(rightpicture.Value))
                    {
                        try
                        {
                            await DeleteImage(rightpicture.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    rightpicture.Value = upload(model.RightPic);
                    rightpicture.UpdatedAt = DateTime.Now;
                    _context.Entry(rightpicture).State = EntityState.Modified;

                }
                //MondayVerticalBaner1
                if (model.MondayVerticalBaner1 != null)
                {
                    Setting verticalBaner1 = await _context.Settings.Where(x => x.Key.Equals("MondayVerticalBaner1")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(verticalBaner1.Value))
                    {
                        try
                        {
                            await DeleteImage(verticalBaner1.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    verticalBaner1.Value = upload(model.MondayVerticalBaner1);
                    verticalBaner1.UpdatedAt = DateTime.Now;
                    _context.Entry(verticalBaner1).State = EntityState.Modified;

                }

                //MondayVerticalBaner2
                if (model.MondayVerticalBaner2 != null)
                {
                    Setting verticalBaner2 = await _context.Settings.Where(x => x.Key.Equals("MondayVerticalBaner2")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(verticalBaner2.Value))
                    {
                        try
                        {
                            await DeleteImage(verticalBaner2.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    verticalBaner2.Value = upload(model.MondayVerticalBaner2);
                    verticalBaner2.UpdatedAt = DateTime.Now;
                    _context.Entry(verticalBaner2).State = EntityState.Modified;

                }

                //MondayVerticalBaner3
                if (model.MondayVerticalBaner3 != null)
                {
                    Setting verticalBaner3 = await _context.Settings.Where(x => x.Key.Equals("MondayVerticalBaner3")).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(verticalBaner3.Value))
                    {
                        try
                        {
                            await DeleteImage(verticalBaner3.Value);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    verticalBaner3.Value = upload(model.MondayVerticalBaner3);
                    verticalBaner3.UpdatedAt = DateTime.Now;
                    _context.Entry(verticalBaner3).State = EntityState.Modified;

                }


                //MondayTitle
                if (!string.IsNullOrEmpty(model.Title))
                {
                    Setting title = await _context.Settings.Where(x => x.Key.Equals("MondayTitle")).FirstOrDefaultAsync();
                    title.Value = model.Title;
                    title.UpdatedAt = DateTime.Now;
                    _context.Entry(title).State = EntityState.Modified;

                }
                await _context.SaveChangesAsync();
                return Json(new { status = 1, message = "با موفقیت انجام شد" });

            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = "خطایی رخ داده است" });
            }
        }
        public class ProductList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        
    }
}
