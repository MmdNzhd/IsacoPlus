using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using KaraYadak.Data;
using KaraYadak.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(new DashboardViewModel
            {
                TotalClients = await _context.Clients.Where(i => i.Status == ClientStatus.فعال).CountAsync(),
                TotalAvailableProducts = await _context.Products.Where(i=>i.ProductStatus == ProductStatus.دردسترس).CountAsync(),
                TotalForms = 0,//await _context.WarehouseRecipts.CountAsync() + await _context.WarehouseRemittances.CountAsync() + await _context.WarehouseToWarehouses.CountAsync(),
                TotalProducts = await _context.Products.CountAsync()
            });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
