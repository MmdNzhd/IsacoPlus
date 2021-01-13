﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace KaraYadak.Controllers
{
    public class HomeSiteController : Controller
    {
        private readonly Random _random = new Random();
        private static readonly Random random = new Random();


        private readonly ApplicationDbContext _context;

        //private readonly IHostingEnvironment _hostingEnvironment;

        public HomeSiteController(ApplicationDbContext context)
        {
            _context = context;
            //_hostingEnvironment = hostingEnvironment;
        }

        [NonAction]
        static String removeDuplicate(string str)
        {
            var sArray = str.Split(",").ToList();
            var list = new List<string>();
            foreach (var item in sArray)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return String.Join(",", list);
        }
        [Route("GetSubCategories/{mainCategory?}")]
        public async Task<IActionResult>GetSubCategories(string mainCategory)
        {
            var subCategory = await _context.ProductCategoryTypes.Where(x => x.Name.Equals(mainCategory)).FirstOrDefaultAsync();
            if (subCategory != null)
            {
                var finalModel =await  _context.ProductCategories.Where(x => x.ProductCategoryType == subCategory.Id)
                    .Select(x=>x.Name).ToListAsync();

                return new JsonResult(new { status = 1, data = finalModel });
            }
            else
            {
                return new JsonResult(new { status = 0, message="دسته بندی یافت نشد."});
            }
        }
        public async Task<ActionResult> Index()
        {
          
           
            //var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
            ////add site visit
            //if(!await _context.SiteVisits.AnyAsync(x => x.Ip == ip && x.Date.Day == DateTime.Now.Day))
            //{
            //    var visit = new Models.SiteVisit()
            //    {
            //        Date = DateTime.Now,
            //        Ip = ip
            //    };
            //    await _context.SiteVisits.AddAsync(visit);
            //    await _context.SaveChangesAsync();

            //}
                

                
            //blog
            ViewBag.blogs = await _context.Blogs.OrderByDescending(x => x.CreateAt).Take(7).ToListAsync();

            //baner
            var baner =await  _context.Baners.OrderByDescending(x => x.CreateAt).FirstOrDefaultAsync();
            ViewBag.baner = baner;
            var now = DateTime.Now;
            if (baner.Date < now)
            {
                ViewBag.timer = now;
            }
            else
            {
                ViewBag.timer = baner.Date;
            }

            if (User.Identity.Name != null)
            {
                var user =await  _context.Users.SingleOrDefaultAsync(x => x.UserName.Equals(User.Identity.Name));
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.FirstName)) ViewBag.Username = user.PhoneNumber;
                    ViewBag.Username = user.FirstName + " " + user.LastName;
                }
                else ViewBag.Username = ":(";
            }
            ViewBag.Categories = await _context.ProductCategoryTypes.Where(x=>x.Id!=11&&x.Id!=5).ToListAsync();
            ViewBag.Brands =await _context.ProductCategories.Where(i => i.Parent != 0 && i.ProductCategoryType == 11).ToListAsync();

            ViewBag.Cars =await _context.ProductCategories.Where(i => i.Parent != 0 && i.ProductCategoryType == 5).ToListAsync();


            //var categories = new List<CategoriesVM>();
            //var categoryTypes = _context.ProductCategories.ToList();
            //foreach (var item in categoryTypes)
            //{
            //    var category = new CategoriesVM()
            //    {

            //        Id = item.Id,
            //        Picture = item.Image,
            //        Title = item.Name
            //    };
            //    categories.Add(category);

            //}

            //model.categories = categories;

            //product
            //**********************************************************************
            //var groupByCodeProduct = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)

            //                          group a by a.Code into pp
            //                          select new { 
            //                          Code=pp.Key,
            //                          Product=pp.ToList()
            //                          })
            var groupByCodeProduct = _context.Products.ToLookup(p => p.Code, p => new ProductForIndexVM
            {
                CreatingDate = p.CreatedAt,
                Title = p.Name,
                Code = p.Code,
                Off = p.Discount,
                Picture = p.ImageUrl,
                Price = p.Price,
                Special = p.SpecialSale,
                CategoryId=p.ProductCategoryType

            }).ToList();



            var products = new List<List<ProductForIndexVM>>();
            var maxDiscount = (from c in groupByCodeProduct
                               select c).Max(c => c.FirstOrDefault().Off);
            var bestOfferProduct = groupByCodeProduct.Where(x => x.FirstOrDefault().Off.Equals(maxDiscount)).Select(x => new ProductForIndexVM
            {
                Code = x.FirstOrDefault().Code,
                CreatingDate = x.FirstOrDefault().CreatingDate,
                Title = x.FirstOrDefault().Title,
                Off = x.FirstOrDefault().Off,
                Picture = (string.IsNullOrEmpty(x.FirstOrDefault().Picture)) ? "" :
                x.FirstOrDefault().Picture,
                Price = x.FirstOrDefault().Price - x.FirstOrDefault().Off * x.FirstOrDefault().Price / 100,
                //Rate = x.FirstOrDefault().Rate.GetValueOrDefault(),
            }).Take(1).ToList();

            var specialProducts = groupByCodeProduct.Where(x=>x.FirstOrDefault().Special).Select(x => new ProductForIndexVM
            {
                Code = x.FirstOrDefault().Code,
                Title = x.FirstOrDefault().Title,
                Off = x.FirstOrDefault().Off,
                Picture = (string.IsNullOrEmpty(x.FirstOrDefault().Picture)) ? "" :
                x.FirstOrDefault().Picture,
                Price = x.FirstOrDefault().Price - x.FirstOrDefault().Off * x.FirstOrDefault().Price / 100,
                Special = x.FirstOrDefault().Special,
                UpdateDate = x.FirstOrDefault().UpdateDate
                //Rate = x.FirstOrDefault().Rate.GetValueOrDefault(),
            }).OrderByDescending(x => x.UpdateDate).Take(10).ToList();
            var maxinDiscountProducts = groupByCodeProduct/*.Where()*/.Select(x => new ProductForIndexVM
            {
                Code = x.FirstOrDefault().Code,
                CreatingDate = x.FirstOrDefault().CreatingDate,
                Title = x.FirstOrDefault().Title,
                Off = x.FirstOrDefault().Off,
                Picture = (string.IsNullOrEmpty(x.FirstOrDefault().Picture)) ? "" :
                x.FirstOrDefault().Picture,
                Price = x.FirstOrDefault().Price - x.FirstOrDefault().Off * x.FirstOrDefault().Price / 100,
                //Rate = x.FirstOrDefault().Rate.GetValueOrDefault(),
            }).Where(e => e.Picture != null).OrderByDescending(x => x.Off).Take(10).ToList();
            var product = groupByCodeProduct/*.Where()*/.Select(x => new ProductForIndexVM
            {
                Code = x.FirstOrDefault().Code,
                CreatingDate = x.FirstOrDefault().CreatingDate,
                Title = x.FirstOrDefault().Title,
                Off = x.FirstOrDefault().Off,
                Picture = (string.IsNullOrEmpty(x.FirstOrDefault().Picture))? "" :
                x.FirstOrDefault().Picture,
                Price = x.FirstOrDefault().Price - x.FirstOrDefault().Off * x.FirstOrDefault().Price / 100,
                //Rate = x.FirstOrDefault().Rate.GetValueOrDefault(),
            }).Where(e => e.Picture != null).OrderByDescending(x => x.CreatingDate).Take(12).ToList();


            var finalmodel = new ListForIndexVM
            {
                First = bestOfferProduct,
                Second = specialProducts,
                Third = maxinDiscountProducts,
                Fourth = product
            };

            products.Add(bestOfferProduct);
            products.Add(specialProducts);
            products.Add(maxinDiscountProducts);
            products.Add(product);

            return View(finalmodel);
        }

    
        public PartialViewResult SpecialOfferProduct()
        {

            return PartialView();
        }
        public PartialViewResult maxinDiscountProducts()
        {

            return PartialView();
        }
        public PartialViewResult OtherProducts()
        {

            return PartialView();
        }
        public PartialViewResult Header()
        {

            return PartialView();
        }

        [Route("ContactUs")]
        public IActionResult ContactUs()
        {
            return View();
        }
      

        [HttpPost]
        public async Task<IActionResult> AddContactusMessage(ContactUsMessage contactUs)

        {
            var ok = _context.contactUsMessages.ToList();
            if (ModelState.IsValid)
            {
                try
                {
                    contactUs.CreateAt = DateTime.Now;
                    await _context.contactUsMessages.AddAsync(contactUs);
                    _context.SaveChanges();
                    return Json(new { status = "1", message = " نظر شما با موفقیت ثبت شده است." });
                }
                catch (Exception ex)
                {

                    return Json(new { status = "0", message = ex.Message });
                }
            }
            return Json(new { status = "0", message = "خطایی رخ داده است لطفا مجددا امتحان کنید." });
        }

    }
}