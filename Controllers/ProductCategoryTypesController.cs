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

namespace KaraYadak.Controllers
{
    [Authorize]
    public class ProductCategoryTypesController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;

        public ProductCategoryTypesController(ApplicationDbContext context)
        {
            _context = context;
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

            var items = await _context.ProductCategoryTypes
                .ToListAsync();

            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data = items.Select(s => new { s.Id, s.Name, UpdatedAt = s.UpdatedAt.ToFriendlyPersianDateTextify() })
                .Skip((int)itemsPerPage * (page.Value - 1))
                .Take((int)itemsPerPage)
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ProductCategoryType productCategoryType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategoryType);
                await _context.SaveChangesAsync();
                return Json("ok");
            }
            return Json("no");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ProductCategoryType productCategoryType)
        {
            if (id != productCategoryType.Id)
            {
                return Json("404");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productCategoryType.UpdatedAt = DateTime.Now;
                    _context.Update(productCategoryType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryTypeExists(productCategoryType.Id))
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

            var productCategoryType = await _context.ProductCategoryTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategoryType == null)
            {
                return Json("404");
            }

            _context.ProductCategoryTypes.Remove(productCategoryType);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool ProductCategoryTypeExists(int id)
        {
            return _context.ProductCategoryTypes.Any(e => e.Id == id);
        }
    }
}
