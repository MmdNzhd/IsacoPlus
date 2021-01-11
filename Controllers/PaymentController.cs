using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using KaraYadak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KaraYadak.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IPaymentService _paymentService;

        public PaymentController(ApplicationDbContext context, IAccountService accountService, IPaymentService paymentService)
        {
            _context = context;
            _accountService = accountService;
            _paymentService = paymentService;
        }
        [Authorize(Roles = PublicHelper.ADMINROLE)]

        public async Task<IActionResult> Index()
        {
            ViewBag.UserList = await _accountService.GetAllUserForAdmin();

            return View();
        }
        [Authorize(Roles = PublicHelper.WarehousingAdminROLE)]

        [Route("WarehousingAdmin")]
        public async Task<IActionResult> WarehousingAdmin()
        {
            ViewBag.UserList = await _accountService.GetAllUserForAdmin();

            return View();
        }
        public class UserList
        {
            public string PhoneNumber { get; set; }
            public string Fullname { get; set; }
        }


        [Authorize(Roles = PublicHelper.ADMINROLE+","+PublicHelper.WarehousingAdminROLE)]

        public async Task<IActionResult> GetLastOfOrdersForAdmin()
        {
            var error = new List<string>();

            var result = await _paymentService.GetLastOfOrdersForAdmin();
          
            return new JsonResult(result);


        }


        [Authorize(Roles = PublicHelper.ADMINROLE + "," + PublicHelper.WarehousingAdminROLE)]
        [HttpPost]
        [Route("ChangeOrderTypeByWarehousingAdmin")]
        public async Task<IActionResult> ChangeOrderTypeByWarehousingAdmin([FromBody]ChangeOrderTypeByWarehousingAdminViewModel model)
        {
            var error = new List<string>();

            var result = await _paymentService.ChangeOrderTypeByWarehousingAdmin(model);
            if (result.isSuccess)
            {


                return new JsonResult(new { status = 1, message = "با موفقیت انجام شد", data = result.isSuccess });

            }
            else
            {
                return new JsonResult(new { status = 0, message = result.message });

            }


        }

        //[Authorize(Roles = PublicHelper.ADMINROLE)]

        //public async Task<IActionResult> GetOrderDetailForAdmin(int orderId)
        //{
        //    var error = new List<string>();

        //    var result = await _paymentService.OrderDetails(orderId);


        //    error.Add("با موفقیت انجام شد");
        //    return Ok(new ResponseResult(Domain.DTO.Response.StatusCode.ok, error, true, result));



        //}




        [Authorize(Roles = PublicHelper.ADMINROLE + "," + PublicHelper.WarehousingAdminROLE)]
        [HttpPost]

        public async Task<IActionResult> GetUserInfoForMoneyBack(string phoneNumber)
        {
            var error = new List<string>();

            var result = await _paymentService.GetUserInfoForMoneyBack(phoneNumber);
            if (result.isSuccess)
            {
                return new JsonResult(new { status = 1, message = "با موفقیت انجام شد", data = result.model });
            }
            else
            {
                return new JsonResult(new { status = 0, message = result.message });

            }

        }




        [Authorize(Roles = PublicHelper.ADMINROLE + "," + PublicHelper.WarehousingAdminROLE)]
        [HttpPost]
        public async Task<IActionResult> MoneyBack([FromBody] MoneyBackViewModel model)
        {
            var error = new List<string>();

            var result = await _paymentService.MoneyBack(model);
            if (result.isSuccess)
            {
                

                return new JsonResult(new { status = 1, message = "با موفقیت انجام شد", data = result.isSuccess });

            }
            else
            {
                return new JsonResult(new { status = 0, message = result.message });

            }

        }
    }
}
