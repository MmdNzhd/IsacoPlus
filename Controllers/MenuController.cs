using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;

namespace KaraYadak.Controllers
{
    //[Authorize]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public MenuController(ApplicationDbContext context, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _context = context;
        }

        [NonAction]
        public List<string> GetRoutes()
        {
            List<string> arr = new List<string>();
            foreach (ActionDescriptor ad in _actionDescriptorCollectionProvider.ActionDescriptors.Items)
            {
                var action = Url.Action(new UrlActionContext()
                {
                    Action = ad.RouteValues["action"],
                    Controller = ad.RouteValues["controller"],
                    Values = ad.RouteValues
                });
                if (ad.RouteValues["controller"].ToLower() != "home"
                    && ad.RouteValues["controller"].ToLower() != "account")
                {
                    arr.Add(action);
                }
            }
            return arr;
        }

        // GET: Access
        public async Task<IActionResult> Index()
        {
            var items = from m in _context.Menu
                        join a in _context.Access on m.AccessId equals a.Id
                        select new ViewModels.MenuVm
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Icon = m.Icon,
                            AccessId = m.AccessId,
                            AccessName = a.Name,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt,
                            Url = m.Url
                        };
            ViewBag.Accesses = _context.Access.ToList();
            ViewBag.Routes = GetRoutes();
            return View(await items.ToListAsync());
        }

        // GET: Access/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url,Icon,AccessId")] Menu menu)
        {
            menu.CreatedAt = DateTime.Now;
            menu.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewBag.Accesses = _context.Access.ToList();
            ViewBag.Routes = GetRoutes();
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Icon,Url,AccessId")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    menu.UpdatedAt = DateTime.Now;
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccessExists(menu.Id))
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
            return View(menu);
        }

        // GET: Access/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccessExists(int id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }
    }
}
