//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DNTPersianUtils.Core.IranCities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using KaraYadak.Data;
//using KaraYadak.ViewModels;
//using KaraYadak.Models;
//using DNTPersianUtils.Core;

//namespace Zarpoosh.Controllers
//{
//    public class WebappController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IHostingEnvironment hostingEnvironment;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private static Random random = new Random();
//        public static string RandomString(int length)
//        {
//            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//            return new string(Enumerable.Repeat(chars, length)
//              .Select(s => s[random.Next(s.Length)]).ToArray());
//        }
//        static string ConvertStringArrayToString(string[] array)
//        {
//            // Concatenate all the elements into a StringBuilder.
//            StringBuilder builder = new StringBuilder();
//            var i = 0;
//            foreach (string value in array)
//            {
//                i++;
//                builder.Append(value);
//                if (i != array.Length)
//                {
//                    builder.Append('_');
//                }
//            }
//            return builder.ToString();
//        }
//        static String removeDuplicate(string str)
//        {
//            var sArray = str.Split(",").ToList();
//            var list = new List<string>();
//            foreach (var item in sArray)
//            {
//                if (!list.Contains(item))
//                    list.Add(item);
//            }
//            return String.Join(",", list);
//        }
//        public WebappController(ApplicationDbContext context, IHostingEnvironment hosting, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
//        {
//            _context = context;
//            hostingEnvironment = hosting;
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }
//        //Index Page
//        public async Task<IActionResult> Index()
//        {
//            var user = _context.Users.SingleOrDefault(x => x.UserName == "Admin");
//            await _signInManager.SignInAsync(user, true);
//            if (Request.Cookies["cart"] != null)
//            {
//                var cookie = Request.Cookies["cart"].Split("_");
//                var list = new List<string>();
//                //return Json(cookie);
//                foreach (var item in cookie)
//                {
//                    list.Add(item.Split("-")[0]);
//                }
//                var s = ConvertStringArrayToString(list.ToArray());
//                ViewBag.Cookie = s;
//            }
//            else
//            {
//                ViewBag.Cookie = "";
//            }
//            //var products = await _context.Products.Where(x => x.ImageUrl != null && x.ProductStatus == ProductStatus.آماده_برای_فروش).ToListAsync();
//            var products = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
//                         .Join(_context.ProductCategories,
//                     ac => ac.CategoryIdLvl1,
//                     cc => cc.Id,
//                     (ac, cc) => new
//                     {
//                         ac,
//                         cc
//                     }
//                     ).ToList()
//                         group a by a.ac.Code into pp
//                         select new ProductWithCategoryVM
//                         {
//                             Product = pp.FirstOrDefault().ac,
//                             Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
//                         })
//                    .ToList();

//            var favorites = new List<string>();
//            if (User.Identity.Name != null)
//            {
//                var f = _context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name).Favorites;
//                if (f != null)
//                {
//                    favorites =f.Split("_").ToList();
//                }
//            }
//            var types = await _context.ProductCategoryTypes.ToListAsync();
//            var setting = await _context.Settings.ToListAsync();
//            var vm = new LandingViewModel
//            {
//                Slider_1 = setting.SingleOrDefault(x => x.Key == "Slider_1").Value,
//                Slider_2 = setting.SingleOrDefault(x => x.Key == "Slider_2").Value,
//                Slider_3 = setting.SingleOrDefault(x => x.Key == "Slider_3").Value,
//                CategoryTypes = types,
//                Services_1 = setting.SingleOrDefault(x => x.Key == "Services_1").Value,
//                Services_2 = setting.SingleOrDefault(x => x.Key == "Services_2").Value,
//                Services_3 = setting.SingleOrDefault(x => x.Key == "Services_3").Value,
//                Services_4 = setting.SingleOrDefault(x => x.Key == "Services_4").Value,
//                Services_1_Img = setting.SingleOrDefault(x => x.Key == "Services_1_Img").Value,
//                Services_2_Img = setting.SingleOrDefault(x => x.Key == "Services_2_Img").Value,
//                Services_3_Img = setting.SingleOrDefault(x => x.Key == "Services_3_Img").Value,
//                Services_4_Img = setting.SingleOrDefault(x => x.Key == "Services_4_Img").Value,
//                Cat_1 = setting.SingleOrDefault(x => x.Key == "Cat_1").Value,
//                Cat_2 = setting.SingleOrDefault(x => x.Key == "Cat_2").Value,
//                Cat_3 = setting.SingleOrDefault(x => x.Key == "Cat_3").Value,
//                Cat_4 = setting.SingleOrDefault(x => x.Key == "Cat_4").Value,
//                Product_Slider = setting.SingleOrDefault(x => x.Key == "Product_Slider").Value,
//                Product_Slider_2 = setting.SingleOrDefault(x => x.Key == "Product_Slider_2").Value,
//                Products = products,
//                Products2 = products,
//                Specials = products.Where(x => x.Product.SpecialSale == true).ToList(),
//                Favorites = favorites
//            };
//            return View(vm);
//        }

//        public IActionResult Search(string key)
//        {
//            if (Request.Cookies["cart"] != null)
//            {
//                var cookie = Request.Cookies["cart"].Split("_");
//                var list = new List<string>();
//                //return Json(cookie);
//                foreach (var item in cookie)
//                {
//                    list.Add(item.Split("-")[0]);
//                }
//                var s = ConvertStringArrayToString(list.ToArray());
//                ViewBag.Cookie = s;
//            }
//            else
//            {
//                ViewBag.Cookie = "";
//            }
//            var favorites = new List<string>();
//            if (User.Identity.Name != null)
//            {
//                var f = _context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name).Favorites;
//                if (f != null)
//                {
//                    favorites = f.Split("_").ToList();
//                }

//            }
//            ViewBag.Fav = favorites;
//            var products = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
//                     .Join(_context.ProductCategories,
//                 ac => ac.CategoryIdLvl1,
//                 cc => cc.Id,
//                 (ac, cc) => new
//                 {
//                     ac,
//                     cc
//                 }
//                 ).ToList()
//                            group a by a.ac.Code into pp
//                            select new ProductWithCategoryVM
//                            {
//                                Product = pp.FirstOrDefault().ac,
//                                Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
//                            })
//                .ToList();

//            var items = products.Where(x => x.Product.Name.Contains(key) || x.Categories.Contains(key)).ToList();
//            return View(items);
//        }
//        public IActionResult ChangeLength(int id, int length)
//        {
//            if (id == 0)
//            {
//                return Json(new { status = '0' });
//            }
//            else if (Request.Cookies["cart"] != null)
//            {
//                var cookie = Request.Cookies["cart"].Split("_");
//                if (cookie.Where(x => x == id.ToString()).Any())
//                {
//                    return Json(new { status = '0' });
//                }
//                else
//                {
//                    var list = new List<string>();
//                    foreach (var item in cookie)
//                    {
//                        if (item.Split("-")[0].Equals(id.ToString()))
//                        {
//                            var l = id.ToString() + "-" + length.ToString();
//                            list.Add(l);
//                        }
//                        else
//                        {
//                            list.Add(item);
//                        }
//                    }
//                    var s = ConvertStringArrayToString(list.ToArray());
//                    return Json(s);
//                }
//                //var s = ConvertStringArrayToString(list.ToArray());
//                //var n = new List<string>();
//                //if (s.Contains(id.ToString()))
//                //{
//                //    var item = cookie.SingleOrDefault(x => x.Split("-")[0] == id.ToString());
//                //    item.Split("-")[1] = length.ToString();
//                //    n.Add(item);
//                //}
//                //else
//                //{

//                //}
//                //return Json(new { status = '1' });
//            }
//            else
//            {
//                return Json(new { status = '0' });
//            }
//        }
//        public async Task<IActionResult> Category(int type)
//        {
//            var c = _context.ProductCategoryTypes.SingleOrDefault(x => x.Id == type);
//            if(c != null)
//                ViewBag.cType = c.Name;
//            var items = await _context.ProductCategories.Where(x => x.ProductCategoryType == type).ToListAsync();
//            return View(items);
//        }
//        public async Task<IActionResult> Products(int category)
//        {
//            if (Request.Cookies["cart"] != null)
//            {
//                var cookie = Request.Cookies["cart"].Split("_");
//                var list = new List<string>();
//                //return Json(cookie);
//                foreach (var item in cookie)
//                {
//                    list.Add(item.Split("-")[0]);
//                }
//                var s = ConvertStringArrayToString(list.ToArray());
//                ViewBag.Cookie = s;
//            }
//            else
//            {
//                ViewBag.Cookie = "";
//            }
//            var favorites = new List<string>();
//            if (User.Identity.Name != null)
//            {
//                var f = _context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name).Favorites;
//                if (f != null)
//                {
//                    favorites = f.Split("_").ToList();
//                }
            
//            }
//            ViewBag.Fav = favorites;

//            var p = await _context.Products.Where(x => x.CategoryIdLvl1 == category || x.CategoryIdLvl2 == category || x.CategoryIdLvl3 == category).Select(x => x.Code).ToListAsync();
//            var items = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
//                     .Join(_context.ProductCategories,
//                     ac => ac.CategoryIdLvl1,
//                     cc => cc.Id,
//                     (ac, cc) => new
//                     {
//                         ac,
//                         cc
//                     }
//                     ).ToList()
//                         group a by a.ac.Code into pp
//                         select new ProductWithCategoryVM
//                         {
//                             Product = pp.FirstOrDefault().ac,
//                             Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
//                         })/*.Where(x => x.Product.CategoryIdLvl1 == category)*/
//                         .ToList();
//            //var items = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)  
//            //         .Join(_context.ProductCategories,
//            //         ac => ac.CategoryIdLvl1,
//            //         cc => cc.Id,
//            //         (ac, cc) => new
//            //         {
//            //             ac,
//            //             cc
//            //         }
//            //         ).ToList()
//            //             group a by a.ac.Name into pp
//            //             select new ProductWithCategoryVM
//            //             {
//            //                 Product = pp.FirstOrDefault().ac,
//            //                 Date = pp.FirstOrDefault().ac.CreatedAt.ToPersianDateTextify(),
//            //                 Status = pp.FirstOrDefault().ac.ProductStatus.ToString(),
//            //                 Categories = removeDuplicate(String.Join(",", (pp.Select(x => x.cc.Name)).ToArray())),
//            //             })
//            //        .ToList();
//            return View(items.Where(x => p.Contains(x.Product.Code)).ToList());
//        }
//        public IActionResult LogIn()
//        {
//            return View();
//        }
//        public IActionResult Register(string call, int id)
//        {
//            if (call != null)
//                ViewBag.Call = call;
//            else
//                ViewBag.Call = "/";
//            return View();
//        }
//        [HttpPost]
//        public async Task<IActionResult> Register(string username)
//        {
//            var code = random.Next(100000, 999999);
//            var userCode = await _context.VerificationCode.FirstOrDefaultAsync(x => x.Username == username && x.Status == VerificationCodeStatus.Stall);
//            if (userCode != null)
//            {
//                userCode.Status = VerificationCodeStatus.Expired;
//                await _context.SaveChangesAsync();
//            }
//            var verification = new VerificationCode();
//            verification.Username = username;
//            verification.Code = code.ToString();
//            verification.Date = DateTime.Now;
//            verification.Status = VerificationCodeStatus.Stall;
//            await _context.VerificationCode.AddAsync(verification);
//            await _context.SaveChangesAsync();
//            return Json(new { Code = code, Vm = username });
//        }
//        public async Task<IActionResult> Verify(VerificationCode input, string callBack)
//        {
//            var url = Url.Action("index", "webapp");
//            if (callBack != null)
//                callBack = url;
//            var codeInDb = await _context.VerificationCode.Where(x => x.Username == input.Username && x.Code == input.Code && x.Status == VerificationCodeStatus.Stall).FirstOrDefaultAsync();
//            if (codeInDb != null)
//            {
//                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == input.Username);
//                if (user != null)
//                {
//                    await _signInManager.SignInAsync(user, true);
//                }
//                else
//                {
//                    var newUser = new ApplicationUser
//                    {
//                        UserName = input.Username,
//                        PhoneNumber = input.Username,
//                        RegistrationDateTime = DateTime.Now
//                    };
//                    var res = await _userManager.CreateAsync(newUser, "12345678");
//                    var result = await _userManager.AddToRoleAsync(newUser, "user");
//                    if (result.Succeeded)
//                    {
//                        await _signInManager.SignInAsync(newUser, true);
//                    }
//                }
//                codeInDb.Status = VerificationCodeStatus.Used;
//                return Json(new { status = '1', returnUrl = callBack });
//            }
//            else
//            {
//                return Json(new { status = '0', message = "کد ورودی اشتباه می باشد" });
//            }
//        }
//        public async Task<IActionResult> Cart()
//        {
//            ViewBag.P = @Request.Headers["Referer"].ToString();
//            var categories = await _context.ProductCategories.ToListAsync();
//            var user = _context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
//            var products = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
//                     .Join(_context.ProductCategories,
//                     ac => ac.CategoryIdLvl1,
//                     cc => cc.Id,
//                     (ac, cc) => new
//                     {
//                         ac,
//                         cc
//                     }
//                     ).ToList()
//                         group a by a.ac.Code into pp
//                         select new Product
//                         {
//                             Code = pp.FirstOrDefault().ac.Code,
//                             Name = pp.FirstOrDefault().ac.Name + " " + removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())).Replace(",",""),
//                             Price = pp.FirstOrDefault().ac.Price,
//                             ImageUrl = pp.FirstOrDefault().ac.ImageUrl,
//                             Id= pp.FirstOrDefault().ac.Id,
                              
//                         })
//                    .ToList();
//            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
//            var vm = new CartVM();
//            if (user != null)
//            {
//                if (user.Phone != null)
//                {
//                    vm.IsProfileCompelete = true;
//                }
//            }
//            var cart = Request.Cookies["cart"];
//            if (cart != null)
//            {
//                vm.ProductIds = cart.Split("_").Select(x => int.Parse(x.Split("-")[0])).ToList();
//                vm.Lengths = cart.Split("_").Select(x => double.Parse(x.Split("-")[1])).ToList();
//                vm.Products = products.Where(x => vm.ProductIds.Contains(int.Parse(x.Code))).ToList();
//                foreach (var item in vm.ProductIds)
//                {
//                    var l = vm.Lengths.ElementAt(vm.ProductIds.IndexOf(item));
//                    vm.Price += (vm.Products.SingleOrDefault(x => x.Code == item.ToString()).Price / 100) * l;
//                }
//                vm.SendPrice = int.Parse(sendPrice.Value);
//                vm.TotalPrice = vm.Price + vm.SendPrice;
//            }
//            return View(vm);
//        }
//        public async Task<IActionResult> RemoveFromBascket(string Id)
//        {
//            //var price = _context.Products.SingleOrDefault(p => p.Id == Guid.Parse(Id)).DiscountPrice;
//            double price = 0;
//            var products = await _context.Products.ToListAsync();
//            var cookie = Request.Cookies["cart"].ToString();
//            var ids = cookie.Split('_');
//            string[] newCookie = new string[ids.Length];
//            int j = 0;
//            int k = 0;
//            for (int i = 0; i < ids.Length; i++)
//            {
//                if (ids[i].Split("-")[0] != Id)
//                {
//                    newCookie[j] = ids.ElementAt(i);
//                    j++;
//                    price += int.Parse(ids[i].Split("-")[1]) * (products.FirstOrDefault(p => p.Code == ids[i].Split("-")[0]).Price / 100);
//                }
//                else
//                {
//                    k++;
//                }
//            }
//            Array.Resize(ref newCookie, j);
//            var output = ConvertStringArrayToString(newCookie);
//            //var discountInDb = await _context.DiscountCodes.Where(x => x.Username == User.Identity.Name && x.Status == DiscountCodeStatus.InUse).FirstOrDefaultAsync();
//            //if (discountInDb != null)
//            //{
//            //    if (discountInDb.Percent == discount)
//            //    {
//            //        var percent = 100 - discount;
//            //        //var dis = ;
//            //        price = (percent * price / 100);
//            //    }
//            //}
//            if (newCookie.Length > 0)
//            {
//                return Json(new { status = '1', items = output, count = k, cartValue = string.Format("{0:n0}", price) });
//            }
//            else
//            {
//                return Json(new { status = '0', items = "", count = k, cartValue = 0 });
//            }
//        }
//        public async Task<IActionResult> ChangeLengthCartPrice(string list, int newLength, string product)
//        {
//            //return Json(newLength);
//            double price = 0;
//            var products = await _context.Products.ToListAsync();
//            var produc = products.FirstOrDefault(x => x.Code == product);
//            var productPrice = (produc.Price / 100) * newLength;
//            var ids = list.Split('_');
//            for (int i = 0; i < ids.Length; i++)
//            {
//                var a = int.Parse(ids[i].Split("-")[1]);
//                var b = products.FirstOrDefault(p => p.Code == ids[i].Split("-")[0]).Price / 100;
//                price += a * b;
//            }
//            return Json(new { price = string.Format("{0:n0}", price), productPrice = string.Format("{0:n0}", productPrice) });
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddToFavorite(string id)
//        {
//            if (User.Identity.Name == null)
//            {
//                return Json(new { status = "0" });
//            }
//            var product = await _context.Products.Where(x => x.Code == id).ToListAsync();
//            if (product == null)
//            {
//                return Json(new { status = "0" });
//            }
//            else
//            {
//                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == User.Identity.Name);
//                if (string.IsNullOrWhiteSpace(user.Favorites))
//                {
//                    user.Favorites += "_" + id;
//                }
//                else
//                {
//                    user.Favorites += "_" + id;
//                }
//                var result = await _context.SaveChangesAsync();
//                return Json(new { status = "1", message = "با موفقیت ثبت شد" });
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> RemoveFromFavorites(string id)
//        {
//            if (User.Identity.Name == null)
//            {
//                return Json(new { status = "0" });
//            }
//            var product = await _context.Products.Where(x => x.Code == id).ToListAsync();
//            if (product == null)
//            {
//                return Json(new { status = "0" });
//            }
//            else
//            {
//                var user = _context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name);
//                if (user.Favorites.Split("_").Contains(id))
//                {
//                    var c = user.Favorites.Replace(id, "");
//                    if (c.Contains("_"))
//                    {
//                        c.Replace("_", "");
//                    }
//                    user.Favorites = c;
//                    var result = await _context.SaveChangesAsync();
//                    return Json(new { status = "1", message = "با موفقیت حذف شد" });
//                }
//                else
//                {
//                    return Json(new { status = "0", message = "false" });
//                }
//            }
//        }
//        public async Task<IActionResult> Favorite()
//        {
//            ViewBag.P = Request.Headers["Referer"].ToString();
//            if (User.Identity.Name == null)
//                return RedirectToAction("register");
//            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == User.Identity.Name);
//            var products = await _context.Products.ToListAsync();
//            var produclList = new List<Product>();
//            foreach (var item in user.Favorites.Split("_"))
//            {
//                if (!produclList.Contains(products.FirstOrDefault(x => x.Code == item)))
//                {
//                    produclList.Add(products.FirstOrDefault(x => x.Code == item));
//                }
//            }
//            return View(produclList);
//        }
//        public IActionResult EditProfile(string call)
//        {
//            ViewBag.Provinces = Iran.Provinces.OrderBy(i => i.ProvinceName);
//            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
//            var vm = new ProfileVM
//            {
//                Address = item.Address,
//                FirstName = item.FirstName,
//                LastName = item.LastName,
//                Phone = item.Phone,
//                PhoneNumber = item.PhoneNumber,
//                CallbackUrl = call,
//                Email = item.Email,
//                NationalCode = item.NationalCode,
//            };
//            return View(vm);
//        }
//        [HttpPost]
//        public async Task<IActionResult> SaveProfile(ProfileVM input)
//        {
//            if (!ModelState.IsValid)
//            {
//                var errors = new List<string>();
//                foreach (var item in ModelState.Values)
//                {
//                    foreach (var err in item.Errors)
//                    {
//                        errors.Add(err.ErrorMessage);
//                    }
//                }
//                return new JsonResult(new { Status = 0, Error = errors });
//            }
//            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
//            user.Address = input.Address;
//            user.FirstName = input.FirstName;
//            user.LastName = input.LastName;
//            user.NationalCode = input.NationalCode;
//            user.Phone = input.Phone;
//            user.Email = input.Email;
//            user.Gender = input.Gender;
//            await _context.SaveChangesAsync();
//            if (input.CallbackUrl == null)
//            {
//                return Json(Url.Action("index", "webapp"));
//            }
//            else
//            {
//                return Json(Url.Action("purchase", "webapp"));
//            }
//        }
//        [Authorize]
//        public IActionResult Purchase()
//        {
//            ViewBag.P = Request.Headers["Referer"].ToString();
//            var item = _context.Users.SingleOrDefault(i => i.UserName == User.Identity.Name);
//            var vm = new RequestFormVM
//            {
//                Address = item.Address,
//                FirstName = item.FirstName,
//                LastName = item.LastName,
//                Phone = item.Phone,
//                PhoneNumber = item.PhoneNumber,
//                Email = item.Email,
//            };
//            return View(vm);
//        }
//        [HttpPost]
//        public async Task<IActionResult> SubmitRequest(RequestFormVM request)
//        {
//            if (!ModelState.IsValid)
//            {
//                var errors = new List<string>();
//                foreach (var item in ModelState.Values)
//                {
//                    foreach (var err in item.Errors)
//                    {
//                        errors.Add(err.ErrorMessage);
//                    }
//                }
//                return new JsonResult(new { Status = 0, Error = errors });
//            }
//            //var cookie = Request.Cookies["cart"];
//            var carts = await _context.ShoppingCarts.ToListAsync();
//            var products = await _context.Products.ToListAsync();
//            var setting = await _context.Settings.ToListAsync();
//            var ProductIds = Request.Cookies["cart"].Split('_').ToList();
//            var productList = new List<Product>();
//            foreach (var item in ProductIds)
//            {
//                productList.Add(products.FirstOrDefault(p => p.Code == item.Split("-")[0]));
//            }
//            var list = new List<string>();
//            var quan = new List<string>();
//            var lengths = ProductIds.Select(x => int.Parse(x.Split("-")[1])).ToList();
//            foreach (var item in ProductIds)
//            {
//                if (list.Contains(item) == false)
//                {
//                    quan.Add(lengths.ElementAt(productList.IndexOf
//                        (productList.FirstOrDefault(x => x.Code == item.Split("-")[0]))).ToString());
//                    //var productQuantity = ProductIds.Where(x => x.ToString() == item.Id.ToString()).Count();
//                    //quan.Add(productQuantity.ToString());
//                    //list.Add(item.Id.ToString());
//                }
//            }
//            var cart = new ShoppingCart();
//            cart.Date = DateTime.Now;
//            cart.UserName = User.Identity.Name;
//            double price = 0;
//            //foreach (var item in productList)
//            //{
//            //    price += int.Parse(item.DiscountPrice);
//            //}
//            foreach (var item in ProductIds)
//            {
//                var s = item.Split("-")[1];
//                var l = Int32.Parse(s);
//                var p = productList.FirstOrDefault(x => x.Code == item.Split("-")[0]).Price / 100;
//                price += p * l;
//            }
//            var sendPrice = int.Parse(setting.SingleOrDefault(x => x.Key == "SendPrice").Value);
//            //var discount = await _context.DiscountCodes.Where(x => x.Status == DiscountCodeStatus.InUse).SingleOrDefaultAsync(x => x.Username == User.Identity.Name);
//            //if (discount == null)
//            //{
//            cart.Price = (price + sendPrice).ToString();
//            cart.SendPrice = sendPrice.ToString();
//            cart.DiscountPercent = "0";
//            //}
//            //else
//            //{
//            //    var percent = 100 - discount.Percent;
//            //    var finalPrice = (price * percent) / 100;
//            //    cart.Price = finalPrice.ToString();
//            //    discount.Status = DiscountCodeStatus.Used;
//            //    cart.DiscountPercent = discount.Percent.ToString();
//            //}
//            await _context.ShoppingCarts.AddAsync(cart);
//            await _context.SaveChangesAsync();
//            var items = new List<CartItem>();
//            for (int i = 0; i < ProductIds.Count; i++)
//            {
//                var cartItem = new CartItem();
//                cartItem.ProductId = Int32.Parse(ProductIds[i].Split("-")[0]);
//                cartItem.UserName = cart.UserName;
//                cartItem.Quantity = Int32.Parse(ProductIds[i].Split("-")[1]);
//                cartItem.Date = DateTime.Now;
//                cartItem.Price = productList.FirstOrDefault(x => x.Code == ProductIds[i].Split("-")[0]).Price.ToString();
//                items.Add(cartItem);
//                await _context.CartItems.AddAsync(cartItem);
//            }
//            await _context.SaveChangesAsync();
//            cart.CartItems = items;
//            cart.Name = request.FirstName + " " + request.LastName;
//            cart.PhoneNumber = request.PhoneNumber;
//            cart.Address = request.Address + " - " + request.PostalCode;
//            cart.UserName = User.Identity.Name;
//            cart.Date = DateTime.Now;
//            cart.Phone = request.Phone;
//            //cart.MapAddress = request.Map;
//            cart.PaymentType = PaymentType.Online;
//            await _context.SaveChangesAsync();
//            cart.PaymentType = PaymentType.InPerson;
//            while (true)
//            {
//                cart.RequestCode = "ZI-" + cart.Id.ToString() + random.Next(1000, 9999).ToString();
//                if (!carts.Where(x => x.RequestCode == cart.RequestCode).Any())
//                {
//                    break;
//                }
//            }
//            await _context.SaveChangesAsync();
//            Response.Cookies.Delete("cart");
//            return Json(new { status = "2", url = Url.Action("index", "webapp"), item = cart.RequestCode });
//            //}
//        }
//        [Authorize]
//        public async Task<IActionResult> SuccessfulRequest(string code)
//        {
//            var request = await _context.ShoppingCarts.Include(x => x.CartItems).SingleOrDefaultAsync(x => x.RequestCode == code && x.UserName == User.Identity.Name);
//            if (request == null)
//                return RedirectToAction("Register");

//            ViewBag.Code = code;
//            var products = await _context.Products.ToListAsync();
//            var sendPrice = await _context.Settings.SingleOrDefaultAsync(x => x.Key == "SendPrice");
//            var vm = new CartVM();
//            if (request != null)
//            {
//                vm.ProductIds = request.CartItems.Select(x => x.ProductId).ToList();
//                vm.Lengths = request.CartItems.Select(x => x.Quantity).ToList();
//                vm.Products = products.Where(x => vm.ProductIds.Contains(x.Id)).ToList();
//                vm.TotalPrice = int.Parse(request.Price);
//                vm.SendPrice = int.Parse(sendPrice.Value);
//                vm.Price = vm.TotalPrice - vm.SendPrice;
//            }
//            return View(vm);
//        }
//        [Route("webapp/productDetail/{code}/{name}")]
//        public async Task<IActionResult> ProductDetail(string code,string name)
//        {
//            //var item = await _context.Products.FirstOrDefaultAsync(x => x.Code == code);
//            //var cTypes = await _context.ProductCategoryTypes.ToListAsync();
//            //var categories = await _context.ProductCategories.ToListAsync();
//            //var list = new List<int>();
//            //list.Add(item.CategoryIdLvl1);
//            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
//            if (Request.Cookies["cart"] != null)
//            {
//                var cookie = Request.Cookies["cart"].Split("_");
//                var list = new List<string>();
//                //return Json(cookie);
//                foreach (var item in cookie)
//                {
//                    list.Add(item.Split("-")[0]);
//                }
//                var s = ConvertStringArrayToString(list.ToArray());
//                ViewBag.Cookie = s;
//            }
//            else
//            {
//                ViewBag.Cookie = "";
//            }
//            var favorites = new List<string>();
//            if (user != null)
//            {
//                var f = _context.Users.SingleOrDefault(x => x.UserName == User.Identity.Name).Favorites;
//                if (f != null)
//                {
//                    favorites = f.Split("_").ToList();
//                }
//            }
//            //var items = await _context.Products.Where(x => x.ProductCategoryType == 7).Join(_context.ProductCategories).ToListAsync();
//            var products = (from a in _context.Products.Include(x => x.Images).Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش && x.Name == name)
//                         .Join(_context.ProductCategories,
//                     ac => ac.CategoryIdLvl1,
//                     cc => cc.Id,
//                     (ac, cc) => new
//                     {
//                         ac,
//                         cc
//                     }
//                     ).ToList()
//                            group a by a.ac.Name into pp
//                            select new ProductWithCategoryVM
//                            {
//                                Product = pp.FirstOrDefault(x => x.ac.Code == code)?.ac,
//                                Categories = removeDuplicate(String.Join(", ", (pp.Where(x => x.ac.Code == code).Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
//                                IsFavorite = favorites.Contains(code),
//                                ProductCategories = pp.Where(x => x.cc.ProductCategoryType == 7).Select(x => x.cc).ToList(),
//                                Codes = removeDuplicate(String.Join(",", pp.Select(x => x.ac.Code).ToArray())),
//                                Material = removeDuplicate(String.Join(", ", (pp.Where(x => x.ac.Code == code).Select(x => x.cc.ProductCategoryType == 3 ? x.cc.Name : "")).ToArray())),
//                            })
//                    .ToList();

//            var items = (from a in _context.Products.Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش)
//                         .Join(_context.ProductCategories,
//                     ac => ac.CategoryIdLvl1,
//                     cc => cc.Id,
//                     (ac, cc) => new
//                     {
//                         ac,
//                         cc
//                     }
//                     ).ToList()
//                            group a by a.ac.Name into pp
//                            select new ProductWithCategoryVM
//                            {
//                                Product = pp.FirstOrDefault(x => x.ac.ProductCategoryType == 7).ac,
//                                Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "").Take(1)).ToArray())),
//                            })
//                    .ToList();

//            ViewBag.Similar = items.Where(X => X.Product.CategoryIdLvl1 == products.FirstOrDefault().Product.CategoryIdLvl1 && X.Product.Code != code).ToList();
//            //var s = new ProductCategoryType();
//            //var ct = products
//            //    .Where(x => cTypes
//            //    .Where(c => c.Id != 7).SingleOrDefault(z => z.Id == categories
//            //    .SingleOrDefault(y => y.Id == x.Product.CategoryIdLvl1).ProductCategoryType) != s)
//            //    .Where(x => x.Product.Name == item.Name).ToList();
//            ////ViewBag.Products = products.Where(x => x.Product.Code != code).ToList();
//            //ViewBag.Cat = products.FirstOrDefault(x => x.Product.Code == code).Categories.Split(", ").ToList();
//            return View(products.FirstOrDefault(x => x.Product.Code == code));
//        }
//        [HttpPost]
//        public async Task<IActionResult> GetComments(string code)
//        {
//            var comments = await _context.Comments.Where(x => x.Status == CommentStatus.تایید_شده && x.ProductCode == code).ToListAsync();
//            return Json(comments);
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddComment(Comment comment)
//        {
//            var product = await _context.Products.FirstOrDefaultAsync(x => x.Code == comment.ProductCode);
//            comment.Date = DateTime.Now;
//            comment.Username = User.Identity.Name;
//            comment.Product = product;
//            await _context.Comments.AddAsync(comment);
//            await _context.SaveChangesAsync();
//            return Json(comment);
//        }
//        //public IActionResult IsFavorite()
//        //{
//        //    return Json(_context.Favorites.ToList());
//        //}

//        /* Compare */
//        public async Task<IActionResult> Compare()
//        {
//            var cookie = "";
//            if (Request.Cookies["compare"] != null)
//            {
//                cookie = Request.Cookies["compare"].ToString();
//            }
//            var p = await _context.Products.FirstOrDefaultAsync(x => x.Code == cookie);
//            ViewBag.P = Request.Headers["Referer"].ToString();
//            var products = (from a in _context.Products.Include(x => x.Images).Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش && x.Name == p.Name)
//                    .Join(_context.ProductCategories,
//                ac => ac.CategoryIdLvl1,
//                cc => cc.Id,
//                (ac, cc) => new
//                {
//                    ac,
//                    cc
//                }
//                ).ToList()
//                            group a by a.ac.Name into pp
//                            select new ProductWithCategoryVM
//                            {
//                                Product = pp.FirstOrDefault(x => x.ac.Code == cookie)?.ac,
//                                Categories = removeDuplicate(String.Join(", ", (pp.Where(x => x.ac.Code == cookie).Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())),
//                                Material = removeDuplicate(String.Join(", ", (pp.Where(x => x.ac.Code == cookie).Select(x => x.cc.ProductCategoryType == 3 ? x.cc.Name : "")).ToArray())),
//                            })
//               .ToList();
//            return View(products.FirstOrDefault(x => x.Product.Code == cookie));
//        }
//        [HttpGet]
//        public async Task<IActionResult> CompareList()
//        {
//            var cookie = "";
//            if (Request.Cookies["compare"] != null)
//            {
//                cookie = Request.Cookies["compare"].ToString();
//            }
//            var p = await _context.Products.FirstOrDefaultAsync(x => x.Code == cookie);
//            var products = (from a in _context.Products.Include(x => x.Images).Where(x => x.ProductStatus == ProductStatus.آماده_برای_فروش && x.Code != cookie)
//                .Join(_context.ProductCategories,
//            ac => ac.CategoryIdLvl1,
//            cc => cc.Id,
//            (ac, cc) => new
//            {
//                ac,
//                cc
//            }
//            ).ToList()
//                            group a by a.ac.Name into pp
//                            select new ProductWithCategoryVM
//                            {
//                                Product = pp.FirstOrDefault().ac,
//                                Categories = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 7 ? x.cc.Name : "")).ToArray())).Replace(",", ""),
//                                Material = removeDuplicate(String.Join(", ", (pp.Select(x => x.cc.ProductCategoryType == 3 ? x.cc.Name : "")).ToArray())).Replace(",",""),
//                            })
//           .ToList();
//            return Json(products);
//        }
//        public IActionResult Tailor()
//        {
//            return View();
//        }

//        public IActionResult Tailordetail()
//        {
//            return View();
//        }
//        public IActionResult AboutUs()
//        {
//            return View();
//        }
//    }
//}