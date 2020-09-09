using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class ProductUnitsController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;

        public ProductUnitsController(ApplicationDbContext context)
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

            var items = await _context.ProductUnits
                .ToListAsync();

            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data = items.Select(s => new
                {
                    s.Id,
                    s.Name,
                    UpdatedAt = s.UpdatedAt.ToFriendlyPersianDateTextify()
                })
                .Skip((int)itemsPerPage * (page.Value - 1))
                .Take((int)itemsPerPage)
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ProductUnit productUnit)
        {
            productUnit.CreatedAt = DateTime.Now;
            productUnit.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(productUnit);
                await _context.SaveChangesAsync();
                return Json("ok");
            }
            return Json("no");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productUnit = await _context.ProductUnits.FindAsync(id);
            if (productUnit == null)
            {
                return NotFound();
            }
            return View(productUnit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,Address,Status")] ProductUnit item)
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
                    if (!ProductUnitExists(item.Id))
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

            var item = await _context.ProductUnits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return Json("404");
            }

            _context.ProductUnits.Remove(item);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool ProductUnitExists(int id)
        {
            return _context.ProductUnits.Any(e => e.Id == id);
        }
    }
}