using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using KaraYadak.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BanerController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public BanerController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? page, int? draw, int start, int length)
        {
            if (page is null)
            {
                page = 1;
            }

            var items = await _context.Baners.OrderByDescending(x=>x.CreateAt).ToListAsync();

            int recordsTotal = items.Count();

            items.Skip(start).Take(length);

            int recordsFiltered = items.Count();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data = items.Select(s => new { s.Id, s.Url, s.Image, UpdatedAt = s.CreateAt.ToFriendlyPersianDateTextify() })
                //.Skip((int)itemsPerPage * (page.Value - 1))
                //.Take((int)itemsPerPage)
            });

        }
        

        [HttpPost]
        public async Task<IActionResult> Create(string Url, string Time, string Date, IFormFile Image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Time) || Time.Equals("12:undefined AM")) Time = "00:00 AM";
                if (string.IsNullOrWhiteSpace(Date)) Date = DateTime.Today.AddDays(1).ToShamsi();
                PersianCalendar pc = new PersianCalendar();
                if (Time.Length < 8) Time = "0" + Time;
                int hour = int.Parse(Time.Substring(0, 2));
                int minute = int.Parse(Time.Substring(3, 2));
                if (Time.Substring(6, 2) != "AM")
                {
                    hour += 12;
                }
                string[] d = Date.Split('/');
                var year = int.Parse(d[0]);
                var month = int.Parse(d[1]);
                var day = int.Parse(d[2]);
                DateTime x = new DateTime(1399, 2, 2, 10, 10, 10, new PersianCalendar());
                DateTime dt = new DateTime(year, month, day, hour, minute, 0, new PersianCalendar());

                var baner = new Baner()
                {
                    CreateAt = DateTime.Now,
                    Date = dt,
                    Image = (Image != null) ? upload(Image) : "",
                    Url = Url
                };
                await _context.Baners.AddAsync(baner);
                await _context.SaveChangesAsync();

                return new JsonResult(new { status = 1, message = "با موفقیت اضافه شد" });
            }
            catch (Exception ex)
            {

                return Json(ex);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json("404");
            }

            var baner = await _context.Baners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baner == null)
            {
                return Json("404");
            }

            _context.Baners.Remove(baner);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool ProductCategoryTypeExists(int id)
        {
            return _context.ProductCategoryTypes.Any(e => e.Id == id);
        }
    }
}
