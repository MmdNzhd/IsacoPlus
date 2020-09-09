using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KaraYadak.Controllers
{
    public class DesignerController : Controller
    {
        public IActionResult AllDesigners()
        {
            return View();
        }
        public IActionResult DesignerDetails(int id)
        {
            return View();
        }
    }
}
