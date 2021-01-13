using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// مشتریان
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Customers()
        {

            return View();
        }
        /// <summary>
        /// گزارش خرید مشتری
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CustomerPurchaseReport()
        {

            return View();
        }
        /// <summary>
        /// مشتری-کالا
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CustomersWhitProduct()
        {

            return View();
        }
        /// <summary>
        /// گزارش خرید مشتریان
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CustomersPurchaseReport()
        {

            return View();
        }
        /// <summary>
        ///کالا-مشتری
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ProductWithCustomer()
        {

            return View();
        }
    }
}
