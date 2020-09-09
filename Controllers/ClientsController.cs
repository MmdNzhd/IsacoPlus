using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authorization;
using DNTPersianUtils.Core;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly double itemsPerPage = 10;
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
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

            var items = await _context.Clients
                .ToListAsync();

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
                    s.Address,
                    s.Email,
                    Name = s.FirstName + " " + s.LastName,
                    s.Phone,
                    Status = s.Status.ToString(),
                    UpdatedAt = s.UpdatedAt.ToFriendlyPersianDateTextify()
                })
                .Skip((int)itemsPerPage * (page.Value - 1))
                .Take((int)itemsPerPage)
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Phone,Address,Status")] Client client)
        {
            //if(client.Status == ClientStatus.)
            client.CreatedAt = DateTime.Now;
            client.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return Json("ok");
            }
            return Json("no");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,Address,Status")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.UpdatedAt = DateTime.Now;
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json("404");
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return Json("404");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Json("ok");
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}