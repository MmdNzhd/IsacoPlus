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
    public class WarehousesController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;

        public WarehousesController(ApplicationDbContext context)
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

            var items = await _context.Warehouses
                .ToListAsync();

            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data = items.Select(s => new { s.Id, s.Name, s.Code, UpdatedAt = s.UpdatedAt.ToFriendlyPersianDateTextify() })
                .Skip((int)itemsPerPage * (page.Value - 1))
                .Take((int)itemsPerPage)
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Warehouse warehouse)
        {
            warehouse.Code = Guid.NewGuid().ToString();
            warehouse.CreatedAt = DateTime.Now;
            warehouse.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return Json("ok");
            }
            return Json("no");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json("404");
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return Json("404");
            }
            return Json(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return Json("404");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    warehouse.UpdatedAt = DateTime.Now;
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.Id))
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

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return Json("404");
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }
    }
}