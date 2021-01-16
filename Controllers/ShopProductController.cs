using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core.IranCities;
using Parbad;
using System.Net.Http;

namespace KaraYadak.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class ShopProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOnlinePayment _onlinePayment;
        private readonly IHttpClientFactory _httpClientFactory;

        public ShopProductController(ApplicationDbContext context, IOnlinePayment onlinePayment, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _onlinePayment = onlinePayment;
            _httpClientFactory = httpClientFactory;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuiopasdfghjklzxcvbnm";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static List<string> removeDuplicateForId(string[] str)
        {
            var sArray = str;
            var list = new List<string>();
            foreach (var item in sArray)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return list;
        }
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

        public IActionResult shopBasket()
        {
            ViewBag.Provinces = Iran.Provinces.ToList();
            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
            ViewBag.profileIsComplete = false;
            if (item != null && !string.IsNullOrWhiteSpace(item.Email))
            {
                ViewBag.profileIsComplete = true;
            }
            if (item != null)
            {
                var vm = new ProfileVM
                {
                    Address = item.Address,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Phone = item.Phone,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    NationalCode = item.NationalCode,
                    CartNumber = item.CartNumber,
                    Gender = item.Gender,
                    ImageProfile = item.AvatarUrl,
                    Province = item.Province,
                    PostalCode = item.PostalCode,
                    City = item.City,
                };
                return View(vm);
            }
            return View(new ProfileVM()
            {
                Address = "",
                FirstName = "",
                LastName = "",
                Phone = "",
                PhoneNumber = "",
                Email = "",
                NationalCode = "",
                CartNumber = "",
                Gender = "",
                ImageProfile = "",
                City = "",
                CallbackUrl = "",
                PostalCode = "",
                Province = ""
            });

        }
        public async Task<JsonResult> GetProductsForBasket(string[] items)
        {


            var products = new List<ShopProductsVM>();
            var NewItems = removeDuplicateForId(items);
            foreach (var item in NewItems)
            {
                var code = item;
                try
                {
                    var product = await (from p in _context.Products.Where(x => x.Code.Equals(code))
                                         join c in _context.ProductCategories.DefaultIfEmpty() on p.CategoryIdLvl1 equals c.Id
                                         into cpTbles
                                         from cp in cpTbles.DefaultIfEmpty()
                                         join ct in _context.ProductCategoryTypes
                                         on cp.ProductCategoryType equals ct.Id
                                         into table2
                                         from t in table2.DefaultIfEmpty()
                                         where t.Id.Equals(11)
                                         select new ShopProductsVM
                                         {
                                             Id = p.Id,
                                             Title = p.Name,
                                             Brand = cp.Name ?? "بدون برند",
                                             Picture = p.ImageUrl,
                                             Price = p.Price,
                                             Code = p.Code,
                                             Off = p.Discount
                                         }
                                   ).FirstOrDefaultAsync();
                    products.Add(product);

                }
                catch (Exception e)
                {

                    return new JsonResult(new { message = e.Message });
                }

            }
            return new JsonResult(new
            {
                result = products
            });
        }

        public async Task<IActionResult> Cart()
        {
            ViewBag.P = @Request.Headers["Referer"].ToString();
            var categories = await _context.ProductCategories.ToListAsync();
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);





            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            if (user != null)
            {
                if (user.PhoneNumber != null)
                {
                    vm.IsProfileCompelete = true;
                }
            }
            var cart = Request.Cookies["tempcart"];
            if (cart != null)
            {
                vm.ProductIds = cart.Split("_").Select(x => x.Split("-")[0]).ToList();
                vm.Count = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
                var products = _context.Products.Where(x => vm.ProductIds.Contains(x.Code)).ToLookup(p => p.Code, p => new ProductWithMeterForFactorVM
                {
                    Name = p.Name,
                    Code = p.Code,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Id = p.Id,

                }).ToList();
                vm.Products = new List<ProductWithMeterForFactorVM>();
                foreach (IGrouping<string, ProductWithMeterForFactorVM> item in products)
                {
                    var x = item.FirstOrDefault();
                    vm.Products.Add(item.FirstOrDefault());

                }
                foreach (var item in vm.ProductIds)
                {
                    int count = vm.Count.ElementAt(vm.ProductIds.IndexOf(item));
                    vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault().Count = count;
                    var pr = vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault();
                    vm.Price += (pr.Price * count) - ((pr.Discount * pr.Price / 100) * pr.Count);
                }
                vm.SendPrice = (sendPrice != null) ? int.Parse(sendPrice.Value) : 25000;
                vm.TotalPrice = vm.Price + vm.SendPrice;
            }
            return PartialView(vm);
        }
        [Authorize]
        public async Task<IActionResult> SubmitBasket(PostType PostType)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return new JsonResult(new { status = 0, message = "پروفایل خود را تکمیل کنید" });
            }
            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            var cart = Request.Cookies["tempcart"];
            var listOfFactorItems = new List<CartItem>();
            if (cart != null)
            {
                vm.ProductIds = cart.Split("_").Select(x => x.Split("-")[0]).ToList();
                vm.Count = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
                var products = _context.Products.Where(x => vm.ProductIds.Contains(x.Code)).ToLookup(p => p.Code, p => new ProductWithMeterForFactorVM
                {
                    Name = p.Name,
                    Code = p.Code,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Id = p.Id,

                }).ToList();
                vm.Products = new List<ProductWithMeterForFactorVM>();
                foreach (IGrouping<string, ProductWithMeterForFactorVM> item in products)
                {
                    var x = item.FirstOrDefault();
                    vm.Products.Add(item.FirstOrDefault());

                }
                foreach (var item in vm.ProductIds)
                {
                    int count = vm.Count.ElementAt(vm.ProductIds.IndexOf(item));
                    vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault().Count = count;
                    var pr = vm.Products.Where(x => x.Code.Equals(item)).SingleOrDefault();
                    var factorItem = new CartItem()
                    {
                        ProductId = pr.Id,
                        ProductName = pr.Name,
                        ProductCode = pr.Code,
                        Price = ((pr.Price) - (pr.Discount * pr.Price / 100)).ToString(),
                        UserName = user.UserName,
                        Date = DateTime.Now,
                        Quantity = count,
                        Discount=(pr.Discount * pr.Price / 100).ToString(),
                    };
                    listOfFactorItems.Add(factorItem);
                    vm.Price += (pr.Price * count) - ((pr.Discount * pr.Price / 100) * pr.Count);
                    vm.DiscountPrice += (pr.Discount * pr.Price / 100) * pr.Count;

                }
                vm.SendPrice = (sendPrice != null) ? int.Parse(sendPrice.Value) : 25000;
                vm.TotalPrice = vm.Price + vm.SendPrice;
            }

            else return new JsonResult(new { status = 0, message = "مشکلی در سبد خرید شما به وجود آمده است" });
            var factor = new ShoppingCart()
            {
                UserName = user.UserName,
                Date = DateTime.Now,
                Price = vm.Price.ToString(),
                Address = user.Address,
                CartItems = listOfFactorItems,
                DiscountPercent = vm.DiscountPrice.ToString(),
                Name = user.FirstName + " " + user.LastName,
                PaymentType = PaymentType.Online,
                Phone = user.Phone,
                PhoneNumber = user.PhoneNumber,
                RequestCode = RandomString(5),
                SendPrice = vm.SendPrice.ToString(),
                Status = RequestStatus.Confirmed,
                PostType= PostType

            };
            await _context.CartItems.AddRangeAsync(listOfFactorItems);
            await _context.ShoppingCarts.AddRangeAsync(factor);
            await _context.SaveChangesAsync();



            #region Connect Banking portal

            var error = new List<string>();

            var shoppingCarts = await _context.ShoppingCarts.FindAsync(factor.Id);
            if (shoppingCarts == null)
            {
                return new JsonResult(new { status = 0, message = "  فاکتوری یافت نشد" });
            }


            var callbackUrl = Url.Action("Verify", "ShopProduct", new { shoppingCarts = factor.Id }, Request.Scheme);
            var price = factor.Price;
            var discount = Convert.ToDecimal(factor.DiscountPercent);
            var finallyPriceWithTax = Convert.ToDecimal(price) - discount + Convert.ToDecimal(factor.SendPrice);


            //sum discount in plan and ehrn discount from plan


            var payment = new Payment
            {
                Amount = Convert.ToDecimal(price),
                Discount = discount,
                FinallyAmountWithTax = finallyPriceWithTax,
                Date = DateTime.Now,
                UserId = user.Id,
                ShoppingCartId = factor.Id,
                PostType= PostType

            };

            var result = await _onlinePayment.RequestAsync(invoice =>
            {
                invoice
                    .SetTrackingNumber(DateTime.Now.Ticks)
                    .SetAmount(payment.FinallyAmountWithTax)
                    .SetCallbackUrl(callbackUrl)
                    .SetGateway(Gateways.ParbadVirtual.ToString());
            });

            // save result in db
            payment.TrackingNumber = result.TrackingNumber.ToString();

            if (result.IsSucceed)
            {
                payment.ErrorDescription = result.Message;
                _context.Update(payment);
                await _context.SaveChangesAsync();

                return new JsonResult(new { status = 1, message = "با موفقیت انجام شد", data = result });

            }
            else
            {
                payment.ErrorDescription = result.Message;
                _context.Update(payment);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = 0, message = "خطا در اتصال به درگاه" });

            }

            #endregion
        }





        #region Verify
        [AllowAnonymous]
        public async Task<IActionResult> Verify()
        {

            try
            {
                var invoice = await _onlinePayment.FetchAsync();


                var result = await _onlinePayment.VerifyAsync(invoice);
                var payment = _context.Payments.FirstOrDefault(x => x.TrackingNumber == result.TrackingNumber.ToString());
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == payment.UserId);
                if (user == null)
                {
                    return new JsonResult(new { status = 0, message = "کاربر نامعتبر" });
                }

                if (result.Amount == payment.FinallyAmountWithTax && result.IsSucceed == true)
                {

                    payment.IsSucceed = true;
                    payment.TransactionCode = result.TransactionCode;
                    payment.ErrorDescription = result.Message;
                    _context.Update(payment);


                    var transaction = new Transaction()
                    {
                        FinallyAmountWithTax = result.Amount,
                        Amount = payment.Amount,
                        DateTime = DateTime.Now,
                        Discount = payment.Discount,
                        Information = result.Message,
                        PaymentId = payment.Id,
                        TransactionCode = result.TransactionCode,
                        IsSucceed = true,
                    };
                    await _context.Transactions.AddAsync(transaction);


                    await _context.SaveChangesAsync();
                    var model = new DargahViewModel
                    {
                        IsSuccess = true,
                        TrackingNumber = payment.TrackingNumber
                    };
                    return View(model);


                }
                else
                {
                    payment.IsSucceed = false;
                    payment.TransactionCode = result.TransactionCode;
                    payment.ErrorDescription = result.Message;
                    _context.Update(payment);
                    _context.Update(payment);
                    await _context.SaveChangesAsync();

                    var model = new DargahViewModel
                    {
                        IsSuccess = true,
                        TrackingNumber = payment.TrackingNumber
                    };
                    return View(model);

                }
            }
            catch (Exception ex)
            {
                var model = new DargahViewModel
                {
                    IsSuccess = true,
                    TrackingNumber = "خطایی رخ داده است"
                };
                return View(model);

            }
            // save result in db

        }
        #endregion


        [Route("GetFactorDetails")]
        public async Task<IActionResult> GetFactorDetails(int id)
        {
            var categories = await _context.ProductCategories.ToListAsync();
            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);





            var sendPrice = await _context.Settings.FirstOrDefaultAsync(x => x.Key == "SendPrice");
            var vm = new SiteCartVM();
            if (user != null)
            {
                if (user.PhoneNumber != null)
                {
                    vm.IsProfileCompelete = true;
                }
            }
            //var cart = Request.Cookies["tempcart"];
            //if (cart != null)
            //{
            //    vm.ProductIds = cart.Split("_").Select(x => x.Split("-")[0]).ToList();
            //    vm.Count = cart.Split("_").Select(x => int.Parse(x.Split("-")[1])).ToList();
            vm.ProductIds = new List<string>();
            vm.Count = new List<int>();
            var factor = await _context.ShoppingCarts.Where(x => x.Id == id).Include(x => x.CartItems).Select(x => x.CartItems).FirstOrDefaultAsync();
            foreach (var item in factor)
            {
                vm.ProductIds.Add(item.ProductId.ToString());
                vm.Count.Add(Convert.ToInt32(item.Quantity));
                ViewBag.FactorDate = item.Date;
                //    vm.Count = 
            }


            var products = _context.Products.Where(x => vm.ProductIds.Contains(x.Id.ToString())).ToLookup(p => p.Code, p => new ProductWithMeterForFactorVM
            {
                Name = p.Name,
                Code = p.Code,
                Discount = p.Discount,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                Id = p.Id,

            }).ToList();

            vm.Products = new List<ProductWithMeterForFactorVM>();
            foreach (IGrouping<string, ProductWithMeterForFactorVM> item in products)
            {
                var x = item.FirstOrDefault();
                vm.Products.Add(item.FirstOrDefault());

            }

            foreach (var item in vm.ProductIds)
            {
                int count = vm.Count.ElementAt(vm.ProductIds.IndexOf(item));
                vm.Products.Where(x => x.Id.Equals(int.Parse(item))).FirstOrDefault().Count = count;
                var pr = vm.Products.Where(x => x.Id.Equals(int.Parse(item))).SingleOrDefault();
                vm.Price += (pr.Price * count) - ((pr.Discount * pr.Price / 100) * pr.Count);
            }
            vm.SendPrice = (sendPrice != null) ? int.Parse(sendPrice.Value) : 25000;
            vm.TotalPrice = vm.Price + vm.SendPrice;
            //}
            return PartialView(vm);
        }

    }
}
