using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KaraYadak.Controllers
{
    public class MVControllers : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult BlogSingle()
        {
            return View();
        }
    }
}
