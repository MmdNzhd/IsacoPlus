using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace KaraYadak
{
    public class TestUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TestUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(bool isSuccess, string error)> CreateTicket(CreateTiket model)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            //var userName= HttpContext.Current.User.Identity.Name;
            return (false, userId);
        }
    }
}
