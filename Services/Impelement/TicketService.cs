using DNTPersianUtils.Core;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketService(ApplicationDbContext dataContext, IAccountService accountService,
            IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _accountService = accountService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<(bool isSuccess, string error)> CreateTicket(CreateTiket model)
        {

            var root = _webHostEnvironment.WebRootPath;

            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (false, "کابر نامعتبر");
            }
            try
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                if (isAdmin)
                {
                    if (model.UserId[0] == "all")
                    {
                        model.UserId = await _dataContext.Users.Where(x => x.Id != user.Id).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        var reciever = await _dataContext.Users.FindAsync(model.UserId[0]);
                        if (reciever == null) return (false, "کاربر یافت نشد");
                    }
                    foreach (var item in model.UserId)
                    {
                        var newTicket = new Ticket()
                        {
                            Content = model.Content,
                            Subject = model.Subject,
                            SenderFile = (model.File != null) ? FileUploader.UploadFile(model.File, root + "/uploads/Ticket/").result : "",
                            TicketPriorityStatus = model.TicketPriorityStatus,
                            CreateDate = DateTime.Now,
                            SenderId = user.Id,
                            ReceiverId = item,
                            TicketStatus = TicketStatus.AdminSenderNotReply

                        };
                        var result = await _dataContext.Tickets.AddAsync(newTicket);

                    }
                }
                else
                {
                    var admin = await _accountService.GetAdmin();
                    var newTicket = new Ticket()
                    {
                        Content = model.Content,
                        Subject = model.Subject,
                        SenderFile = (model.File != null) ? FileUploader.UploadFile(model.File, root + "/Img/Ticket/").result : "",
                        TicketPriorityStatus = model.TicketPriorityStatus,
                        CreateDate = DateTime.Now,
                        SenderId = user.Id,
                        ReceiverId = admin.Id,
                        TicketStatus = TicketStatus.UserSenderNotReply


                    };
                    await _dataContext.Tickets.AddAsync(newTicket);


                }
                await _dataContext.SaveChangesAsync();

                return (true, "با موفقیت ثبت شده است");

            }
            catch (Exception ex)
            {
                return (false, "مشکلی رخ داده است");
                //return (false,ex.InnerException.Message);

            }
        }


        public async Task<(bool isSuccess, string error)> SenderSeenTicket(List<int> ids)
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (false, "کابر نامعتبر");
            }

            try
            {
                foreach (var id in ids)
                {
                    var ticket = await _dataContext.Tickets.FindAsync(id);
                    if (ticket.SenderId != user.Id) return (false, "کابر نامعتبر");

                    ticket.IsSenderSeen = true;
                    ticket.SenderSeenDate = DateTime.Now;

                    _dataContext.Tickets.Update(ticket);
                }
                await _dataContext.SaveChangesAsync();

                return (true, "با موفقیت ثبت شده است");

            }
            catch (Exception ex)
            {

                return (false, "مشکلی رخ داده است");

            }
        }

        public async Task<(bool isSuccess, string error)> ReceiverSeenTicket(List<int> ids)
        {

            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (false, "کابر نامعتبر");
            }

            try
            {
                foreach (var id in ids)
                {
                    var ticket = await _dataContext.Tickets.FindAsync(id);
                    if (ticket.ReceiverId != user.Id) return (false, "کابر نامعتبر");

                    ticket.IsReciverSeen = true;
                    ticket.ReceiverSeenDate = DateTime.Now;

                    _dataContext.Tickets.Update(ticket);
                }
                await _dataContext.SaveChangesAsync();

                return (true, "با موفقیت ثبت شده است");

            }
            catch (Exception ex)
            {

                return (false, "مشکلی رخ داده است");

            }
        }


        public async Task<(bool isSuccess, string error)> AnswerTicket(TicketAdminAnswer model)
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (false, "کابر نامعتبر");
            }
            var root = _webHostEnvironment.WebRootPath;
            try
            {
                var ticket = await _dataContext.Tickets.FindAsync(model.Id);
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    ticket.TicketStatus = TicketStatus.AdminSenderReply;

                }
                else
                {
                    if (ticket.ReceiverId != user.Id) return (false, "کابر نامعتبر");
                    ticket.TicketStatus = TicketStatus.UserSenderReply;


                }
                ticket.AnswerDate = DateTime.Now;
                ticket.Answer = model.Answer;
                ticket.ReceiverFile = (model.AnswerFile != null) ? FileUploader.UploadFile(model.AnswerFile, root + "/Img/Ticket/").result : "";

                _dataContext.Tickets.Update(ticket);
                await _dataContext.SaveChangesAsync();

                return (true, "با موفقیت ثبت شده است");

            }
            catch (Exception ex)
            {

                return (false, "مشکلی رخ داده است");

            }
        }


        public async Task<(TicketNotification model, bool isSuccess, string error)> GetNotifTicket()
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (null, false, "کابر نامعتبر");
            }
            try
                
            {
                var tickets = await _dataContext.Tickets
                    .Include(x => x.Sender)
                    .Include(x => x.Receive)
                    .Where(x => (x.SenderId == user.Id && !string.IsNullOrEmpty(x.Answer) && !x.IsSenderSeen) || (x.ReceiverId == user.Id && !x.IsReciverSeen))
                    .OrderByDescending(x => x.IsReciverSeen)
                    .ThenByDescending(x => x.CreateDate)
                    .ThenBy(x => x.TicketPriorityStatus)
                    .Select(x => new GetAllTicketForAdmin()
                    {
                        Id = x.Id,
                        SenderFile = x.SenderFile,
                        TicketPriorityStatus = x.TicketPriorityStatus,
                        Answer = x.Answer,
                        ReceiverFile = x.ReceiverFile,
                        AnswerDate = (x.AnswerDate != null) ? x.AnswerDate.ToShortPersianDateTimeString(true) : "",
                        Content = x.Content,
                        CreateDate = (x.CreateDate != null) ? x.CreateDate.ToShortPersianDateTimeString(true) : "",
                        Subject = x.Subject,
                        IsSenderSeen = x.IsSenderSeen,
                        SenderFullName = ( _userManager.IsInRoleAsync(x.Sender, "Admin").Result) ? "ادمین" :x.Sender.FirstName+" "+ x.Sender.LastName,
                        ReceiverFullName = (_userManager.IsInRoleAsync(x.Receive, "Admin").Result) ? "ادمین" : x.Receive.FirstName + " " + x.Receive.LastName,
                        HasAnswer = ((x.SenderId != user.Id) && string.IsNullOrEmpty(x.Answer)) ? true : false
                    }).ToListAsync();
                if (tickets == null)
                {
                    return (null, false, "");

                }
                else
                {
                    var finalModel = new TicketNotification()
                    {
                        Count = tickets.Count,
                        Tickets = tickets
                    };
                    return (finalModel, true, "");

                }


            }
            catch (Exception ex)
            {

                return (null, false, "مشکلی رخ داده است");

            }
        }
        public async Task<(List<GetAllTicketForAdmin> model, bool isSuccess, string error)> GetAllTicketForAdmin()
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (null, false, "کابر نامعتبر");
            }
            var isAsmin = await _userManager.IsInRoleAsync(user, "Admin");
            try
            {
                var tickets = await _dataContext.Tickets
                    .Include(x => x.Sender)
                    .Include(x => x.Receive)
                    .OrderBy(x => x.TicketStatus)
                    .ThenByDescending(x => x.CreateDate)
                    .Select(x => new GetAllTicketForAdmin()
                    {
                        Id = x.Id,
                        SenderFile = x.SenderFile,
                        TicketPriorityStatus = x.TicketPriorityStatus,
                        Answer = x.Answer,
                        ReceiverFile = x.ReceiverFile,
                        AnswerDate = (x.AnswerDate != null) ? x.AnswerDate.ToShortPersianDateTimeString(true) : "",
                        Content = x.Content,
                        CreateDate = (x.CreateDate != null) ? x.CreateDate.ToShortPersianDateTimeString(true) : "",
                        Subject = x.Subject,
                        IsSenderSeen = x.IsSenderSeen,
                        SenderFullName = x.SenderId== user.Id&&isAsmin? "ادمین" : x.Sender.FirstName + " " + x.Sender.LastName,

                        ReceiverFullName = x.SenderId == user.Id && isAsmin ? "ادمین" : x.Receive.FirstName + " " + x.Receive.LastName,


                        HasAnswer = ((x.SenderId != user.Id) && string.IsNullOrEmpty(x.Answer)) ? true : false,
                        SenderId = (x.SenderId != user.Id) ? x.SenderId : ""

                    }).ToListAsync();
                if (tickets == null)
                {
                    return (null, false, "");

                }
                else
                {
                    return (tickets, true, "");

                }


            }
            catch (Exception ex)
            {

                //return (null, false, "مشکلی رخ داده است");
                return (null, false, ex.Message);

            }
        }
        public  async Task<bool>IsAdmin(ApplicationUser user,string role)
        {
            var isAdmin=await _userManager.IsInRoleAsync(user, role);
            return isAdmin;
        }
        public async Task<(List<GetAllTicketForCurrectUser> model, bool isSuccess, string error)> GetAllTicketForUser()
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (null, false, "کابر نامعتبر");
            }
            try
            {
                var tickets = await _dataContext.Tickets
                    .Where(x => x.SenderId == user.Id || x.ReceiverId == user.Id)
                    .Include(x => x.Sender)
                    .Include(x => x.Receive)
                    .OrderByDescending(x => x.IsReciverSeen)
                    .ThenByDescending(x => x.CreateDate)
                    .ThenBy(x => x.TicketPriorityStatus)
                    .Select(x => new GetAllTicketForCurrectUser()
                    {
                        Id = x.Id,
                        TicketPriorityStatus = x.TicketPriorityStatus,
                        Answer = x.Answer,
                        Content = x.Content,
                        CreateDate = (x.CreateDate != null) ? x.CreateDate.ToShortPersianDateTimeString(true) : "",
                        Subject = x.Subject,
                        SenderFullName = (( _userManager.IsInRoleAsync(x.Sender, "Admin").Result)) ? "ادمین" :x.Sender.FirstName+" "+ x.Sender.LastName,
                        ReceiverFullName = ((_userManager.IsInRoleAsync(x.Receive, "Admin").Result)) ? "ادمین" : x.Receive.FirstName + " " + x.Receive.LastName,
                        HasAnswer = ((x.SenderId != user.Id) && string.IsNullOrEmpty(x.Answer)) ? true : false
                    }).ToListAsync();
                if (tickets == null)
                {
                    return (null, false, "");

                }
                else
                {
                    return (tickets, true, "");

                }


            }
            catch (Exception ex)
            {

                return (null, false, "مشکلی رخ داده است");

            }
        }

        public async Task<(TicketDetailsForUser model, bool isSuccess, string error)> GetTicketInfo(int id)
        {
            var user = await _accountService.GetCurrectUser();
            if (user == null)
            {
                return (null, false, "کابر نامعتبر");
            }
            try
            {
                var tickets = await _dataContext.Tickets.
                    Where(x => x.Id == id)
                    .Where(x => x.ReceiverId == user.Id || x.SenderId == user.Id)
                    .Select(x => new TicketDetailsForUser()
                    {
                        Id = x.Id,
                        ReceiverFile = x.ReceiverFile,
                        TicketPriorityStatus = x.TicketPriorityStatus,
                        Answer = x.Answer,
                        SenderFile = x.SenderFile,
                        AnswerDate = (x.AnswerDate != null) ? x.AnswerDate.ToShortPersianDateTimeString(true) : "",
                        Content = x.Content,
                        CreateDate = (x.CreateDate != null) ? x.CreateDate.ToShortPersianDateTimeString(true) : "",
                        Subject = x.Subject,
                        IsSenderSeen = x.IsSenderSeen,
                        SenderFullName = (( _userManager.IsInRoleAsync(x.Sender, "Admin").Result)) ? "ادمین" :x.Sender.FirstName+" "+ x.Sender.LastName,
                        ReceiverFullName = ((_userManager.IsInRoleAsync(x.Receive, "Admin").Result)) ? "ادمین" : x.Receive.FirstName + " " + x.Receive.LastName,
                        HasAnswer = ((x.SenderId != user.Id) && string.IsNullOrEmpty(x.Answer)) ? true : false
                    }).FirstOrDefaultAsync();
                if (tickets == null)
                {
                    return (null, false, "");

                }
                else
                {
                    return (tickets, true, "");

                }


            }
            catch (Exception ex)
            {

                return (null, false, "مشکلی رخ داده است");

            }
        }
    }
}
