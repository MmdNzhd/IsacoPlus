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
using Microsoft.AspNetCore.Http;
using System.Globalization;
using KaraYadak.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScarpinInCSharp;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CallForPriceController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public CallForPriceController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        //[Route("CallForPrice")]
        [AllowAnonymous]
        public  IActionResult CallForPrice(string productCode)
        {
            //webScraping


            var webscrap = new webScrapp();
            var finalResult = webscrap.isacoWebscrap(productCode);

            return Json(finalResult);
        }
        

      
    }
}
