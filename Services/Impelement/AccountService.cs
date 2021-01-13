using KaraYadak.Data;
using KaraYadak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaraYadak.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<ApplicationUser> GetAdmin()
        {
            var admin = await (from r in _context.Roles
                               where r.Name == "Admin"
                               join ur in _context.UserRoles
                               on r.Id equals ur.RoleId
                               join u in _context.Users
                               on ur.UserId equals u.Id
                               select u).AsNoTracking().FirstOrDefaultAsync();
            return (admin);

        }

        public async Task<ApplicationUser> GetCurrectUser()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            return (user);
        }

        public async Task<List<AllUserForAdminViewModel>> GetAllUserForAdmin()
        {
            try
            {

                //var finalModel = _context.Users.Join(
                //    _context.Factors,
                //    user => user.UserName,
                //    factor => factor.UserName,
                //    (user, factor) => new AllUserForAdminViewModel()


                

                var finalModel = _context.Users.Include(x=>x.Payments).Select(x => new AllUserForAdminViewModel
                    {
                        Id = x.Id,
                        FullName = x.FirstName + " " + x.LastName,
                        Address = x.Address,
                        City = x.City,
                        Logo = x.AvatarUrl,
                        Email = x.Email,
                        PhoneNUmber = x.PhoneNumber,
                        Province = x.Province,
                        PostalCode = x.PostalCode,
                        IsActive = IsEnable(x),
                        CountOfOrder =x.Payments.Count.ToString(),
                        SumOfOrder =( x.Payments.Count > 0)?x.Payments.Sum(x=>x.FinallyAmountWithTax).ToString():""
                }).AsNoTracking().AsQueryable();
                    

                return await finalModel.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool isSuccess, string error)> BlockUser(string userId)
        {
            
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return (false, "کاربر یافت نشد");
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddDays(3000);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false,"خطایی رخ داده است");

            }

        }

        public async Task<(bool isSuccess, string error)> UnBlockUser(string userId)
        {

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return (false, "کاربر یافت نشد");
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");

            }
        }

        public async Task<(bool isSuccess, string error)> GetUserAddress(string phoneNumber)
        {
            try
            {
                var user =await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (user == null || user.LockoutEnabled) return (false, "کاربر نا معتبر");
                return (true, user.Address);
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");
            }
        }

        public static bool IsEnable(ApplicationUser user)
        {
            if (user.LockoutEnabled) return false;
            else return true;
        }

        public async Task<ApplicationUser> GetWarehousingAdmin()
        {
            var admin = await (from r in _context.Roles
                               where r.Name == PublicHelper.WarehousingAdminROLE
                               join ur in _context.UserRoles
                               on r.Id equals ur.RoleId
                               join u in _context.Users
                               on ur.UserId equals u.Id
                               select u).AsNoTracking().FirstOrDefaultAsync();
            return (admin);

        }

        public async Task<(bool isSuccess, string error)> ChangeWarehousingAdmin(string phoneNumber)
        {
            try
            {
                var regex = new Regex(@"^(\+98|0)?9\d{9}$");
                if (!regex.IsMatch(phoneNumber)) return (false, "شماره وارد شده نا معتبر میباشد");
                if(await _context.Users.AnyAsync(x => x.UserName == phoneNumber))
                {
                    return (false, "کاربری با این شماره ثبت شده است");
                }
                var admin = await (from r in _context.Roles
                                   where r.Name == PublicHelper.WarehousingAdminROLE
                                   join ur in _context.UserRoles
                                   on r.Id equals ur.RoleId
                                   join u in _context.Users
                                   on ur.UserId equals u.Id
                                   select u).AsNoTracking().FirstOrDefaultAsync();

                if (admin == null) return (false, "کاربری یافت نشد");
                admin.PhoneNumber = phoneNumber;
                admin.UserName = phoneNumber;
                admin.NormalizedUserName = phoneNumber;
                _context.Users.Update(admin);
                await _context.SaveChangesAsync();
                return (true, "");
            }
            catch (Exception)
            {

                return (false, "خطایی رخ داده است");
            }
        }
    }
}
