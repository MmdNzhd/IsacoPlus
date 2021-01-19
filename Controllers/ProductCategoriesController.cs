using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
//using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account.Manage;

namespace KaraYadak.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductCategoriesController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _hostingEnvironment.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
        public IActionResult Index()
        {
            ViewBag.ProductCategoryParents = _context.ProductCategories.Where(i => i.Parent == 0).ToList();
            ViewBag.ProductCategoryTypes = _context.ProductCategoryTypes.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? page, int? draw, int start, int length)
        {
            if (page is null)
            {
                page = 1;
            }

            var items = await (from pc in _context.ProductCategories
                               join pp in _context.ProductCategories.DefaultIfEmpty() on pc.Parent equals pp.Id
                               into tbl1
                               from t1 in tbl1.DefaultIfEmpty()
                               join pct in _context.ProductCategoryTypes.DefaultIfEmpty() on pc.ProductCategoryType equals pct.Id
                               into tbl2
                               from t2 in tbl2.DefaultIfEmpty()
                               select new
                               {
                                   pc.Id,
                                   pc.Name,
                                   pc.ProductCategoryType,
                                   pc.Parent,
                                   pc.UpdatedAt,
                                   ParentString = pc.Parent > 0 ? t1.Name : "ندارد",
                                   ProductCategoryTypeString = pc.ProductCategoryType > 0 ? t2.Name : "بی نوع",
                               }).OrderByDescending(x => x.UpdatedAt)

                         .ToListAsync();

            int recordsTotal = items.Count();

            //items.Skip(start).Take(length);

            //int recordsFiltered = items.Count();

            return Json(new
            {
                data = items
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.ProductCategoryType,
                    s.ProductCategoryTypeString,
                    s.Parent,
                    s.ParentString,
                    UpdatedAt = s.UpdatedAt.ToFriendlyPersianDateTextify()
                })
            });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ProductCategoryType,Parent")] ProductCategory productCategory, IFormFile image)
        {
            //return Json(productCategory);
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var fileName = upload(image);
                    productCategory.Image = "/uploads/" + fileName;
                }
                _context.Update(productCategory);
                await _context.SaveChangesAsync();
                return Json("ok");
            }
            return Json("no");
        }
        public async Task<IActionResult> GetGetCategory(int id)
        {
            var cat = await _context.ProductCategories.FindAsync(id);
            if (cat == null)
                return new JsonResult(new { status = "0", message = "دسته بندی مورد نظر یافت نشد" });
            return new JsonResult(new { status = "1", message = "با موفقیت انجام شد" ,model=cat});

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProductCategoryType,Parent")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return Json("404");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productCategory.UpdatedAt = DateTime.Now;
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return Json("404");
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json("ok");
            }
            return Json("no");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json("404");
            }

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return Json("404");
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.Id == id);
        }
        public IActionResult RemoveSs()
        {
            var c = _context.ProductCategories.Where(x => x.Parent == 88).ToList()/*.Select(x => x.Name)*/;
            _context.ProductCategories.RemoveRange(c);
            _context.SaveChanges();
            return Json(c);
        }
    }
}
