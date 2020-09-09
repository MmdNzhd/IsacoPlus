using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;

namespace KaraYadak.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CommentController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _hostingEnvironment.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
        public async Task<ActionResult> Index()
        {
            var items = await _context.Comments.Include(x => x.Product).OrderByDescending(x => x.Date).ToListAsync();
            return View(items);
        }
        public async Task<IActionResult> Accept(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.Status = Models.CommentStatus.تایید_شده;
            await _context.SaveChangesAsync();
            return Json(new { status = '1', message = "با موفقیت انجام شد" });
        }
        public async Task<IActionResult> Decline(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.Status = Models.CommentStatus.رد_شده;
            await _context.SaveChangesAsync();
            return Json(new { status = '1', message = "با موفقیت انجام شد" });
        }
        public ActionResult Details(IFormFile image)
        {
            var s = _hostingEnvironment.WebRootPath + "/uploads/" + "637271913469540954Green-Embroidered-Worked-Fabric-3401-5.jpeg";
            var file = new FileInfo(s);

            var before = file.Length;
            var optimizer = new ImageOptimizer();
            optimizer.Compress(file);

            file.Refresh();

            var after = file.Length;
            return Json(new { before , after , file.Attributes });
        }

        // GET: CommentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
