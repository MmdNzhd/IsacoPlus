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
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/Blogs/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;

        }

        public BlogsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.OrderByDescending(i => i.UpdateAt).ToListAsync());
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ShortDescription,Text")] Blog blog, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                blog.CreateAt = DateTime.Now;
                blog.UpdateAt = DateTime.Now;
                blog.Image = (blog != null) ? upload(Image) : "";
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "با موفقیت بروز رسانی شد" });
            }
            return new JsonResult(new { status = "0", message = "خطایی رخ داده است" });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
            }
            return PartialView(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ShortDescription,Text")] Blog blog, IFormFile Image)
        {
            if (id != blog.Id)
            {
                return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blog.UpdateAt = DateTime.Now;
                    blog.Image = (Image  != null) ? upload(Image) : "";
                    _context.Blogs.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExist(blog.Id))
                    {
                        return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
                    }
                    else
                    {
                        return new JsonResult(new { status = "0", message = "خطایی رخ داده است" });
                    }
                }
                return new JsonResult(new { status = "1", message = "با موفقیت بروز رسانی شد" });
            }
            return new JsonResult(new { status = "0", message = "خطایی رخ داده است" });
        }
        [AllowAnonymous]
        public async Task<IActionResult> BlogDetails(int id)
        {

            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                return View(blog);
            }
            else
            {
                return View(null);
            }

        }
        

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return new JsonResult(new { status = "0", message = "بلاگی یافت نشد" });
            }
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return new JsonResult(new { status = "1", message = "با موفقیت  حذف شد" });
        }

        private bool BlogsExist(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
