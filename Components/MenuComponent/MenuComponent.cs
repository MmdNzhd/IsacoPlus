using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;

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
        private async Task<List<MenuComponentVM>> GetItemsAsync()
        {
            return await _context.Menu
                .Select(s => new MenuComponentVM
                {
                    Icon = s.Icon.Length > 0 ? s.Icon : "feather icon-circle",
                    Title = s.Name,
                    Url = s.Url
                })
                .ToListAsync();
        }
    }
    public class MenuComponentVM
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
