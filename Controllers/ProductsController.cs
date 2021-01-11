using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;
using KaraYadak.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using EFCore.BulkExtensions;
using Newtonsoft.Json;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ProductsController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

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


        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetCategoryTypeNameById(int id)
        {
            var name = (from a in _context.ProductCategoryTypes
                        join c in _context.ProductCategories on a.Id equals c.ProductCategoryType
                        where c.Id.Equals(id)
                        select a.Name).FirstOrDefault();
            return name;
        }
        public string GetCategoryNameById(List<int> ids)
        {
            var names = "";
            foreach (var item in ids)
            {
                names += _context.ProductCategories.Find(item).Name + ",";

            }


            return names;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? page, int? draw, int start, int length)
        {
            if (page is null)
            {
                page = 1;
            }
            var items = (from a in _context.Products
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


            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                data = items.Select(s => new
                {
                    s.Product.Id,
                    s.Product.Name,
                    s.Product.ProductStatus,
                    Status = s.Product.ProductStatus.ToString(),
                    //s.Product.Description,
                    s.Product.CategoryIdLvl1,
                    s.Product.CategoryIdLvl2,
                    s.Product.CategoryIdLvl3,
                    //s.Product.CategoryIdLvl4,
                    s.Product.Code,
                    //UnitId = s.Product.Unit.Id,
                    //Unit = s.Product.Unit.Name,
                    s.Categories,
                    UpdatedAt = s.Product.UpdatedAt.ToFriendlyPersianDateTextify(),
                    SpecialSale = (s.Product.SpecialSale) ? "فعال" : "غیر فعال"
                }).OrderByDescending(x => x.Code)
                //.Skip((int)itemsPerPage * (page.Value - 1))
                //.Take((int)itemsPerPage)
            });

        }

        public async Task<IActionResult> Create()
        {
            ViewBag.PC = await _context.ProductCategories.ToListAsync();
            ViewBag.PCWT = _context.ProductCategoryTypes.Select(s => new ProductCategoriesWithType
            {
                Name = s.Name,
                Categories = _context.ProductCategories
                    .Where(w => w.ProductCategoryType == s.Id && w.Parent == 0)
                    .Select(c => new ProductCategoryWithChilds
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Childs = _context.ProductCategories.Where(i => i.Parent == c.Id).ToList()
                    })
                    .ToList()
            }).ToList();
            //return Json(ViewBag.PCWT);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel input, List<int> CategoryId, int UnitId, string productMiddleName)
        {
            try
            {

                //var products = await _context.Products.OrderByDescending(x => x.Id).ToListAsync();
                if (await _context.Products.AnyAsync(i => i.Code == input.Code || i.Name == input.Name))
                {
                    return Json(new { status = "0", message = "کالا از قبل وجود دارد" });
                }


                var categories = await _context.ProductCategories.ToListAsync();
                var productCategoryTypes = await _context.ProductCategoryTypes.ToListAsync();
                var unit = _context.ProductUnits.Find(UnitId);
                int cid_1 = 0, cid_2 = 0, cid_3 = 0, cid_4 = 0;

                foreach (var cid in CategoryId)
                {
                    cid_1 = cid;
                    cid_2 = _context.ProductCategories.Find(cid_1) != null ? _context.ProductCategories.Find(cid_1).Parent : 0;
                    cid_3 = _context.ProductCategories.Find(cid_2) != null ? _context.ProductCategories.Find(cid_2).Parent : 0;
                    var category = cid_1;
                    category = cid_2 != 0 ? cid_2 : category;
                    category = cid_3 != 0 ? cid_3 : category;
                    cid_4 = categories.SingleOrDefault(x => x.Id == category).ProductCategoryType;

                    await _context.AddAsync(new Product
                    {
                        CategoryIdLvl1 = cid_1,
                        CategoryIdLvl2 = cid_2,
                        CategoryIdLvl3 = cid_3,
                        ProductCategoryType = cid_4,
                        Code = input.Code,
                        Name = input.Name,
                        MinEntity = input.MinEntity,
                        MaxEntity = input.MaxEntity,
                        ProductStatus = ProductStatus.دردسترس,
                        Unit = unit,
                        Description = input.Description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
                await _context.SaveChangesAsync();
                return Json(new { status = "1", message = "ثبت شد" });
            }
            catch (Exception ex)
            {

                return Json(new { status = "0", message = "خطایی رخ داده است" });

            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Products.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProductStatus,Description,CategoryId,SecondCategoryId,ThirdCategoryId,ForthCategoryId,Code,UnitId")] Product item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    item.UpdatedAt = DateTime.Now;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json("404");
            }

            var item = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return Json("404");
            }

            _context.Products.Remove(item);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToSpecial(string code)
        {
            if (code == null)
            {
                return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            }

            var item = _context.Products.Where(m => m.Code == code).ToList();
            if (item == null)
            {
                return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            }
            foreach (var i in item)
            {
                i.SpecialSale = true;
                _context.Products.Update(i);
            }
            await _context.SaveChangesAsync();

            return new JsonResult(new { status = "1", message = "به پیشنهاد ویژه اضافه شد" });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveProductFromSpecial(string code)
        {
            if (code == null)
            {
                return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            }

            var item = _context.Products.Where(m => m.Code == code).ToList();

            if (item == null)
            {
                return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            }
            foreach (var i in item)
            {
                i.SpecialSale = false;
                _context.Products.Update(i);
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { status = "1", message = "از پیشنهاد ویژه حذف شد" });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        public async Task<IActionResult> RemoveProducts()
        {
            //var products = await _context.Products.Where(x => x.Code != "10027").ToListAsync();
            //_context.Products.RemoveRange(products);
            //await _context.SaveChangesAsync();
            await _context.Products.AddAsync(new Product
            {
                Code = "1000",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            await _context.SaveChangesAsync();
            return Json(true);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(string code)
        {

            //Comment
            ViewBag.comments = await _context.Comments.Where(x => x.Status.Equals(CommentStatus.تایید_شده) && x.ProductCode.Equals(code)).OrderByDescending(x => x.Date).Select(
                x => new CommentVm
                {
                    code = code,
                    Text = x.Text,
                    Rate = x.Rate,
                    UserFullName = _context.Users.FirstOrDefault(i => i.UserName.Equals(x.Username)).FirstName + " " + _context.Users.FirstOrDefault(i => i.UserName.Equals(x.Username)).LastName
                }
                ).Distinct().Take(15).ToListAsync();

            var brandWithCars = new Dictionary<int, List<string>>();

            var finalModel = new ProductDetailSVM();
            //
            var items = (from p in _context.Products.Where(x => x.Code.Equals(code))
                         join c in _context.ProductCategories.DefaultIfEmpty() on p.CategoryIdLvl1 equals c.Id
                         into cpTbles
                         from cp in cpTbles.DefaultIfEmpty()
                         join ct in _context.ProductCategoryTypes
                         on cp.ProductCategoryType equals ct.Id
                         into table2
                         from t in table2.DefaultIfEmpty()
                         where t.Id.Equals(5)
                         select new
                         {
                             carModel = cp.Name,
                             carBrand = p.CategoryIdLvl2
                         }
                                 ).AsQueryable();
            var productWithCars = await items.ToListAsync();
            foreach (var item in productWithCars)
            {
                if (!brandWithCars.ContainsKey(item.carBrand))
                {
                    var newCarModelsList = new List<string>();
                    newCarModelsList.Add(item.carModel);
                    brandWithCars.Add(item.carBrand, newCarModelsList);
                }
                else
                {
                    var newCarModelsList = brandWithCars[item.carBrand];
                    newCarModelsList.Add(item.carModel);
                    brandWithCars[item.carBrand] = newCarModelsList.Distinct().ToList();
                }
            }
            var product = await _context.Products.Where(x => x.Code.Contains(code)).Select(x => new ProductDetailVM
            {
                Code = x.Code,
                Description = x.Description,
                Price = x.Price,
                Title = x.Name,
                Images = x.ImageUrl,
            }).FirstOrDefaultAsync();

            finalModel.Product = product;
            var groupByCodeProduct = _context.Products.ToLookup(p => p.Code, p => new ProductForIndexVM
            {
                CreatingDate = p.CreatedAt,
                Title = p.Name,
                Code = p.Code,
                Off = p.Discount,
                Picture = p.ImageUrl,
                Price = p.Price

            }).ToList();
            var otherProducts = groupByCodeProduct/*.Where()*/.Select(x => new ProductForIndexVM
            {
                Code = x.FirstOrDefault().Code,
                CreatingDate = x.FirstOrDefault().CreatingDate,
                Title = x.FirstOrDefault().Title,
                Off = x.FirstOrDefault().Off,
                Picture = x.FirstOrDefault().Picture,
                Price = x.FirstOrDefault().Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.CreatingDate).Take(12).ToList();
            var carBranding = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<int, List<string>> item in brandWithCars)
            {
                var carBrand = _context.ProductCategories.Find(item.Key).Image;

                carBranding.Add(carBrand, item.Value);
            }
            finalModel.OtherProduct = otherProducts;
            finalModel.CarCategoriesForProduct = carBranding;

            return View(finalModel);
        }
        [AllowAnonymous]
        [Route("Search/{key?}/{Page?}")]
        public IActionResult SearchProduct(string key, int page)
        {
            //filter side
            var categories = (from c in _context.ProductCategories
                              join ct in _context.ProductCategoryTypes
                              on c.ProductCategoryType equals ct.Id
                              into tble
                              from t in tble
                              select new FilteringVM
                              {
                                  CatId = t.Id,
                                  SubCategory = t.Name,
                                  Categories = c.Name
                              }).ToList();


            ViewBag.Filter = categories.GroupBy(x => x.CatId).Select(x =>
            new FilterVM
            {
                SubCategory = x.FirstOrDefault().SubCategory,
                Categories = x.ToList().Select(y => y.Categories).ToList(),
                CatId = x.Key
            }).ToList();


            var baner = _context.Baners.OrderByDescending(x => x.Date).FirstOrDefault();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            ViewBag.Key = key;
            if (page == 0)
            {
                page = 1;
            }
            if (string.IsNullOrEmpty(key)) key = " ";
            //Other Product With BestSeller Product
            var sellers = from r in _context.CartItems
                          group r by r.ProductId into g
                          select new
                          {
                              ProductId = g.Key,
                              Sum = g.Sum(x => x.Quantity)
                          };
            var bestSellers = sellers.OrderByDescending(x => x.Sum).Take(24).ToList();
            var segustProduct = new List<ProductForIndexVM>();
            foreach (var item in bestSellers)
            {
                var pr = (from x in _context.Products
                          where x.Id.Equals(item.ProductId)
                          select new ProductForIndexVM
                          {
                              Id = x.Id,
                              CreatingDate = x.CreatedAt,
                              Title = x.Name,
                              Off = x.Discount,
                              Picture = x.ImageUrl,
                              Price = x.Price,
                              Rate = x.Rate.GetValueOrDefault(),
                              Code = x.Code
                          }).FirstOrDefault();
                segustProduct.Add(pr);
            }
            ViewBag.segustProduct = segustProduct;

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
                                          Tags = pp.FirstOrDefault().ac.Tags ?? "",
                                          Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name ?? "")).ToArray())),
                                          SubCategories = pp.FirstOrDefault().ac.CategoryIdLvl2 > 0 ? (_context.ProductCategories.Find(pp.FirstOrDefault().ac.CategoryIdLvl2).Name) : "",
                                          CategoriyTypes = (pp.FirstOrDefault().ac.CategoryIdLvl1 > 0) ? GetCategoryTypeNameById(pp.FirstOrDefault().ac.CategoryIdLvl1) : "",
                                      })
                                     .ToList();


            var products = groupByCodeProduct.Where(x => x.Categories.Contains(key) || x.Product.Name.Contains(key) || x.Tags.Contains(key) || x.SubCategories.Contains(key) || x.Product.Code.Contains(key)
              || x.CategoriyTypes.Equals(key)).Select(x => new ProductForIndexVM
              {
                  Id = x.Product.Id,
                  CreatingDate = x.Product.CreatedAt,
                  Title = x.Product.Name,
                  Off = x.Product.Discount,
                  Picture = x.Product.ImageUrl,
                  Price = x.Product.Price,
                  Rate = x.Product.Rate.GetValueOrDefault(),
                  Code = x.Product.Code
              }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            ViewBag.Page = Math.Ceiling(Convert.ToDecimal(products.Count / 12));
            ViewBag.CurrectPage = page;
            //OferBox
            ViewBag.BestOfferProduct = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Code = x.Product.Code,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();
            return View(products.Skip((page - 1) * 12).Take(12).ToList());

        }
        [AllowAnonymous]

        [Route("ProductList/{Page?}")]
        public IActionResult AllProducts(int page)
        {

            if (page == 0)
            {
                page = 1;
            }


            var products = new List<ProductForIndexVM>();
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
                                      select new
                                      {
                                          Product = pp.FirstOrDefault().ac,
                                      })
                                 .ToList();



            products = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Id = x.Product.Id,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
                Rate = x.Product.Rate.GetValueOrDefault(),
            }).OrderByDescending(x => x.CreatingDate).ToList();
            ViewBag.Page = Math.Ceiling(Convert.ToDecimal(products.Count / 12));
            ViewBag.CurrectPage = page;
            return View(products.Skip((page - 1) * 12).Take(12).ToList());


        }
        [HttpPost]
        public async Task<ActionResult> AddProductWithExel(IFormFile file)
        {
            var categories = await _context.ProductCategories.ToListAsync();

            var code = "";
            var name = "";
            var des = "";
            var ids = new List<int>();
            var price = 0.0;
            var discount = 0;
            var count = 0;
            if (Request != null)
            {
                if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    //update product list
                    var updateProducts = new List<Product>();
                    var updateProductsImg = new List<Image>();
                    var prosucts = new List<Product>();

                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.Length];
                    var data = file.OpenReadStream().Read(fileBytes, 0, Convert.ToInt32(file.Length));
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            try
                            {

                                name = (workSheet.Cells[rowIterator, 1].Value).ToString();
                                code = (workSheet.Cells[rowIterator, 2].Value).ToString();
                                des = (workSheet.Cells[rowIterator, 3].Value).ToString();
                                var stringIds = (workSheet.Cells[rowIterator, 4].Value).ToString();
                                var splitedId = stringIds.Split(",");
                                ids.Clear();
                                foreach (var item in splitedId)
                                {
                                    ids.Add(int.Parse(item));
                                }
                                price = Convert.ToDouble(workSheet.Cells[rowIterator, 5].Value);
                                discount = Convert.ToInt16(workSheet.Cells[rowIterator, 6].Value);
                                count = Convert.ToInt16(workSheet.Cells[rowIterator, 7].Value);
                                if (!_context.Products.Any(x => x.Code == code))
                                {
                                    int cid_1 = 0, cid_2 = 0, cid_3 = 0, cid_4 = 0;

                                    foreach (var id in ids)
                                    {
                                        cid_1 = id;
                                        cid_2 = _context.ProductCategories.Find(cid_1) != null ? _context.ProductCategories.Find(cid_1).Parent : 0;
                                        cid_3 = _context.ProductCategories.Find(cid_2) != null ? _context.ProductCategories.Find(cid_2).Parent : 0;
                                        var category = cid_1;
                                        category = cid_2 != 0 ? cid_2 : category;
                                        category = cid_3 != 0 ? cid_3 : category;
                                        cid_4 = (categories.SingleOrDefault(x => x.Id == category)) != null ?
                                            categories.SingleOrDefault(x => x.Id == category).ProductCategoryType : 0;
                                        var newProducts = new Product()
                                        {
                                            Code = code,
                                            Name = name,
                                            Description = des,
                                            CreatedAt = DateTime.Now,
                                            Discount = discount,
                                            MaxEntity = count,
                                            MinEntity = count,
                                            UpdatedAt = DateTime.Now,
                                            CategoryIdLvl1 = cid_1,
                                            CategoryIdLvl2 = cid_2,
                                            CategoryIdLvl3 = cid_3,
                                            ProductCategoryType = cid_4,
                                            ProductStatus = ProductStatus.آماده_برای_فروش,
                                            ImageUrl = GetPicture(cid_4),
                                            Price = price,

                                        };
                                        updateProducts.Add(newProducts);


                                    }


                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        //var maxId = await _context.Products.MaxAsync(x => x.Id);

                        //foreach (var pr in await _context.Products.Where(x=>x.Id>maxId).ToListAsync())
                        //{
                        //    var img = new Image()
                        //    {
                        //        CreatedAt = DateTime.Now,
                        //        UpdatedAt = DateTime.Now,
                        //        Key = pr.Code,
                        //        ProductId = pr.Id,
                        //        Url = "9fad10de-80a0-4601-b481-2d53b62faea7.jpg",
                        //    };
                        //    updateProductsImg.Add(img);
                        //}

                        //await _context.Images.AddRangeAsync(updateProductsImg);
                        await _context.AddRangeAsync(updateProducts);

                        await _context.SaveChangesAsync();


                        //await _context.BulkInsertAsync(updateProducts);


                    }
                }
            }


            return new JsonResult(new { status = 1, message = "با موفقیت انجام شد" });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> updateProductWithExel(IFormFile file)
        {
            string xx = "";
            var code = "";
            if (Request != null)
            {
                if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    //update product list
                    var updateProducts = new List<Product>();
                    var prosucts = new List<Product>();

                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.Length];
                    var data = file.OpenReadStream().Read(fileBytes, 0, Convert.ToInt32(file.Length));
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            try
                            {

                                code = (workSheet.Cells[rowIterator, 1].Value).ToString();
                                if (_context.Products.Any(x => x.Code == code))
                                {
                                    prosucts = await _context.Products.Where(x => x.Code == code).ToListAsync();
                                    var price = Convert.ToDouble(workSheet.Cells[rowIterator, 2].Value);
                                    var count = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value);
                                    foreach (var item in prosucts)
                                    {
                                        item.Price = price;
                                        item.MinEntity = count;
                                        item.MaxEntity = count;
                                    }
                                    updateProducts.AddRange(prosucts);

                                }
                            }
                            catch (Exception ex)
                            {

                                //return new JsonResult(new { status = 0, message = ex.Message });

                            }
                        }
                        _context.Products.UpdateRange(updateProducts);
                        await _context.SaveChangesAsync();
                    }
                }
            }


            return new JsonResult(new { status = 1, message = "با موفقیت انجام شد" });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> exel(IFormFile file)
        {
            string xx = "";

            if ((file == null) && (file.Length <= 0) && string.IsNullOrEmpty(file.FileName))
            {
                return Json(new { status = "0", message = "خطا در فایل ارسالی" });
            }
            var fileName = DateTime.Now.Ticks.ToString() + Path.GetFileName(file.FileName);
            var path = _webHostEnvironment.WebRootPath + "/uploads/" + fileName;
            file.CopyTo(new FileStream(path, FileMode.Create));
            var fileLocation = new FileInfo(path);
            try
            {
                using (ExcelPackage package = new ExcelPackage(fileLocation))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    //var workSheet = package.Workbook.Worksheets.First();
                    int totalRows = workSheet.Dimension.Rows;

                    var DataList = new List<ApplicationUser>();

                    for (int i = 4; i <= totalRows; i++)
                    {
                        byte[] salt = new byte[128 / 8];
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(salt);
                        }

                        if (workSheet.Cells[i, 1].Value != null)
                        {
                            var x = workSheet.Cells[i, 1].Value.ToString();
                            xx += x + " , ";
                        }



                    }
                    //await _context.Users.AddRangeAsync(DataList);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                return Json(new { status = "0", message = ex.Message });
            }

            return Json(new { status = "1", message = xx /*"با موفقیت ارسال شد" */});
        }
        [AllowAnonymous]

        [Route("Filtering/{type1?}/{type2?}/{Page?}")]
        public async Task<IActionResult> IndexFilterProduct(string type1, string type2, int page)
        {
            /* ViewBag.FilterName*/
            var filterName = "";
            if (!string.IsNullOrWhiteSpace(type1))
            {
                filterName = type1;
            }
            else if (!string.IsNullOrWhiteSpace(type2))
            {
                filterName = type2;
            }
            if (!string.IsNullOrEmpty(filterName))
            {
                string categorytypeName = "";
                var cat = await _context.ProductCategories.FirstOrDefaultAsync(x => x.Name == filterName);
                if (cat == null)
                {
                    var categorytype = await _context.ProductCategoryTypes.FirstOrDefaultAsync(x => x.Name == filterName);
                    categorytypeName = categorytype.Name;
                    ViewBag.FilterName = filterName;

                }
                else
                {
                    categorytypeName = _context.ProductCategoryTypes.FirstOrDefault(x => x.Id == cat.ProductCategoryType).Name;
                    ViewBag.FilterName = categorytypeName + " : " + filterName;
                    var subCategory = _context.ProductCategories.FirstOrDefault(x => x.Id == cat.Parent);

                    if (subCategory != null)
                    {
                        ViewBag.FilterName = categorytypeName + " : " + subCategory.Name + " ----> " + filterName;
                    }

                }

            }

            //filter side
            var categories = (from c in _context.ProductCategories
                              join ct in _context.ProductCategoryTypes
                              on c.ProductCategoryType equals ct.Id
                              into tble
                              from t in tble
                              select new FilteringVM
                              {
                                  CatId = t.Id,
                                  SubCategory = t.Name,
                                  Categories = c.Name
                              }).ToList();


            ViewBag.Filter = categories.GroupBy(x => x.CatId).Select(x =>
            new FilterVM
            {
                SubCategory = x.FirstOrDefault().SubCategory,
                Categories = x.ToList().Select(y => y.Categories).ToList(),
                CatId = x.Key
            }).ToList();
            if (type2 == "cars")
            {
                ViewBag.IsCar = true;
                ViewBag.car = _context.ProductCategories.Where(x => x.Name.Equals(type1)).FirstOrDefault();
            }
            var baner = _context.Baners.OrderByDescending(x => x.Date).FirstOrDefault();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            ViewBag.type1 = type1;
            ViewBag.type2 = type2;
            if (page == 0)
            {
                page = 1;
            }
            if (string.IsNullOrEmpty(type1)) type1 = "nothing";
            if (string.IsNullOrEmpty(type2)) type2 = "nothing";




            //Other Product With BestSeller Product
            var sellers = from r in _context.CartItems
                          group r by r.ProductId into g
                          select new
                          {
                              ProductId = g.Key,
                              Sum = g.Sum(x => x.Quantity)
                          };
            var bestSellers = sellers.OrderByDescending(x => x.Sum).Take(24).ToList();
            var segustProduct = new List<ProductForIndexVM>();
            foreach (var item in bestSellers)
            {
                var pr = (from x in _context.Products
                          where x.Id.Equals(item.ProductId)
                          select new ProductForIndexVM
                          {
                              Id = x.Id,
                              CreatingDate = x.CreatedAt,
                              Title = x.Name,
                              Off = x.Discount,
                              Picture = x.ImageUrl,
                              Price = x.Price,
                              Rate = x.Rate.GetValueOrDefault(),
                              Code = x.Code
                          }).FirstOrDefault();
                segustProduct.Add(pr);
            }
            ViewBag.segustProduct = segustProduct;






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
                                          Tags = pp.FirstOrDefault().ac.Tags ?? "",
                                          Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name ?? "")).ToArray())),
                                          SubCategories = GetCategoryNameById(pp.Where(x => x.ac.CategoryIdLvl2 > 0).Select(x => x.ac.CategoryIdLvl2).ToList()),
                                          CategoriyTypes = (pp.FirstOrDefault().ac.CategoryIdLvl1 > 0) ? GetCategoryTypeNameById(pp.FirstOrDefault().ac.CategoryIdLvl1) : "",
                                      })
                                     .ToList();
            var products = groupByCodeProduct.Where(x => x.Categories.Contains(type1) || x.Categories.Contains(type2) || x.SubCategories.Contains(type1)
            || x.CategoriyTypes.Equals(type2) || x.CategoriyTypes.Equals(type1)).Select(x => new ProductForIndexVM
            {
                Id = x.Product.Id,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
                Rate = x.Product.Rate.GetValueOrDefault(),
                Code = x.Product.Code,
            }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            ViewBag.Page = Math.Ceiling(Convert.ToDecimal(products.Count / 12));
            ViewBag.CurrectPage = page;
            //OferBox
            ViewBag.BestOfferProduct = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Code = x.Product.Code,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();

            return View(products.Skip((page - 1) * 12).Take(12).ToList());

        }

        [Route("Filter/{car?}/{brand?}/{mainCategory?}/{subCategory?}/{page?}")]
        public async Task<IActionResult> Filter(string car, string brand, string mainCategory, string subCategory, int page)
        {
            /* ViewBag.FilterName*/
            var filterName = "";
            if (!string.IsNullOrWhiteSpace(mainCategory))
            {
                filterName = mainCategory;
                if (!string.IsNullOrEmpty(subCategory))
                {
                    filterName = mainCategory + " ----- " + subCategory;
                }
            }
            if (!string.IsNullOrEmpty(car))
            {
                filterName += "  خودرو ---- " + car;
            }

            if (!string.IsNullOrEmpty(brand))
            {
                filterName += "  برند ---- " + brand;
            }
            if (!string.IsNullOrEmpty(filterName))
            {
                ViewBag.FilterName = filterName;


            }

            //filter side
            var categories = (from c in _context.ProductCategories
                              join ct in _context.ProductCategoryTypes
                              on c.ProductCategoryType equals ct.Id
                              into tble
                              from t in tble
                              select new FilteringVM
                              {
                                  CatId = t.Id,
                                  SubCategory = t.Name,
                                  Categories = c.Name
                              }).ToList();


            ViewBag.Filter = categories.GroupBy(x => x.CatId).Select(x =>
            new FilterVM
            {
                SubCategory = x.FirstOrDefault().SubCategory,
                Categories = x.ToList().Select(y => y.Categories).ToList(),
                CatId = x.Key
            }).ToList();

            var baner = await _context.Baners.OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            if (page == 0)
            {
                page = 1;
            }




            //Other Product With BestSeller Product
            var sellers = from r in _context.CartItems
                          group r by r.ProductId into g
                          select new
                          {
                              ProductId = g.Key,
                              Sum = g.Sum(x => x.Quantity)
                          };
            var bestSellers = sellers.OrderByDescending(x => x.Sum).Take(24).ToList();
            var segustProduct = new List<ProductForIndexVM>();
            foreach (var item in bestSellers)
            {
                var pr = (from x in _context.Products
                          where x.Id.Equals(item.ProductId)
                          select new ProductForIndexVM
                          {
                              Id = x.Id,
                              CreatingDate = x.CreatedAt,
                              Title = x.Name,
                              Off = x.Discount,
                              Picture = x.ImageUrl,
                              Price = x.Price,
                              Rate = x.Rate.GetValueOrDefault(),
                              Code = x.Code
                          }).FirstOrDefault();
                segustProduct.Add(pr);
            }
            ViewBag.segustProduct = segustProduct;






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
                                          Tags = pp.FirstOrDefault().ac.Tags ?? "",
                                          Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.Name ?? "")).ToArray())),
                                          SubCategories = GetCategoryNameById(pp.Where(x => x.ac.CategoryIdLvl2 > 0).Select(x => x.ac.CategoryIdLvl2).ToList()),
                                          CategoriyTypes = (pp.FirstOrDefault().ac.CategoryIdLvl1 > 0) ? GetCategoryTypeNameById(pp.FirstOrDefault().ac.CategoryIdLvl1) : "",
                                      })
                                     .ToList();

            var products = groupByCodeProduct.Where(x => x.Categories.Contains(subCategory) ||
            x.Categories.Contains(car) || x.SubCategories.Contains(brand)
            || x.CategoriyTypes.Equals(mainCategory) || x.CategoriyTypes.Equals(subCategory)).Select(x => new ProductForIndexVM
            {
                Id = x.Product.Id,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
                Rate = x.Product.Rate.GetValueOrDefault(),
                Code = x.Product.Code,
            }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            ViewBag.Page = Math.Ceiling(Convert.ToDecimal(products.Count / 12));
            ViewBag.CurrectPage = page;
            //OferBox
            ViewBag.BestOfferProduct = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Code = x.Product.Code,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();

            if (!string.IsNullOrWhiteSpace(car))
            {

                products = groupByCodeProduct.Where(x => x.Categories.Contains(car)).Select(x => new ProductForIndexVM
                {
                    Id = x.Product.Id,
                    CreatingDate = x.Product.CreatedAt,
                    Title = x.Product.Name,
                    Off = x.Product.Discount,
                    Picture = x.Product.ImageUrl,
                    Price = x.Product.Price,
                    Rate = x.Product.Rate.GetValueOrDefault(),
                    Code = x.Product.Code,
                }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {

                products = groupByCodeProduct.Where(x => x.Categories.Contains(brand)).Select(x => new ProductForIndexVM
                {
                    Id = x.Product.Id,
                    CreatingDate = x.Product.CreatedAt,
                    Title = x.Product.Name,
                    Off = x.Product.Discount,
                    Picture = x.Product.ImageUrl,
                    Price = x.Product.Price,
                    Rate = x.Product.Rate.GetValueOrDefault(),
                    Code = x.Product.Code,
                }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            }

            if (!string.IsNullOrWhiteSpace(mainCategory))
            {

                products = groupByCodeProduct.Where(x => x.Categories.Contains(mainCategory)||x.CategoriyTypes.Contains(mainCategory)).Select(x => new ProductForIndexVM
                {
                    Id = x.Product.Id,
                    CreatingDate = x.Product.CreatedAt,
                    Title = x.Product.Name,
                    Off = x.Product.Discount,
                    Picture = x.Product.ImageUrl,
                    Price = x.Product.Price,
                    Rate = x.Product.Rate.GetValueOrDefault(),
                    Code = x.Product.Code,
                }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            }

            if (!string.IsNullOrWhiteSpace(subCategory))
            {

                products = groupByCodeProduct.Where(x => x.Categories.Contains(subCategory) || x.CategoriyTypes.Contains(subCategory)).Select(x => new ProductForIndexVM
                {
                    Id = x.Product.Id,
                    CreatingDate = x.Product.CreatedAt,
                    Title = x.Product.Name,
                    Off = x.Product.Discount,
                    Picture = x.Product.ImageUrl,
                    Price = x.Product.Price,
                    Rate = x.Product.Rate.GetValueOrDefault(),
                    Code = x.Product.Code,
                }).OrderByDescending(x => x.CreatingDate).Distinct().ToList();
            }
            return View(products.Skip((page - 1) * 12).Take(12).ToList());
        }
        [AllowAnonymous]

        [Route("ListOfCars/{Page?}")]
        public async Task<IActionResult> ListOfCars(int page)
        {
            var baner = _context.Baners.OrderByDescending(x => x.Date).FirstOrDefault();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            if (page == 0)
            {
                page = 1;
            }




            //Other Product With BestSeller Product
            var sellers = from r in _context.CartItems
                          group r by r.ProductId into g
                          select new
                          {
                              ProductId = g.Key,
                              Sum = g.Sum(x => x.Quantity)
                          };
            var bestSellers = sellers.OrderByDescending(x => x.Sum).Take(24).ToList();
            var segustProduct = new List<ProductForIndexVM>();
            foreach (var item in bestSellers)
            {
                var pr = (from x in _context.Products
                          where x.Id.Equals(item.ProductId)
                          select new ProductForIndexVM
                          {
                              Id = x.Id,
                              CreatingDate = x.CreatedAt,
                              Title = x.Name,
                              Off = x.Discount,
                              Picture = x.ImageUrl,
                              Price = x.Price,
                              Rate = x.Rate.GetValueOrDefault(),
                              Code = x.Code
                          }).FirstOrDefault();
                segustProduct.Add(pr);
            }
            ViewBag.segustProduct = segustProduct;


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
                                      })
                                     .ToList();

            var listOfCars = await _context.ProductCategories.Where(x => x.ProductCategoryType.Equals(5) && x.Parent.Equals(0)).Select(x => new CarsVM
            {
                Id = x.Id,
                Image = x.Image,
                Name = x.Name
            }).ToListAsync();
            //OferBox
            ViewBag.BestOfferProduct = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Code = x.Product.Code,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();
            ViewBag.Page = (listOfCars.Count / 12) + 1;
            ViewBag.CurrectPage = page;
            return View(listOfCars.Skip((page - 1) * 12).Take(12).ToList());

        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> AddToFavorite(string code)
        {
            if (string.IsNullOrEmpty(code)) return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            var username = User.Identity.Name;
            if (username == null) return new JsonResult(new { status = "2", message = "لطفا وارد اکانت خود شوید" });
            if (!IsExsistFavorite(code, username))
            {
                var favorite = new Favorite()
                {
                    CreateAt = DateTime.Now,
                    ProductCode = code,
                    UserName = username
                };
                await _context.Favorites.AddAsync(favorite);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "به علاقه مندی ها اضافه شد" });
            }
            else
            {
                var fav = await _context.Favorites.FirstOrDefaultAsync(x => x.ProductCode.Equals(code) && x.UserName.Equals(username));
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "از لیست علاقه مندی های شما حذف شد" });
            }
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> IsFavoriteForThisUser(string code)
        {
            if (string.IsNullOrEmpty(code)) return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            var username = User.Identity.Name;
            if (username == null) return null;
            return new JsonResult(new { status = "1", result = IsExsistFavorite(code, username) });
        }
        [AllowAnonymous]

        [Route("MyFavoriteProducts/{Page?}")]
        public async Task<IActionResult> MyFavoriteProducts(int page)
        {
            //filter side
            var categories = (from c in _context.ProductCategories
                              join ct in _context.ProductCategoryTypes
                              on c.ProductCategoryType equals ct.Id
                              into tble
                              from t in tble
                              select new FilteringVM
                              {
                                  CatId = t.Id,
                                  SubCategory = t.Name,
                                  Categories = c.Name
                              }).ToList();


            ViewBag.Filter = categories.GroupBy(x => x.CatId).Select(x =>
            new FilterVM
            {
                SubCategory = x.FirstOrDefault().SubCategory,
                Categories = x.ToList().Select(y => y.Categories).ToList(),
                CatId = x.Key
            }).ToList();
            //baner
            var baner = _context.Baners.OrderByDescending(x => x.Date).FirstOrDefault();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            if (page == 0)
            {
                page = 1;
            }
            //Other Product With BestSeller Product
            var sellers = from r in _context.CartItems
                          group r by r.ProductId into g
                          select new
                          {
                              ProductId = g.Key,
                              Sum = g.Sum(x => x.Quantity)
                          };
            var bestSellers = sellers.OrderByDescending(x => x.Sum).Take(24).ToList();
            var segustProduct = new List<ProductForIndexVM>();
            foreach (var item in bestSellers)
            {
                var pr = (from x in _context.Products
                          where x.Id.Equals(item.ProductId)
                          select new ProductForIndexVM
                          {
                              Id = x.Id,
                              CreatingDate = x.CreatedAt,
                              Title = x.Name,
                              Off = x.Discount,
                              Picture = x.ImageUrl,
                              Price = x.Price,
                              Rate = x.Rate.GetValueOrDefault(),
                              Code = x.Code
                          }).FirstOrDefault();
                segustProduct.Add(pr);
            }
            ViewBag.segustProduct = segustProduct;
            var myProductCode = await _context.Favorites.Where(x => x.UserName.Equals(User.Identity.Name)).Select(x => x.ProductCode).ToListAsync();

            var products = new List<ProductForIndexVM>();
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
                                      where myProductCode.Contains(pp.FirstOrDefault().ac.Code)
                                      select new
                                      {
                                          Product = pp.FirstOrDefault().ac,
                                      })

                                 .ToList();



            products = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Id = x.Product.Id,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
                Rate = x.Product.Rate.GetValueOrDefault(),
                Code = x.Product.Code,
            }).OrderByDescending(x => x.CreatingDate).ToList();
            ViewBag.Page = Math.Ceiling(Convert.ToDecimal(products.Count / 12));
            ViewBag.CurrectPage = page;
            //oferbox
            ViewBag.BestOfferProduct = groupByCodeProduct.Select(x => new ProductForIndexVM
            {
                Code = x.Product.Code,
                CreatingDate = x.Product.CreatedAt,
                Title = x.Product.Name,
                Off = x.Product.Discount,
                Picture = x.Product.ImageUrl,
                Price = x.Product.Price,
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();
            return View(products.Skip((page - 1) * 12).Take(12).ToList());

        }
        [NonAction]
        public bool IsExsistFavorite(string code, string userName)
        {
            bool IsExsist = _context.Favorites.Any(x => x.UserName.Equals(userName) && x.ProductCode.Equals(code));
            return IsExsist;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> AddCommentInProduct(string code, string text, string rate)
        {
            if (string.IsNullOrEmpty(code)) return new JsonResult(new { status = "0", message = "محصولی یافت نشد" });
            var username = User.Identity.Name;
            if (username == null) return new JsonResult(new { status = "2", message = "لطفا وارد اکانت خود شوید" });
            var product = _context.Products.FirstOrDefault(x => x.Code.Equals(code));
            if (!IsExsistCommentForThisUser(code, username))
            {
                var comment = new Comment()
                {
                    Date = DateTime.Now,
                    Product = product,
                    ProductCode = code,
                    Status = CommentStatus.در_حال_بررسی,
                    Text = text,
                    Username = username,
                    Rate = (!string.IsNullOrEmpty(rate)) ? int.Parse(rate) : 0,
                };
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "سپاس از نظردهی شما!" });
            }
            return new JsonResult(new { status = "1", message = "شما قبلا نظر خود را ثبت کرده اید!" });

        }
        [NonAction]
        public bool IsExsistCommentForThisUser(string code, string userName)
        {
            bool IsExsist = _context.Comments.Any(x => x.Username.Equals(userName) && x.ProductCode.Equals(code));
            return IsExsist;
        }

        //side filter
        [AllowAnonymous]

        public async Task<IActionResult> GetFilterMenu()
        {

            return PartialView();

            //return new JsonResult(new { status = "1", message = "شما قبلا نظر خود را ثبت کرده اید!" });
        }
        public string GetPicture(int id)
        {
            switch (id)
            {
                case 4:
                    return "/Uploads/ProductImages/مصرفی.JPG";
                case 5:
                    return "/Uploads/ProductImages/مصرفی.JPG";
                case 6:
                    return "/Uploads/ProductImages/موتور.JPG";
                case 7:
                    return "/Uploads/ProductImages/جلوبندی.JPG";
                case 8:
                    return "/Uploads/ProductImages/برقی.JPG";
                case 9:
                    return "/Uploads/ProductImages/تزئینی.JPG";
                case 10:
                    return "/Uploads/ProductImages/بدنه.JPG";
                case 11:
                    return "/Uploads/ProductImages/بدنه.JPG";
                case 12:
                    return "/Uploads/ProductImages/کلاج.JPG";
                case 13:
                    return "/Uploads/ProductImages/کلاج.JPG";
                case 14:
                    return "/Uploads/ProductImages/سایر.JPG";
                default:
                    return "/Uploads/ProductImages/سایر.JPG";
            }
        }
    }
}