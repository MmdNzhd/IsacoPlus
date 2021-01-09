using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Data;
using KaraYadak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = PublicHelper.ADMINROLE + "," + PublicHelper.USERROLE)]

    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IAccountService _accountService;
        private readonly ITicketService _ticketService;

        public TicketsController(ApplicationDbContext dataContext, IAccountService accountService, ITicketService ticketService)
        {
            _dataContext = dataContext;
            _accountService = accountService;
            _ticketService = ticketService;
        }


        [HttpGet()]
        public async Task<IActionResult> Index()
        {

            ViewBag.UserList = await (from r in _dataContext.Roles
                                      where r.Name != "Admin"
                                      join ur in _dataContext.UserRoles
                                      on r.Id equals ur.RoleId
                                      join u in _dataContext.Users
                                      on ur.UserId equals u.Id
                                      select new UserList { Fullname = u.FirstName + " " + u.LastName, Id = u.Id }).ToListAsync();

            return View();
        }
        public class UserList
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
        }


        [HttpPost]
        //[AllowAnonymous]

        public async Task<ActionResult> CreateTicket([FromForm] CreateTiket model)
        {
            string message = "";
            var result = await _ticketService.CreateTicket(model);
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }



        [HttpPost]
        public async Task<ActionResult> SenderSeenTicket([FromBody] listOfIds model)
        {
            string message = "";
            var result = await _ticketService.SenderSeenTicket(model.ids);
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }

        [HttpPost]
        public async Task<ActionResult> ReceiverSeenTicket([FromBody] listOfIds model)
        {
            string message = "";
            var result = await _ticketService.ReceiverSeenTicket(model.ids);
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }



        [HttpPost]
        public async Task<ActionResult> AnswerTicket([FromForm] TicketAdminAnswer model)
        {
            string message = "";
            var result = await _ticketService.AnswerTicket(model);
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }









        [HttpGet]
        public async Task<ActionResult> GetTicketInfo(int id)
        {
            string message = "";
            var result = await _ticketService.GetTicketInfo(id);
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message, data = result.model });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }



        [HttpGet]
        public async Task<ActionResult> GetNotifTicket()
        {
            string message = "";
            var result = await _ticketService.GetNotifTicket();
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }


        [AllowAnonymous]
        [Authorize(Roles = PublicHelper.ADMINROLE)]

        [HttpGet]
        public async Task<ActionResult> GetAllTicketForAdmin(int id)
        {
            string message = "";
            var result = await _ticketService.GetAllTicketForAdmin();
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(result.model);
            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }
        [AllowAnonymous]
        [Authorize(Roles = PublicHelper.ADMINROLE)]

        [HttpGet]
        public async Task<ActionResult> GetNotifTicketForAdmin()
        {
            string message = "";
            var result = await _ticketService.GetNotifTicket();
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(new { status = 1, message = message });

            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }

        [AllowAnonymous]
        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpGet]
        public async Task<ActionResult> GetNotifTicketCountForAdmin()
        {
            string message = "";
            var result = await _ticketService.GetNotifTicket();
            if (result.isSuccess)
            {
                message = "با موفقیت انجام شد";
                return new JsonResult(result.model.Count);

            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }
        [AllowAnonymous]
        [Authorize(Roles = PublicHelper.USERROLE)]


        [HttpGet]

        public async Task<ActionResult> GetAllTicketForUser()
        {
            string message = "";
            var result = await _ticketService.GetAllTicketForUser();
            if (result.isSuccess)
            {
                return new JsonResult(new { status = 1, message = message });

            }
            else
            {
                message = result.error;
                return new JsonResult(new { status = 0, message = message });

            }
        }


        //[AllowAnonymous]
        [Route("UserTicket")]
        public async Task<ActionResult> UserTicket()
        {
            var result = await _ticketService.GetAllTicketForUser();
            var userId = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            ViewBag.UserId = userId.Id;
            if (result.model == null) result.model = new List<GetAllTicketForCurrectUser>();
            return View(result.model);
        }


    }

    public class listOfIds
    {
        public List<int> ids { get; set; }
    }
}
