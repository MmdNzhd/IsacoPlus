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
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/Videos/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;

        }

        public VideosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Videos.OrderByDescending(i => i.UpdateAt).ToListAsync());
        }






        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Title,ShortDescription,Text")] Video Video)
        {
            if (ModelState.IsValid)
            {
                Video.CreateAt = DateTime.Now;
                Video.UpdateAt = DateTime.Now;
                _context.Add(Video);
                await _context.SaveChangesAsync();
                return new JsonResult(new { status = "1", message = "با موفقیت بروز رسانی شد" });
            }
            return new JsonResult(new { status = "0", message = "خطایی رخ داده است" });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { status = "0", message = "ویدیو یافت نشد" });
            }

            var Video = await _context.Videos.FindAsync(id);
            if (Video == null)
            {
                return new JsonResult(new { status = "0", message = "ویدیو یافت نشد" });
            }
            return PartialView(Video);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ShortDescription,Text")] Video Video, IFormFile Image)
        {
            if (id != Video.Id)
            {
                return new JsonResult(new { status = "0", message = "اخباری یافت نشد" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Video.UpdateAt = DateTime.Now;
                    Video.Image = (Image != null) ? upload(Image) : "";
                    _context.Videos.Update(Video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideosExist(Video.Id))
                    {
                        return new JsonResult(new { status = "0", message = "اخباری یافت نشد" });
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
        public async Task<IActionResult> VideoDetails(int id)
        {

            var Video = _context.Videos.Find(id);
            if (Video != null)
            {
                return View(Video);
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
                return new JsonResult(new { status = "0", message = "اخباری یافت نشد" });
            }

            var Video = await _context.Videos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Video == null)
            {
                return new JsonResult(new { status = "0", message = "اخباری یافت نشد" });
            }
            _context.Videos.Remove(Video);
            await _context.SaveChangesAsync();
            return new JsonResult(new { status = "1", message = "با موفقیت  حذف شد" });
        }

        private bool VideosExist(int id)
        {
            return _context.Videos.Any(e => e.Id == id);
        }
    }
}
