using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using DNTPersianUtils.Core.IranCities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using KaraYadak.Data;
using KaraYadak.Helper;
using KaraYadak.Models;
using KaraYadak.ViewModels;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        [Route("{Controller}/{Action}/")]
        [Route("{Controller}/{Action}/{phoneNumber}")]
        [Route("{Controller}/{Action}/{phoneNumber}/{returnUrl}")]
        public IActionResult AddCustomer(string phoneNumber, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)!)
            {
                ViewBag.Url = returnUrl;
            }
            else
            {
                ViewBag.Url = "/Customer/ProductSale";
            }
            var model = new ProfileVM();
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var user = _context.Users.Where(x => x.PhoneNumber.Equals(phoneNumber)).FirstOrDefault();
                if (user != null)
                {
                    model = new ProfileVM()
                    {
                        PhoneNumber = user.Phone,
                        Phone = user.Phone,
                        Address = user.Address,
                        CallbackUrl = "",
                        FirstName = user.FirstName,
                        Email = user.Email,
                        Gender = user.Gender,
                        LastName = user.LastName,
                        NationalCode = user.NationalCode,
                    };
                    return View(model);
                }
            }

            ViewBag.Provinces = Iran.Provinces.OrderBy(i => i.ProvinceName);

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomers(ProfileVM input)
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var newUser = new ApplicationUser();

            var user = await _context.Users.Where(x => x.PhoneNumber.Equals(input.PhoneNumber)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Address = input.Address;
                user.FirstName = input.FirstName;
                user.LastName = input.LastName;
                user.NationalCode = input.NationalCode;
                user.Phone = input.Phone;
                user.Email = input.Email;
                user.Gender = input.Gender;
                user.PhoneNumber = input.PhoneNumber;
                _context.Entry(user).State = EntityState.Modified;
            }
            else
            {
                newUser.Address = input.Address;
                newUser.FirstName = input.FirstName;
                newUser.LastName = input.LastName;
                newUser.NationalCode = input.NationalCode;
                newUser.Phone = input.Phone;
                newUser.Email = input.Email;
                newUser.Gender = input.Gender;
                newUser.PhoneNumber = input.PhoneNumber;
                await _context.Users.AddAsync(newUser);

            }


            await _context.SaveChangesAsync();
            if (input.CallbackUrl == null)
            {
                return Json(new
                {
                    status = "1",
                    Url = Url.Action("ProductSale", "Customer"),
                    result = user??newUser
                });
            }
            else
            {
                return Json(new
                {
                    status = "1",
                    Url = input.CallbackUrl,
                    result = user ?? newUser
                });
            }
        }
        public IActionResult ProductSale()
        {
            ViewBag.sendPrice = _context.Settings.Where(x => x.Key == "SendPrice").Select(x => x.Value).SingleOrDefault();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductSales(string qrCode)
        {
            var item = await _context.CartItems.Where(x => x.QR.Equals(qrCode)).FirstOrDefaultAsync();

            if (item != null)
            {
                var product = _context.Products.Find(item.ProductId);
                var data = new CartItemForSaler()
                {
                    ProductId = item.ProductId,
                    Date = item.Date,
                    Descrip = product.Description,
                    discount = product.Discount,
                    Price = item.Price,
                    QR = item.QR,
                    Quantity = item.Quantity,
                    UserName = item.UserName

                };
                return new JsonResult(new { status = "1", result = data });

            }
            else
            {
                return new JsonResult(new { status = "0", message = "خظایی رخ داده است" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUserByPhoneNumber(string phoneNumber)
        {
            var user = await _context.Users.Where(x => x.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.Phone == null)
                {
                    return new JsonResult(new
                    {
                        status = "0",
                        message = "اطلاعات کاربری تکمیل شود!",
                        url = "/Customer/AddCustomer/" + phoneNumber
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        status = "1",
                        message = "",
                        url = "/Customer/ProductSale"

                    });
                }

            }
            else
            {
                return new JsonResult(new
                {
                    status = "0",
                    message = "کاربری یافت نشد",
                    url = "/Customer/AddCustomer"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUserByCart()
        {
            var mifare = new Mifare();
            var phoneNumber = "";
            var contectDevice = mifare.ConnectDevice();
            if (contectDevice.Status == "0")
            {
                return new JsonResult(new { message = contectDevice.Message, status = contectDevice.Status });
            }
            else
            {
                var connectCart = mifare.ConnectCart();
                if (connectCart.Status == "0")
                {
                    return new JsonResult(new { message = connectCart.Message, status = connectCart.Status });
                }
                else
                {
                    var readInfo = mifare.Read();
                    if (readInfo.Status == "0")
                    {
                        return new JsonResult(new { message = readInfo.Message, status = readInfo.Status });
                    }
                    else
                    {
                        phoneNumber = readInfo.PhoneNumber;
                        var user = await _context.Users.Where(x => x.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            return new JsonResult(new
                            {
                                status = "1",
                                message = "",
                                url = "/Customer/ProductSale",
                                result = user

                            });
                        }
                        else
                        {
                            return new JsonResult(new
                            {
                                status = "0",
                                message = "کاربری یافت نشد",
                                url = "/Customer/ProductSale",

                            });
                        }


                    }

                }
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddUserInCustomersClub(string phoneNumber)

        {
            var mifare = new Mifare();
            var contectDevice = mifare.ConnectDevice();
            if (contectDevice.Status == "0")
            {
                return new JsonResult(new { message = contectDevice.Message, status = contectDevice.Status });
            }
            else
            {
                var connectCart = mifare.ConnectCart();
                if (connectCart.Status == "0")
                {
                    return new JsonResult(new { message = connectCart.Message, status = connectCart.Status });
                }
                else
                {
                    var writeInfo = mifare.Write(phoneNumber);
                    if (writeInfo.Status == "0")
                    {
                        return new JsonResult(new { message = writeInfo.Message, status = writeInfo.Status });
                    }
                    else
                    {

                        return new JsonResult(new
                        {
                            status = "1",
                            message = "با موفقیت ثبت شد",


                        });

                    }

                }
            }

        }
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(string qrCode)
        {
            var cartItem = await _context.CartItems.Where(x => x.QR.Equals(qrCode)).FirstOrDefaultAsync();
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return new JsonResult(new { message = "با موفقیت حذف شد", status = "1" });
            }
            else return new JsonResult(new { message = "خطایی رخ داده است", status = "0" });
        }
        [HttpPost]
        public async Task<IActionResult> EditCartItem(CartItemForSaler cartItemForSaler)
        {

            var cartItem = await _context.CartItems.Where(x => x.QR.Equals(cartItemForSaler.QR)).FirstOrDefaultAsync();
            if (cartItem != null)
            {
                try
                {
                    cartItem.Price = cartItemForSaler.Price;
                    cartItem.ProductId = cartItemForSaler.ProductId;
                    cartItem.Quantity = cartItemForSaler.Quantity;
                    _context.Entry(cartItem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { message = "با موفقیت بروزرسانی شد", status = "1" });
                }
                catch (Exception ex)
                {

                    return new JsonResult(new { message = ex.Message.ToString() + ex.StackTrace.ToString(), status = "1" });
                }
            }
            else return new JsonResult(new { message = "خطایی رخ داده است", status = "0" });
        }

    }
}
