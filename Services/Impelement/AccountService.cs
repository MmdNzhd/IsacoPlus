using DNTPersianUtils.Core;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaraYadak.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationUser> GetAdmin()
        {
            var admin = await (from r in _context.Roles
                               where r.Name == "Admin"
                               join ur in _context.UserRoles
                               on r.Id equals ur.RoleId
                               join u in _context.Users
                               on ur.UserId equals u.Id
                               select u).AsNoTracking().FirstOrDefaultAsync();
            return (admin);

        }

        public async Task<ApplicationUser> GetCurrectUser()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            return (user);
        }

        public async Task<List<AllUserForAdminViewModel>> GetAllUserForAdmin(string startDate, string endDate)
        {
            try
            {



                Regex regex = new Regex(@"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|31|([1-2][0-9])|(0[1-9]))))$");


                DateTime? sDate = null;
                DateTime? eDate = null;
                if (startDate != null)
                {
                    if (regex.IsMatch(startDate))
                    {
                        return (null);
                    }

                    sDate = startDate.ToGregorianDateTime(true);
                    sDate = sDate.Value.AddDays(+1);
                }

                if (endDate != null)
                {
                    if (regex.IsMatch(endDate))
                    {
                        return (null);
                    }
                    eDate = endDate.ToGregorianDateTime(true);
                    eDate = eDate.Value.AddDays(+1);

                }
                var finalModel = _context.Users.Include(x => x.Payments).Select(x => new AllUserForAdminViewModel
                {
                    Id = x.Id,
                    Date = x.RegistrationDateTime,
                    FullName = x.FirstName + " " + x.LastName,
                    Address = x.Address,
                    City = x.City,
                    Logo = x.AvatarUrl,
                    Email = x.Email,
                    PhoneNUmber = x.PhoneNumber,
                    Province = x.Province,
                    PostalCode = x.PostalCode,
                    IsActive = IsEnable(x),
                    CountOfOrder = x.Payments.Where(x => x.IsSucceed).ToList().Count.ToString(),
                    SumOfOrder = (x.Payments.Where(x => x.IsSucceed).ToList().Count > 0) ? SumPayment(x.Payments) : ""
                }).AsNoTracking().AsQueryable();

                if (sDate != null)
                {
                    finalModel = finalModel.Where(x => x.Date >= sDate);
                }

                if (eDate != null)
                {
                    finalModel = finalModel.Where(x => x.Date <= eDate);
                }
                return await finalModel.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string SumPayment(ICollection<Payment> payments)
        {
            if (payments.Where(x => x.IsSucceed).Count() > 0)
            {
                return payments.Where(x => x.IsSucceed).Sum(x => x.FinallyAmountWithTax).ToString();
            }
            return "";
        }
        public async Task<(bool isSuccess, string error)> BlockUser(string userId)
        {

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return (false, "کاربر یافت نشد");
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddDays(3000);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");

            }

        }

        public async Task<(bool isSuccess, string error)> UnBlockUser(string userId)
        {

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return (false, "کاربر یافت نشد");
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");

            }
        }

        public async Task<(bool isSuccess, string error)> GetUserAddress(string phoneNumber)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (user == null || user.LockoutEnabled) return (false, "کاربر نا معتبر");
                return (true, user.Address);
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");
            }
        }

        public static bool IsEnable(ApplicationUser user)
        {
            if (user.LockoutEnabled) return false;
            else return true;
        }

        public async Task<ApplicationUser> GetWarehousingAdmin()
        {
            var admin = await (from r in _context.Roles
                               where r.Name == PublicHelper.WarehousingAdminROLE
                               join ur in _context.UserRoles
                               on r.Id equals ur.RoleId
                               join u in _context.Users
                               on ur.UserId equals u.Id
                               select u).AsNoTracking().FirstOrDefaultAsync();
            return (admin);

        }

        public async Task<(bool isSuccess, string error)> ChangeWarehousingAdmin(string phoneNumber)
        {
            try
            {
                var regex = new Regex(@"^(\+98|0)?9\d{9}$");
                if (!regex.IsMatch(phoneNumber)) return (false, "شماره وارد شده نا معتبر میباشد");
                if (await _context.Users.AnyAsync(x => x.UserName == phoneNumber))
                {
                    return (false, "کاربری با این شماره ثبت شده است");
                }
                var admin = await (from r in _context.Roles
                                   where r.Name == PublicHelper.WarehousingAdminROLE
                                   join ur in _context.UserRoles
                                   on r.Id equals ur.RoleId
                                   join u in _context.Users
                                   on ur.UserId equals u.Id
                                   select u).AsNoTracking().FirstOrDefaultAsync();

                if (admin == null) return (false, "کاربری یافت نشد");
                admin.PhoneNumber = phoneNumber;
                admin.UserName = phoneNumber;
                admin.NormalizedUserName = phoneNumber;
                _context.Users.Update(admin);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");
            }
        }

        public async Task<(bool isSuccess, string error, UserInfoForReportViewModel model)> GetUserInfoForReport(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return (false, "کاربری یافت نشد", null);
                }
                var finalModel = new UserInfoForReportViewModel()
                {
                    Address = user.Address,
                    City = user.City,
                    PhoneNumber = user.PhoneNumber,
                    Province = user.Province
                };
                return (true, "", finalModel);
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است", null);
            }
        }

        public async Task<(bool isSuccess, string error, List<CustomerPurchaseReportViewModel> model)> GetCustomerPurchaseReport(string userId, string startDate = null, string endDate = null)
        {
            try
            {
                Regex regex = new Regex(@"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|31|([1-2][0-9])|(0[1-9]))))$");


                DateTime? sDate = null;
                DateTime? eDate = null;
                if (startDate != null)
                {
                    if (regex.IsMatch(startDate))
                    {
                        return (false, "تاریخ شروع را درست وارد کنید", null);
                    }

                    sDate = startDate.ToGregorianDateTime(true);
                    sDate = sDate.Value.AddDays(+1);
                }

                if (endDate != null)
                {
                    if (regex.IsMatch(endDate))
                    {
                        return (false, "تاریخ پایان را درست وارد کنید", null);
                    }
                    eDate = endDate.ToGregorianDateTime(true);
                    eDate = eDate.Value.AddDays(+1);

                }
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return (false, "کابری یافت نشد", null);
                var allProducts = new List<CartItem>();
                var allProduct = await (from p in _context.Payments
                                        where p.UserId == user.Id
                                        join c in _context.ShoppingCarts.Include(x => x.CartItems)
                                        on p.ShoppingCartId equals c.Id
                                        select c.CartItems).ToListAsync();

                foreach (var item in allProduct)
                {
                    allProducts.AddRange(item);


                }
                var allProductGroupById = allProducts.GroupBy(x => x.ProductId);
                var finalModal = allProductGroupById.Select(x => new CustomerPurchaseReportViewModel()
                {
                    Price = x.FirstOrDefault().Price,
                    Discount = x.FirstOrDefault().Discount,
                    ProductCode = x.FirstOrDefault().ProductCode,
                    ProductName = x.FirstOrDefault().ProductName,
                    Date = x.FirstOrDefault().Date,
                    BuyCount = x.ToList().Sum(x => x.Quantity).ToString()
                }).ToList();


                if (sDate != null)
                {
                    finalModal = finalModal.Where(x => x.Date >= sDate).ToList();
                }

                if (eDate != null)
                {
                    finalModal = finalModal.Where(x => x.Date <= eDate).ToList();
                }
                finalModal.Add(new CustomerPurchaseReportViewModel()
                {
                    ProductCode = "#All",
                    ProductName = "مجموع",
                    Discount = finalModal.Sum(x => Convert.ToDecimal(x.Discount)).ToString(),
                    Price = finalModal.Sum(x => Convert.ToDecimal(x.Price)).ToString(),
                    BuyCount = finalModal.Sum(x => Convert.ToDecimal(x.BuyCount)).ToString()
                });
                return (true, "", finalModal);
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است", null);
            }
        }

        public async Task<(bool isSuccess, string error, List<CustomersWhitProductReportViewModel> model)>
            CustomersWhitProductReport(string userId = null, string productCode = null, string startDate = null, string endDate = null)
        {
            try
            {
                Regex regex = new Regex(@"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|31|([1-2][0-9])|(0[1-9]))))$");


                DateTime? sDate = null;
                DateTime? eDate = null;
                if (startDate != null)
                {
                    if (regex.IsMatch(startDate))
                    {
                        return (false, "تاریخ شروع را درست وارد کنید", null);
                    }

                    sDate = startDate.ToGregorianDateTime(true);
                    sDate = sDate.Value.AddDays(+1);
                }

                if (endDate != null)
                {
                    if (regex.IsMatch(endDate))
                    {
                        return (false, "تاریخ پایان را درست وارد کنید", null);
                    }
                    eDate = endDate.ToGregorianDateTime(true);
                    eDate = eDate.Value.AddDays(+1);

                }
                ApplicationUser user = new ApplicationUser();
                IQueryable<CustomersWhitProductReportViewModel> allProduct;
                allProduct = (from p in _context.Payments
                              join c in _context.ShoppingCarts.Include(x => x.CartItems)
                              on p.ShoppingCartId equals c.Id
                              select new CustomersWhitProductReportViewModel
                              {
                                  FactorId = p.Id.ToString(),
                                  Date = p.Date,
                                  PersianDate = p.Date.ToShortPersianDateString(true),
                                  Discount = p.Discount.ToString(),
                                  Price = p.Amount.ToString(),
                                  PriceWithDisCount = p.FinallyAmountWithTax.ToString(),
                                  BuyCount = "1",
                                  ProductsCode = c.CartItems.Select(x => new ProductCodeWithId { Id = x.Id.ToString(), Code = x.ProductCode }).ToList(),
                                  UserId = p.UserId

                              }).AsNoTracking().AsQueryable();


                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _context.Users.FindAsync(userId);

                }
                if (user.UserName != null)
                {
                    allProduct = allProduct.Where(x => x.UserId.Equals(user.Id));

                }

                if (sDate != null)
                {
                    allProduct = allProduct.Where(x => x.Date >= sDate);
                }
                if (eDate != null)
                {
                    allProduct = allProduct.Where(x => x.Date <= eDate);
                }
                var finalmodel = await allProduct.ToListAsync();

                if (!string.IsNullOrEmpty(productCode))
                {
                    finalmodel = finalmodel.Where(x => x.ProductsCode.Any(x => x.Code == productCode.Trim())).ToList();
                }


                return (true, "", finalmodel);




            }
            catch (Exception ex)
            {
                return (false, JsonConvert.SerializeObject(ex), null);

                return (false, "خطایی رخ داده است", null);
            }
        }

        public async Task<(bool isSuccess, string error, List<CustomersPurchaseReportViewModel> model)> GetCustomersPurchaseReport(string userId, string productCode, string startDate, string endDate)
        {
            try
            {
                Regex regex = new Regex(@"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|31|([1-2][0-9])|(0[1-9]))))$");


                DateTime? sDate = null;
                DateTime? eDate = null;
                if (startDate != null)
                {
                    if (regex.IsMatch(startDate))
                    {
                        return (false, "تاریخ شروع را درست وارد کنید", null);
                    }

                    sDate = startDate.ToGregorianDateTime(true);
                    sDate = sDate.Value.AddDays(+1);
                }

                if (endDate != null)
                {
                    if (regex.IsMatch(endDate))
                    {
                        return (false, "تاریخ پایان را درست وارد کنید", null);
                    }
                    eDate = endDate.ToGregorianDateTime(true);
                    eDate = eDate.Value.AddDays(+1);

                }
                ApplicationUser user = new ApplicationUser();
                var allProduct = await (from p in _context.Payments.Include(x => x.User)
                                        join c in _context.ShoppingCarts.Include(x => x.CartItems)
                                        on p.ShoppingCartId equals c.Id
                                        select new ProductWithCustomerName
                                        {

                                            customerName = p.User.FirstName + " " + p.User.LastName,
                                            customerId = p.UserId,
                                            CartItems = c.CartItems.ToList()

                                        }).ToListAsync();
                var products = new List<CartItem>();
                foreach (var item in allProduct)
                {
                    foreach (var pr in item.CartItems)
                    {
                        pr.UserName = item.customerName;
                        pr.QR = item.customerId;
                        pr.Discount = (Convert.ToDouble(pr.Price) * Convert.ToDouble(pr.Discount) / 100).ToString();
                    }
                    products.AddRange(item.CartItems);
                }

                var finalProductWithGroupBy = products.GroupBy(x => new { x.UserName, x.ProductCode }).ToList();
                var finalProduct = finalProductWithGroupBy.Select(x => new CustomersPurchaseReportViewModel
                {
                    CustomerName = x.FirstOrDefault().UserName,
                    UserId = x.FirstOrDefault().QR,
                    ProductName = x.FirstOrDefault().ProductName,
                    ProductCode = x.FirstOrDefault().ProductCode,
                    Discount = x.FirstOrDefault().Discount,
                    Date = x.FirstOrDefault().Date,
                    Price = x.FirstOrDefault().Price,
                    BuyCount = x.ToList().Sum(q => q.Quantity).ToString(),
                }).ToList();


                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _context.Users.FindAsync(userId);

                }
                if (user.UserName != null)
                {
                    finalProduct = finalProduct.Where(x => x.UserId == user.Id).ToList();

                }

                if (sDate != null)
                {
                    finalProduct = finalProduct.Where(x => x.Date >= sDate).ToList();
                }
                if (eDate != null)
                {
                    finalProduct = finalProduct.Where(x => x.Date <= eDate).ToList();
                }

                if (!string.IsNullOrEmpty(productCode))
                {
                    finalProduct = finalProduct.Where(x => x.ProductCode == productCode.Trim()).ToList();
                }
                var listOfOrders = await (from p in _context.Payments
                                          join c in _context.ShoppingCarts.Include(x => x.CartItems)
                                          on p.ShoppingCartId equals c.Id
                                          select c).ToListAsync();
                foreach (var model in finalProduct)
                {
                    model.FactorCount = listOfOrders.Where(y => y.CartItems.Any(x => x.ProductCode == model.ProductCode
                    && x.UserName == y.UserName)
                    ).Count().ToString();
                }
                finalProduct = finalProduct.OrderBy(x => x.CustomerName).ToList();
                finalProduct.Add(new CustomersPurchaseReportViewModel()
                {
                    BuyCount = finalProduct.Sum(x => Convert.ToDouble(x.BuyCount)).ToString(),
                    Discount = finalProduct.Sum(x => Convert.ToDouble(x.Discount)).ToString(),
                    Price = finalProduct.Sum(x => Convert.ToDouble(x.Price)).ToString(),
                    FactorCount = finalProduct.Sum(x => Convert.ToDouble(x.FactorCount)).ToString(),
                    CustomerName = "all",
                    ProductName = "مجموع"
                });

                return (true, "", finalProduct);




            }
            catch (Exception ex)
            {

                return (false, "خطایی رخ داده است", null);
            }
        }

        public async Task<(bool isSuccess, string error, List<ProductWhitCustomersReportViewModel> model)> GetProductWhitCustomersReport(string userId, string productCode, string startDate, string endDate)
        {
            try
            {
                Regex regex = new Regex(@"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|31|([1-2][0-9])|(0[1-9]))))$");


                DateTime? sDate = null;
                DateTime? eDate = null;
                if (startDate != null)
                {
                    if (regex.IsMatch(startDate))
                    {
                        return (false, "تاریخ شروع را درست وارد کنید", null);
                    }

                    sDate = startDate.ToGregorianDateTime(true);
                    sDate = sDate.Value.AddDays(+1);
                }

                if (endDate != null)
                {
                    if (regex.IsMatch(endDate))
                    {
                        return (false, "تاریخ پایان را درست وارد کنید", null);
                    }
                    eDate = endDate.ToGregorianDateTime(true);
                    eDate = eDate.Value.AddDays(+1);

                }
                ApplicationUser user = new ApplicationUser();
                var allProduct = await (from p in _context.Payments.Include(x => x.User)
                                        join c in _context.ShoppingCarts.Include(x => x.CartItems)
                                        on p.ShoppingCartId equals c.Id
                                        select new ProductWithCustomerName
                                        {

                                            customerName = p.User.FirstName + " " + p.User.LastName,
                                            customerId = p.UserId,
                                            CartItems = c.CartItems.ToList()

                                        }).ToListAsync();
                var products = new List<CartItem>();
                foreach (var item in allProduct)
                {
                    foreach (var pr in item.CartItems)
                    {
                        pr.UserName = item.customerName;
                        pr.QR = item.customerId;
                        pr.Discount = (Convert.ToDouble(pr.Price) * Convert.ToDouble(pr.Discount) / 100).ToString();
                    }
                    products.AddRange(item.CartItems);
                }

                var finalProductWithGroupBy = products.GroupBy(x => new { x.UserName, x.ProductCode }).ToList();
                var finalProduct = finalProductWithGroupBy.Select(x => new CustomersPurchaseReportViewModel
                {
                    CustomerName = x.FirstOrDefault().UserName,
                    UserId = x.FirstOrDefault().QR,
                    ProductName = x.FirstOrDefault().ProductName,
                    ProductCode = x.FirstOrDefault().ProductCode,
                    Discount = x.FirstOrDefault().Discount,
                    Date = x.FirstOrDefault().Date,
                    Price = x.FirstOrDefault().Price,
                    BuyCount = x.ToList().Sum(q => q.Quantity).ToString(),
                }).ToList();


                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _context.Users.FindAsync(userId);

                }
                if (user.UserName != null)
                {
                    finalProduct = finalProduct.Where(x => x.UserId == user.Id).ToList();

                }

                if (sDate != null)
                {
                    finalProduct = finalProduct.Where(x => x.Date >= sDate).ToList();
                }
                if (eDate != null)
                {
                    finalProduct = finalProduct.Where(x => x.Date <= eDate).ToList();
                }

                if (!string.IsNullOrEmpty(productCode))
                {
                    finalProduct = finalProduct.Where(x => x.ProductCode == productCode.Trim()).ToList();
                }
                var listOfOrders = await (from p in _context.Payments
                                          join c in _context.ShoppingCarts.Include(x => x.CartItems)
                                          on p.ShoppingCartId equals c.Id
                                          select c).ToListAsync();
                foreach (var model in finalProduct)
                {
                    model.FactorCount = listOfOrders.Where(y => y.CartItems.Any(x => x.ProductCode == model.ProductCode
                    && x.UserName == y.UserName)
                    ).Count().ToString();
                }
                finalProduct = finalProduct.OrderBy(x => x.CustomerName).ToList();


                var finalReport = finalProduct.GroupBy(y => y.ProductCode)
                                    .Select(o => new ProductWhitCustomersReportViewModel()
                                    {
                                        Date = o.FirstOrDefault().Date,
                                        ProductCode = o.FirstOrDefault().ProductCode,
                                        ProductName = o.FirstOrDefault().ProductName,
                                        CustomerWithBuyCounts = o.Select(x => new CustomerWithBuyCount()
                                        {
                                            CustomerId = x.UserId,
                                            CustomerName = x.CustomerName,
                                            BuyCount = x.BuyCount
                                        }).ToList()
                                    }).ToList();

                var allUser =await _context.Users.Select(x => new CustomerWithBuyCount
                {
                    BuyCount = "0",
                    CustomerId = x.Id,
                    CustomerName = x.FirstName + " " + x.LastName
                }).OrderBy(x => x.CustomerId).ToListAsync();

                foreach (var item in finalReport)
                {
                    foreach (var us in allUser)
                    {
                        if (!item.CustomerWithBuyCounts.Any(x => x.CustomerId == us.CustomerId))

                            item.CustomerWithBuyCounts.Add(us);
                    }
                    item.CustomerWithBuyCounts = item.CustomerWithBuyCounts.OrderBy(x => x.CustomerId).ToList();

                }
                return (true, "", finalReport);

            }
            catch (Exception ex)
            {

                return (false, JsonConvert.SerializeObject(ex), null);
            }
        }
    }
}
