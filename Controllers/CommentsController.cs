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
    //   [Authorize(Roles = "admin")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public CommentsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.Where(x => x.Status.Equals(CommentStatus.در_حال_بررسی)).OrderByDescending(i => i.Date).ToListAsync());
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
