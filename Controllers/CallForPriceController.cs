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
using KaraYadak.Services;
using Newtonsoft.Json;
using AngleSharp.Html.Parser;
using System.Collections.Generic;

namespace KaraYadak.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CallForPriceController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ISmsSender _sms;

        public CallForPriceController(IWebHostEnvironment hostEnvironment,ISmsSender sms)
        {
            _hostEnvironment = hostEnvironment;
            _sms = sms;
        }

        //[HttpPost]
        //[AllowAnonymous]
        public JsonResult CallForPrice(string productCode)
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
            Thread.Sleep(1000);

            var innerHtml = finalTable.GetAttribute("innerHTML");
            m_driver.Close();


            return Json("<table class='table'>" + innerHtml + "</table>");
        }


        public async Task<IActionResult> CallForPriceWithSms(string from, string to, string message)
        {
            message = message.ToEnglishNumbers();

            HtmlParser parser = new HtmlParser();
            var doc = parser.ParseDocument(CallForPrice(message).Value.ToString());

            if (!CallForPrice(message).Value.ToString().Contains("td"))
            {
                //

                var smsRes2 = await _sms.SendWithPattern(from, "ep59ztf8gf", JsonConvert.SerializeObject(new { code = message }));
                return Json("nok");


            }

            //return Json(CallForPrice(message).Value.ToString());



            var tds = new List<string>();
            var tr = doc.All.Last(tag => tag.LocalName == "tr" /*&& tag.GetAttribute("class") == "table"*/);
            var td = tr.Children;
            foreach (var item in td)
            {
                tds.Add(item.InnerHtml);
            }


            string path = _hostEnvironment.WebRootPath + "/Log.txt";

            using (StreamWriter sw = System.IO.File.AppendText(path))
            {
                sw.WriteLine(from);
                sw.WriteLine(to);
                sw.WriteLine(message);
                sw.WriteLine("-------------------------------------------");
            }
           
            var smsData = new
            {
                code =tds[0] ,
                name = tds[1],
                price= tds[3]
            };
            var smsRes = await _sms.SendWithPattern(from, "ih82g2tlpa", JsonConvert.SerializeObject(smsData));



            return Json("Ok");

        }
       

    }
}
