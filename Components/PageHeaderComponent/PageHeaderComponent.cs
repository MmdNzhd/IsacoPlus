using KaraYadak.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Components
{
    public class PageHeaderComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PageHeaderComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string Page, string Parent, string ParentUrl)
        {
            return View(nameof(PageHeaderComponent), new PageHeaderComponentVM { Page = Page, Parent = Parent, ParentUrl = ParentUrl });
        }
    }
    public class PageHeaderComponentVM
    {
        public string Page { get; set; }
        public string Parent { get; set; }
        public string ParentUrl { get; set; }
    }
}
