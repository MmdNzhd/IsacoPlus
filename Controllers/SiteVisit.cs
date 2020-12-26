using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Controllers
{
    public class SiteVisit : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteVisit(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("SiteVisit")]
        public async Task<IActionResult> Index()
        {
            var allVisitCount = await _context.SiteVisits.CountAsync();
            var todayVisit = await _context.SiteVisits.Where(x => x.Date.Day == DateTime.Now.Day).Select(x => x.Ip).ToListAsync();
            var todayVisitCount = todayVisit.Count;
            return new JsonResult(new { allVisit = allVisitCount, todayVisit = todayVisit, todayVisitCount = todayVisitCount });


        }
    }
}
