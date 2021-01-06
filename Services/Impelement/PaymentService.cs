using DNTPersianUtils.Core;
using EFCore.BulkExtensions;
using KaraYadak.Data;
using KaraYadak.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Parbad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class PaymentService : IPaymentService
    {
        private readonly IOnlinePayment _onlinePayment;
        private readonly ApplicationDbContext _dataContext;
        private readonly IAccountService _accountService;
        private readonly ITicketService _ticketService;
        private readonly ISmsSender _smsService;

        public PaymentService(IOnlinePayment onlinePayment, ApplicationDbContext dataContext, IAccountService accountService,
            ITicketService ticketService, ISmsSender smsService)
        {
            _onlinePayment = onlinePayment;
            _dataContext = dataContext;
            _accountService = accountService;
            _ticketService = ticketService;
            _smsService = smsService;
        }


        //public async Task<(bool isSuccess, string message, double? discount)> CheckGiftCode(string giftCode)
        //{
        //    var user = await _accountService.GetCurrectUser();

        //    var giftCart = await _dataContext.GiftCarts.FirstOrDefaultAsync(x => x.GiftCode == giftCode && x.EmployerId == user.Id);

        //    if (giftCart == null)
        //    {
        //        giftCart = await _dataContext.GiftCarts.FirstOrDefaultAsync(x => x.GiftCode == giftCode);
        //        if (giftCart == null)
        //        {
        //            return (false, "کد وارد شده معتبر نیست", null);

        //        }
        //    }

        //    if (string.IsNullOrEmpty(giftCart.EmployerId))
        //    {
        //        if (giftCart.IsUse)
        //        {
        //            return (false, "شما قبلا از این کد استفاده کردین", null);
        //        }
        //        if (giftCart.ExpireTime < DateTime.Now)
        //        {
        //            return (false, "تاریخ مصرف این کد به پایان رسیده است", null);
        //        }
        //    }
        //    else
        //    {
        //        if (user == null && !user.IsActive || !user.IsActive)
        //        {
        //            return (false, "کاربر نامعتبر", null);
        //        }
        //        if (giftCart.EmployerId != user.Id)
        //        {
        //            return (false, "کد وارد شده معتبر نیست", null);
        //        }

        //        if (giftCart.IsUse)
        //        {
        //            return (false, "شما قبلا از این کد استفاده کردین", null);
        //        }
        //        if (giftCart.ExpireTime < DateTime.Now)
        //        {
        //            return (false, "تاریخ مصرف این کد به پایان رسیده است", null);
        //        }
        //    }

        //    return (true, "", giftCart.Discount);


        //}

        public async Task<(bool isSuccess, string message, MoneyBackViewModel model)> GetUserInfoForMoneyBack(string phoneNumber)
        {

            try
            {
                //_dataContext.BulkInsert( new List<Payment>());
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

                if (user == null) return (false, "کابری یافت نشد", null);
                if (user.CartNumber == null) return (false, "اطلاعات شماره کارت کاربر تکمیل نشده است", null);
                var lastFactor = await _dataContext.Payments.Include(x => x.ShoppingCart)
                                                            .Where(x => x.UserId == user.Id && !x.IsBackMOney&&x.IsSucceed)
                                                           .OrderByDescending(x => x.Date)
                                                           .FirstOrDefaultAsync();
                if (lastFactor == null) return (false, "این کابر هیچ تراکنش مالی برای بازگشت پول ندارد  ", null);
              
                var model = new MoneyBackViewModel()
                {
                    Price = Convert.ToDouble(lastFactor.Amount),
                    ShebaNumber = user.CartNumber,
                    PlanId = lastFactor.ShoppingCartId.Value,
                    CompanyId = lastFactor.UserId,
                    FactorId = lastFactor.Id
                };
                return (true, "", model);

            }
            catch (Exception ex)
            {

                var err = "مشکلی رخ داده است";
                return (false, err, null);

            }
        }

        public async Task<(bool isSuccess, string message)> MoneyBack(MoneyBackViewModel model)
        {
            try
            {
                var user = await _dataContext.Users.FindAsync(model.CompanyId);
                if (user == null) return (false, "کاربر نا معتبر");
                var lastFactor = await _dataContext.Payments.FindAsync(model.FactorId);
                lastFactor.IsBackMOney = true;
                var factor = new Payment()
                {
                    UserId = model.CompanyId,
                    Date = DateTime.Now,
                    ShoppingCartId = model.PlanId,
                    Amount = Convert.ToDecimal(model.Price),
                    TrackingNumber = model.TrackingCode,
                    IsBackMOney = true,
                };
                var userId = new List<string>();
                userId.Add(model.CompanyId);

                await _dataContext.Payments.AddAsync(factor);
                await _dataContext.SaveChangesAsync();
                var ticket = new CreateTiket()
                {
                    Subject = "بازگشت پول",
                    Content = $"کابر گرامی مبلغ {model.Price} به حساب شما واریز شده است" +
                    $"شماره پیگیری : {model.TrackingCode}",
                    TicketPriorityStatus = Models.TicketPriorityStatus.Immediate,
                    UserId = userId,

                };

                await _ticketService.CreateTicket(ticket);


                //   send sms to user
                await _smsService.SendWithPattern(user.PhoneNumber, "2d0fik6wla", JsonConvert.SerializeObject(new { trackingNubmber=factor.TrackingNumber }));

                return (true, "");
            }
            catch (Exception ex)
            {

                var err = "مشکلی رخ داده است";
                return (false, err);

            }

        }

        public async Task<List<OrderListForAdminViewModel>> GetLastOfOrdersForAdmin()
        {
            try
            {
                var lastOfOrder = _dataContext.Payments
               .Include(x => x.User)
              .OrderByDescending(x => x.Date)
              .Select(x => new OrderListForAdminViewModel
              {
                  Date = x.Date.ToShortPersianDateTimeString(true),
                  OrderId = x.Id,
                  phoneNumber = x.User.PhoneNumber,
                  Price = x.Amount + x.Discount,
                  PriceWithTax = x.FinallyAmountWithTax,
                  OrderType = (x.IsBackMOney)?"بازگشت پول":(x.FinallyAmountWithTax == 0) ? "هدیه" : "پرداخت آنلاین",
                  CompanyName = x.User.FirstName + " " + x.User.LastName,
                  Issuccess = (x.IsSucceed) ? "موفق" : "ناموفق",
                  Discount = x.Discount,
                  PriceWithDiscount = x.Amount

              }).AsNoTracking().AsQueryable();
                var finalModel = await lastOfOrder.ToListAsync(); 

                

                return finalModel;
            }
            catch (Exception ex)
            {


                return (null);
            }
        }

        public Task<(bool isSuccess, string message, double? discount)> CheckGiftCode(string giftCode)
        {
            throw new NotImplementedException();
        }
    }
}
