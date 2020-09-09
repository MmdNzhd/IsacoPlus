using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.Include(i => i.Parent).Include(i => i.CategoryType).Include(i => i.Childs).ToListAsync();
            var categoryTypes = await _context.CategoryTypes.ToListAsync();
            var model = new CategoriesPageViewModel
            {
                CategoryTypes = categoryTypes,
                Categories = categories
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryTypesList()
        {
            var items = await _context.CategoryTypes//.Include(i=>i.Categories)
                .ToListAsync();
            return Json(new { data = items });
        }

        [HttpPost]
        public async Task<IActionResult> CategoriesList(int? type)
        {
            var items = await _context.Categories.Include(i => i.Parent).Include(i => i.CategoryType)
                .ToListAsync();

            if (type.HasValue && type.Value > 0)
            {
                items = items.Where(i => i.CategoryType.Id == type.Value).ToList();
            }

            return Json(new { data = items });
        }

        public IActionResult AllCategories()
        {
            return View();
        }
    }
}
