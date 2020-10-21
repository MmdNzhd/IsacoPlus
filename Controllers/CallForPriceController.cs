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
using System.Threading;

namespace KaraYadak.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CallForPriceController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public CallForPriceController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        //[HttpPost]
        //[AllowAnonymous]
        public  IActionResult CallForPrice(string productCode)
        {
            //webScraping

            var url = _hostEnvironment.WebRootPath + "/Rar$EXa14112.24292";
            //var webscrap = new webScrapp();
            //var finalResult = webscrap.isacoWebscrap(productCode);
            IWebDriver m_driver;
            //m_driver = new ChromeDriver(@"C:\Users\MmdNzhd\AppData\Local\Temp\Rar$EXa0.659");
            m_driver = new ChromeDriver(url);

            m_driver.Url = "https://www.isaco.ir/main/auto-parts-list/auto-parts-list/";
            m_driver.Manage().Window.Maximize();
            IWebElement button;
            IWebElement finalTable;
            Thread.Sleep(100);


            m_driver.FindElement(By.Id("gdmCode")).SendKeys(productCode);
            Thread.Sleep(100);

            button = m_driver.FindElement(By.Id("post_form"));
            Thread.Sleep(100);

            button.Click();
            Thread.Sleep(100);
            finalTable = m_driver.FindElement(By.Id("result"));
            Thread.Sleep(2000);

            var innerHtml = finalTable.GetAttribute("innerHTML");
            m_driver.Close();


            return Json("<table class='table'>" + innerHtml + "</table>");
        }
        

      
    }
}
