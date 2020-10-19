using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using DNTPersianUtils.Core;

namespace KaraYadak.Controllers
{

    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly double itemsPerPage = 10;



        public CommentsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.Where(x => x.Status.Equals(CommentStatus.در_حال_بررسی)).OrderByDescending(i => i.Date).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [Route("ContactUsManager")]
        public async Task<IActionResult> ContactUsManager()
        {
            var model = _context.contactUsMessages.OrderByDescending(x => x.CreateAt).AsNoTracking().AsQueryable();
            var finalModel = await model.ToListAsync();
            return View(model);

        }
        public async Task<IActionResult> ContactUsManagerData(int? page, int? draw, int start, int length)
        {
            if (page is null)
            {
                page = 1;
            }

            var items = await _context.contactUsMessages.ToListAsync();

            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data = items.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.PhoneNumber,
                    s.Email,
                    s.Text,
                    UpdatedAt = s.CreateAt.ToFriendlyPersianDateTextify()
                })
                .Skip((int)itemsPerPage * (page.Value - 1))
                .Take((int)itemsPerPage)
            });

        }






        [HttpPost]
        public async Task<IActionResult> AcceptComment(string[] ids)
        {
            try
            {
                foreach (var item in ids)
                {
                    var comment = _context.Comments.Find(int.Parse(item));
                    comment.Status = CommentStatus.تایید_شده;
                    _context.Entry(comment).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "با موفقیت ثبت شد" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { status = "0", message = "خطایی رخ داده است", exception = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectComment(string[] ids)
        {
            try
            {
                foreach (var item in ids)
                {
                    var comment = _context.Comments.Find(int.Parse(item));
                    comment.Status = CommentStatus.رد_شده;
                    _context.Entry(comment).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "با موفقیت ثبت شد" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { status = "0", message = "خطایی رخ داده است", exception = e.Message });
            }
        }


    }
}
