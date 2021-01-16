using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.Extensions.Configuration;

namespace KaraYadak.Controllers
{
    public class ShopProductsController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private readonly IConfigurationSection _settings;

        public ShopProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IConfiguration iConfig)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = iConfig;
            _settings = _configuration.GetSection("AppSettings");
        }
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _hostingEnvironment.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
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
        public IActionResult Index()
        {
            var items = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.درفروشگاه || x.ProductStatus == ProductStatus.آماده_برای_فروش)
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                         group a by a.ac.Name into pp
                         select new ProductWithCategoryVM
                         {
                             Product = pp.FirstOrDefault().ac,
                             Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray())),
                             Id = pp.FirstOrDefault().ac.Id
                         })
                         .ToList();

            return View(items.OrderByDescending(x => x.Id));
        }
        public async Task<IActionResult> GetWareHouseProducts(int? page, int start, int length)
        {
            if (page is null)
            {
                page = 1;
            }

            //var items = await _context.Products
            //    .ToListAsync();
            var items = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.دردسترس)
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                         group a by a.ac.Name into pp
                         select new ProductWithCategoryVM
                         {
                             Product = pp.FirstOrDefault().ac,
                             Date = pp.FirstOrDefault().ac.CreatedAt.ToPersianDateTextify(),
                             Status = pp.FirstOrDefault().ac.ProductStatus.ToString(),
                             Categories = removeDuplicate(String.Join(",", (pp.Select(x => x.cc.Name)).ToArray())),
                             Id = pp.FirstOrDefault().ac.Id
                         })
                    .ToList();

            return Json(items.OrderByDescending(x => x.Id)/*.Skip(3).FirstOrDefault()*/);
        }
        [HttpPost]
        public async Task<IActionResult> GetSetOptions(int page, string name)
        {
            //var items = await _context.Products
            //    .ToListAsync();
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == name);
            var items = (from a in _context.Products.Where(x => x.Name != name).Where(x => x.ProductStatus == ProductStatus.درفروشگاه || x.ProductStatus == Models.ProductStatus.آماده_برای_فروش)
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                         group a by a.ac.Name into pp
                         select new ProductWithCategoryVM
                         {
                             Product = pp.FirstOrDefault().ac,
                             Date = pp.FirstOrDefault().ac.CreatedAt.ToPersianDateTextify(),
                             Status = pp.FirstOrDefault().ac.ProductStatus.ToString(),
                             Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray())),
                         }).OrderByDescending(x => x.Date)
                    .ToList();
            int count = items.Count;
            int pagesize = 2;
            bool before = true;
            bool after = true;
            if (page == 0 || page == 1)
            {
                page = 1;
                before = false;
            }
            else
            {
                before = true;
            }
            var skip = (page - 1) * pagesize;
            var pages = count / pagesize;
            if (pages / 2 != 0)
            {
                pages += 1;
            }
            if (pages == page)
            {
                after = false;
            }
            return Json(new { items = items.Skip(skip).Take(pagesize).ToList(), after, before, pages, product.SetProducts, page });
        }
        [HttpPost]
        public async Task<IActionResult> SearchSetProducts(string key, string name, int page)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == name);
            var items = (from a in _context.Products.Where(x => x.Name != name).Where(x => x.ProductStatus == ProductStatus.درفروشگاه || x.ProductStatus == Models.ProductStatus.آماده_برای_فروش)
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                         group a by a.ac.Name into pp
                         select new ProductWithCategoryVM
                         {
                             Product = pp.FirstOrDefault().ac,
                             Date = pp.FirstOrDefault().ac.CreatedAt.ToPersianDateTextify(),
                             Status = pp.FirstOrDefault().ac.ProductStatus.ToString(),
                             Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray())),
                         }).OrderByDescending(x => x.Date).Where(x => x.Product.Name.Contains(key))
                    .ToList();
            int count = items.Count;
            int pagesize = 2;
            bool before = true;
            bool after = true;
            if (page == 0 || page == 1)
            {
                page = 1;
                before = false;
            }
            else
            {
                before = true;
            }
            var skip = (page - 1) * pagesize;
            var pages = count / pagesize;
            if (pages / 2 != 0)
            {
                pages += 1;
            }
            if (pages == page)
            {
                after = false;
            }
            return Json(new { items = items.Skip(skip).Take(pagesize).ToList(), after, before, pages, product.SetProducts, page });
        }
        [HttpPost]
        public async Task<IActionResult> AddToShop(ProductVM product)
        {
            if (product.Status == ProductStatus.آماده_برای_فروش && product.DiscountPrice == 0 || product.Status == ProductStatus.آماده_برای_فروش && product.Price == 0)
            {
                return Json(new { method = "error", status = "0", message = "برای ورود به فروشگاه اول باید قیمت گذاری شود" });
            }
            var items = await _context.Products.Where(x => x.Name == product.Name).ToListAsync();
            foreach (var item in items)
            {
                item.Description = product.Description;
                item.Tags = product.Tags;
                if (product.Price != 0 && product.DiscountPrice != 0)
                {
                    item.Discount = Convert.ToInt32(((product.Price - product.DiscountPrice) / product.Price) * 100);
                }
                else
                {
                    item.Discount = 0;
                }
                item.SetProducts = product.setProductsValue;
                item.Price = product.Price;
                item.ProductStatus = product.Status;
                item.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            var products = (from a in _context.Products
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                            group a by a.ac.Name into pp
                            select new ProductWithCategoryVM
                            {
                                Product = pp.FirstOrDefault().ac,
                                Categories = String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray()),
                            })
                    .ToList();

            return Json(new
            {
                method = "create",
                item = products.Select(s => new
                {
                    s.Product.Id,
                    s.Product.Name,
                    s.Product.ProductStatus,
                    Status = s.Product.ProductStatus.ToString(),
                    Price = string.Format("{0:n0}", s.Product.Price).ToPersianNumbers(),
                    Date = s.Product.UpdatedAt.ToPersianDateTextify(),
                    s.Product.Description,
                    s.Product.CategoryIdLvl1,
                    s.Product.CategoryIdLvl2,
                    s.Product.CategoryIdLvl3,
                    //s.Product.CategoryIdLvl4,
                    s.Product.Code,
                    s.Product.Discount,
                    s.Product.Tags,
                    s.Product.ImageUrl,
                    s.Product.QR,
                    //UnitId = s.Product.Unit.Id,
                    //Unit = s.Product.Unit.Name,
                    s.Categories,
                    UpdatedAt = s.Product.UpdatedAt.ToFriendlyPersianDateTextify(),
                    product = s.Product.Description + "_" + s.Product.Id
                }).SingleOrDefault(x => x.Name == product.Name)
            });
        }
        public async Task<IActionResult> EditProduct(ProductVM product, string type)
        {
            var products = (from a in _context.Products
                     .Join(_context.ProductCategories,
                     ac => ac.CategoryIdLvl1,
                     cc => cc.Id,
                     (ac, cc) => new
                     {
                         ac,
                         cc
                     }
                     ).ToList()
                            group a by a.ac.Name into pp
                            select new ProductWithCategoryVM
                            {
                                Product = pp.FirstOrDefault().ac,
                                Categories = String.Join(", ", (pp.Select(x => x.cc.Name)).ToArray()),
                            }).OrderBy(x => x.Product.UpdatedAt)
                    .ToList();
            if (type == "0")
            {
                //Edit Single Product
                var items = await _context.Products.Where(x => x.Name == product.Name).ToListAsync();
                foreach (var item in items)
                {
                    item.Description = product.Description;
                    if (product.Tags != null)
                    {
                        item.Tags = product.Tags;
                    }
                    if (product.Price != 0 && product.DiscountPrice != 0)
                    {
                        item.Discount = 100 - Convert.ToInt32((product.DiscountPrice * 100) / product.Price);
                    }
                    else
                    {
                        item.Discount = 0;
                    }
                    item.SetProducts = product.setProductsValue;
                    item.Price = product.DiscountPrice;
                    item.UpdatedAt = DateTime.Now;
                    item.ProductStatus = product.Status;
                }
                await _context.SaveChangesAsync();

                return Json(new
                {
                    method = "edit",
                    item = products.Select(s => new
                    {
                        s.Product.Id,
                        s.Product.Name,
                        s.Product.ProductStatus,
                        Status = s.Product.ProductStatus.ToString(),
                        Price = string.Format("{0:n0}", s.Product.Price).ToPersianNumbers(),
                        Date = s.Product.UpdatedAt.ToPersianDateTextify(),
                        s.Product.Description,
                        s.Product.CategoryIdLvl1,
                        s.Product.CategoryIdLvl2,
                        s.Product.CategoryIdLvl3,
                        //s.Product.CategoryIdLvl4,
                        s.Product.Code,
                        s.Product.Discount,
                        s.Product.Tags,
                        s.Product.SpecialSale,
                        s.Product.ImageUrl,
                        //UnitId = s.Product.Unit.Id,
                        //Unit = s.Product.Unit.Name,
                        s.Categories,
                        UpdatedAt = s.Product.UpdatedAt.ToFriendlyPersianDateTextify(),
                        product = s.Product.Description + "_" + s.Product.Id
                    }).SingleOrDefault(x => x.Name == product.Name)
                });
            }
            else
            {
                //Edit Multipite Product
                var items = new List<Product>();
                var ids = product.Name.Split("_");
                foreach (var item in ids)
                {
                    if (item != "")
                    {
                        var p = await _context.Products.Where(x => x.Name == item).ToListAsync();
                        items.AddRange(p);
                    }
                }
                foreach (var item in items)
                {
                    item.Description = product.Description;
                    item.Tags = product.Tags;
                    if (product.Price != 0 && product.DiscountPrice != 0)
                    {
                        item.Discount = 100 - Convert.ToInt32((product.DiscountPrice * 100) / product.Price);
                    }
                    else
                    {
                        item.Discount = 0;
                    }
                    item.SetProducts = product.setProductsValue;
                    item.Price = product.DiscountPrice;
                    item.UpdatedAt = DateTime.Now;
                    item.ProductStatus = product.Status;
                }
                await _context.SaveChangesAsync();
                return Json(new
                {
                    method = "multipiteEdit",
                    item = products.Select(s => new
                    {
                        s.Product.Id,
                        s.Product.Name,
                        s.Product.ProductStatus,
                        Status = s.Product.ProductStatus.ToString(),
                        Price = string.Format("{0:n0}", s.Product.Price).ToPersianNumbers(),
                        Date = s.Product.UpdatedAt.ToPersianDateTextify(),
                        s.Product.Description,
                        s.Product.CategoryIdLvl1,
                        s.Product.CategoryIdLvl2,
                        s.Product.CategoryIdLvl3,
                        //s.Product.CategoryIdLvl4,
                        s.Product.Code,
                        s.Product.SpecialSale,
                        s.Product.Discount,
                        s.Product.Tags,
                        s.Product.ImageUrl,
                        //UnitId = s.Product.Unit.Id,
                        //Unit = s.Product.Unit.Name,
                        s.Categories,
                        UpdatedAt = s.Product.UpdatedAt.ToFriendlyPersianDateTextify(),
                        product = s.Product.Description + "_" + s.Product.Id
                    }).Where(x => product.Name.Split("_").Contains(x.Name)).ToList()
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string code)
        {
            if (code == null)
            {
                return new JsonResult(new { status = 0, message = " کد را وارد کنید " });
            }
            var item = await _context.Products.FirstOrDefaultAsync(x => x.Code == code);
            if (item == null)
                return new JsonResult(new { status = 0, message = " کد را وارد کنید " });

            var items = await _context.Products.Where(m => m.Name == item.Name).ToListAsync();


            _context.Products.RemoveRange(items);
            await _context.SaveChangesAsync();
            return new JsonResult(new { status = 1, message = "با موفقیت انجام شد" });

        }


        public async Task<IActionResult> AddProductToSpecial(string code)
        {
            var items = await _context.Products.Where(x => x.Code == code).ToListAsync();
            foreach (var item in items)
            {
                item.SpecialSale = true;
            }
            await _context.SaveChangesAsync();
            return Json(new { status = '1', message = "با موفقیت انجام شد" });
        }
        public async Task<IActionResult> RemoveFromSpecial(string code)
        {
            var items = await _context.Products.Where(x => x.Code == code).ToListAsync();
            foreach (var item in items)
            {
                item.SpecialSale = false;
            }
            await _context.SaveChangesAsync();
            return Json(new { status = '1', message = "با موفقیت انجام شد" });
        }
        [HttpGet]
        public async Task<IActionResult> GetProductImage(string code)
        {
            try
            {
                var product = await _context.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Code == code);
                return Json(product.Images.Select(x => x.Url));

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        public IActionResult AddImage(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var host = _hostingEnvironment.WebRootPath;
                var url = _settings.GetSection("ProductImages").Value;
                var savePath = Path.Combine(host, url);
                var uploadResult = FileUploader.UploadImage(image, savePath, compression: 70, width: 512, height: 512);
                if (!uploadResult.succsseded)
                {
                    return Json(false);
                }
                else
                {
                    var img = uploadResult.result;
                    return Json("/Uploads/ProductImages/" + img);
                }
            }
            else
            {
                return Json(false);
            }
        }
        public async Task<IActionResult> AddImgToGallery(string code, string image)
        {
            var products = await _context.Products.Where(x => x.Code == code).ToListAsync();
            if (products == null)
                return Json(new { status = '0' });
            foreach (var item in products)
            {
               
                    item.ImageUrl = image;
                
                var img = new Image();
                img.ProductId = item.Id;
                img.Key = item.Code;
                img.CreatedAt = DateTime.Now;
                img.UpdatedAt = DateTime.Now;
                img.Url = image;
                await _context.Images.AddAsync(img);
            }
            await _context.SaveChangesAsync();
            //var images = await _context.Images.Where(x => x.ProductId == id).ToListAsync();
            //var list = new List<string>();
            //foreach (var item in images)
            //{
            //    list.Add(item.Url);
            //}
            return Json(new { code, image });
        }
        public async Task<IActionResult> DeleteImgFromGallery(string name, string url)
        {
            var products = await _context.Products.Where(x => x.Code == name).ToListAsync();
            if (products == null)
                return Json(new { status = '0' });
            var id = 0;
            foreach (var item in products)
            {
                var images = await _context.Images.Where(x => x.ProductId == item.Id).ToListAsync();
                //if (images.Count == 1)
                //{
                //    return Json(new { status = '0', message = "هر محصول باید حداقل یک عکس داشته باشد" });
                //}
                if (images.Count != 0)
                {
                    if (item.ImageUrl == url)
                    {
                        //if (images.Count != 1)
                        //{
                        //    item.ImageUrl = images.Where(x => x.Url != url).FirstOrDefault().Url;
                        //}
                        //else
                        //{
                        item.ImageUrl = null;
                        //}
                    }
                    var img = await _context.Images.Where(x => x.Url == url && x.Key == item.Code).ToListAsync();
                    if (img != null)
                    {
                        _context.RemoveRange(img);
                    }
                }
                id = item.Id;
            }
            await _context.SaveChangesAsync();
            return Json(name);
        }
        public async Task<IActionResult> SetMainImage(string url, string name)
        {
            var products = await _context.Products.Where(x => x.Name == name).ToListAsync();
            if (products == null)
                return Json(new { status = '0' });
            foreach (var item in products)
            {
                item.ImageUrl = url;
            }
            await _context.SaveChangesAsync();
            return Json(new { status = "1", message = "با موفقیت انجام شد", name, url });
        }
    }
}
