using KaraYadak.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Extention
{
    public class SiteVisitCounter
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SiteVisitCounter> _logger;
        //private readonly IActionContextAccessor _accessor;
        private readonly ApplicationDbContext _context;

        public SiteVisitCounter(RequestDelegate next, ILogger<SiteVisitCounter> logger
            )
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_accessor = accessor;
        }

        public async Task Invoke(HttpContext context,ApplicationDbContext _context/*, IActionContextAccessor _accessor*/)
        {
            var error = new List<string>();

            try
            {
                #region GetSiteCount
                var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

                //var ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();

                //add site visit
                if (!await _context.SiteVisits.AnyAsync(x => x.Ip == ip && x.Date.Day == DateTime.Now.Day))
                {
                    var visit = new Models.SiteVisit()
                    {
                        Date = DateTime.Now,
                        Ip = ip
                    };
                    await _context.SiteVisits.AddAsync(visit);
                    await _context.SaveChangesAsync();

                }


                #endregion

                await _next.Invoke(context);
            }
            catch (Exception ex)

            {

            }

            
        }
    }
}
