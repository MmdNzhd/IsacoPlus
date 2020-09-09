using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;

namespace KaraYadak.Controllers
{
    [Authorize]
    public class AccessController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Access
        public async Task<IActionResult> Index()
        {
            return View(await _context.Access.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Access access)
        {
            access.CreatedAt = DateTime.Now;
            access.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(access);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(access);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var access = await _context.Access.FindAsync(id);
            if (access == null)
            {
                return NotFound();
            }
            return View(access);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Access access)
        {
            if (id != access.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    access.UpdatedAt = DateTime.Now;
                    _context.Update(access);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccessExists(access.Id))
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
            return View(access);
        }

        // GET: Access/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var access = await _context.Access
                .FirstOrDefaultAsync(m => m.Id == id);
            if (access == null)
            {
                return NotFound();
            }

            _context.Access.Remove(access);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccessExists(int id)
        {
            return _context.Access.Any(e => e.Id == id);
        }
    }
}
