using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebScarpinInCSharp
{
   
    public class webScrapp 
    {
        public string isacoWebscrap(string productCode)
        {
            IWebDriver m_driver;
            m_driver = new ChromeDriver(@"C:\Users\MmdNzhd\AppData\Local\Temp\Rar$EXa0.659");
            //m_driver = new ChromeDriver(@"C:\Users\Administrator\AppData\Local\Temp\2\Rar$EXa8844.29816");

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

            return "<table class='table'>"+innerHtml+ "</table>";
        }
    }
}
