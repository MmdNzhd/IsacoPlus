using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = PublicHelper.ADMINROLE)]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportsController(ApplicationDbContext context, IAccountService accountService , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _accountService = accountService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> GetUserInfoForReport(string userId)
        {
            var result = await _accountService.GetUserInfoForReport(userId);
            if (result.isSuccess)
            {
                return Json(new { status = 1, message = "با موفقیت اتجام شد", data = result.model });
            }
            else
            {
                return Json(new { status = 0, message = result.error });

            }
        }


        public async Task<IActionResult> GetCustomerPurchaseReport(string userId, string startDate, string endDate)
        {

            var result = await _accountService.GetCustomerPurchaseReport(userId, startDate, endDate);
            if (result.isSuccess)
            {
                return Json(result.model);
            }
            else
            {
                return Json(new { status = 0, message = result.error });

            }
        }
        public async Task<IActionResult> CustomersWhitProductReport(string userId=null, string productCode = null, string startDate = null, string endDate = null)
        {

       
            var result = await _accountService.CustomersWhitProductReport(userId, productCode,startDate, endDate);
            if (result.isSuccess)
            {
                return Json(result.model);
            }
            else
            {
                return Json(new { status = 0, message = result.error });

            }
        }

        public async Task<IActionResult> GetCustomersPurchaseReport(string userId = null, string productCode = null, string startDate = null, string endDate = null)
        {


            var result = await _accountService.GetCustomersPurchaseReport(userId, productCode, startDate, endDate);
            if (result.isSuccess)
            {
                return Json(result.model);
            }
            else
            {
                return Json(new { status = 0, message = result.error });

            }
        }


        public async Task<IActionResult> GetProductWhitCustomersReport(string userId = null, string productCode = null, string startDate = null, string endDate = null)
        {


            var result = await _accountService.GetProductWhitCustomersReport(userId, productCode, startDate, endDate);
            if (result.isSuccess)
            {
                var allUser = await _context.Users.Select(x => new CustomerWithBuyCount
                {
                    BuyCount = "0",
                    CustomerId = x.Id,
                    CustomerName = x.FirstName + " " + x.LastName
                }).OrderBy(x=>x.CustomerId).ToListAsync();

                string rootFolder = _webHostEnvironment.WebRootPath;
                string fileName = @"ExportCustomers.xlsx";

                FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));

                using (ExcelPackage package = new ExcelPackage(file))
                {

                    var finalModel = result.model;

                    if (package.Workbook.Worksheets.Any(x=>x.Name== "Customer"))
                    {
                        package.Workbook.Worksheets.Delete("Customer");
                    }
                   


                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Customer");
                    if (package.Workbook.Worksheets.Any(x => x.Name == "Sheet1"))
                    {
                        package.Workbook.Worksheets.Delete("Sheet1");
                    }
                    ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("Sheet1");
                    int totalRows = finalModel.Count();

                    worksheet.Cells[1, 1].Value = "Product Code";
                    worksheet.Cells[1, 2].Value = "Product Name ";

                    for (int j = 0; j < allUser.Count; j++)
                    {
                        worksheet.Cells[1, j+3].Value = allUser[j].CustomerName;
                    }
                    int i = 0;
                    for (int row = 2; row <= totalRows + 1; row++)
                    {
                        worksheet.Cells[row, 1].Value = finalModel[i].ProductCode;
                        worksheet.Cells[row, 2].Value = finalModel[i].ProductName;

                        for (int p = 0; p < allUser.Count-1; p++)
                        {
                            worksheet.Cells[row, p+3].Value = finalModel[i].CustomerWithBuyCounts[p].BuyCount;

                        }
                        i++;
                    }
                    if (package.Workbook.Worksheets.Any(x => x.Name == "Sheet1"))
                    {
                        package.Workbook.Worksheets.Delete("Sheet1");
                    }
                    package.Save();


                }

                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            else
            {
                return Json(new { status = 0, message = result.error });

            }
        }
        
        /// <summary>
        /// مشتریان
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("Customers")]
        public async Task<IActionResult> Customers()
        {

            return View();
        }
        /// <summary>
        /// گزارش خرید مشتری
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("CustomerPurchaseReport")]
        //[AllowAnonymous]
        public async Task<IActionResult> CustomerPurchaseReport()
        {

            ViewBag.UserList = await (from r in _context.Roles
                                      where r.Name != PublicHelper.ADMINROLE && r.Name != PublicHelper.WarehousingAdminROLE
                                      join ur in _context.UserRoles
                                      on r.Id equals ur.RoleId
                                      join u in _context.Users
                                      on ur.UserId equals u.Id
                                      select new UserList { Fullname = u.FirstName + " " + u.LastName, Id = u.Id }).ToListAsync();

            return View();
        }
        public class UserList
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
        }
        public class ProductList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
        /// <summary>
        /// مشتری-کالا
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("CustomersWhitProductReport")]
        public async Task<IActionResult> CustomersWhitProductReport()
        {
            ViewBag.UserList = await (from r in _context.Roles
                                      where r.Name != PublicHelper.ADMINROLE && r.Name != PublicHelper.WarehousingAdminROLE
                                      join ur in _context.UserRoles
                                      on r.Id equals ur.RoleId
                                      join u in _context.Users
                                      on ur.UserId equals u.Id
                                      select new UserList { Fullname = u.FirstName + " " + u.LastName, Id = u.Id }).ToListAsync();

            var groupByCodeProduct = _context.Products.ToLookup(p => p.Code, p => new ProductList
            {
                Code = p.Code,
                Name=p.Name

            }).ToList();
           ViewBag.ProductList = groupByCodeProduct.Select(x => new ProductList
           {
                Code = x.Key.Trim(),
              Name=x.FirstOrDefault().Name
            }).ToList();

            return View();

        }
        /// <summary>
        /// گزارش خرید مشتریان
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("CustomersPurchaseReport")]
        public async Task<IActionResult> CustomersPurchaseReport()
        {

            ViewBag.UserList = await (from r in _context.Roles
                                      where r.Name != PublicHelper.ADMINROLE && r.Name != PublicHelper.WarehousingAdminROLE
                                      join ur in _context.UserRoles
                                      on r.Id equals ur.RoleId
                                      join u in _context.Users
                                      on ur.UserId equals u.Id
                                      select new UserList { Fullname = u.FirstName + " " + u.LastName, Id = u.Id }).ToListAsync();

            var groupByCodeProduct = _context.Products.ToLookup(p => p.Code, p => new ProductList
            {
                Code = p.Code,
                Name = p.Name

            }).ToList();
            ViewBag.ProductList = groupByCodeProduct.Select(x => new ProductList
            {
                Code = x.Key.Trim(),
                Name = x.FirstOrDefault().Name
            }).ToList();

            return View();
        }
        /// <summary>
        ///کالا-مشتری
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("ProductWithCustomer")]
        public async Task<IActionResult> ProductWithCustomer()
        {


            ViewBag.UserList = await (from r in _context.Roles
                                      where r.Name != PublicHelper.ADMINROLE && r.Name != PublicHelper.WarehousingAdminROLE
                                      join ur in _context.UserRoles
                                      on r.Id equals ur.RoleId
                                      join u in _context.Users
                                      on ur.UserId equals u.Id
                                      select new UserList { Fullname = u.FirstName + " " + u.LastName, Id = u.Id }).OrderBy(x=>x.Fullname).ToListAsync();

            var groupByCodeProduct = _context.Products.ToLookup(p => p.Code, p => new ProductList
            {
                Code = p.Code,
                Name = p.Name

            }).ToList();
            ViewBag.ProductList = groupByCodeProduct.Select(x => new ProductList
            {
                Code = x.Key.Trim(),
                Name = x.FirstOrDefault().Name
            }).ToList();

            return View();
        }
    }
}
