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
        public async Task<IActionResult> CallForPrice(string productCode)
        {
            //webScraping

            IWebDriver m_driver;
            m_driver = new ChromeDriver(@"C:\Users\MmdNzhd\AppData\Local\Temp\Rar$EXa0.659");
            m_driver.Url = "https://www.isaco.ir/main/auto-parts-list/auto-parts-list/";
            m_driver.Manage().Window.Maximize();
            IWebElement button;
            IWebElement finalTable;


            m_driver.FindElement(By.Id("gdmCode")).SendKeys("1460100412");

            button = m_driver.FindElement(By.Id("post_form"));

            button.Click();
            //m_driver.Close();
            // price_table
            finalTable = m_driver.FindElement(By.ClassName("price_table"));
            return (IActionResult)finalTable;
        }
        

      
    }
}
