using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Services
{
    public interface IAccountService
    {
        Task<ApplicationUser> GetCurrectUser();
        Task<ApplicationUser> GetAdmin();
        Task<List<AllUserForAdminViewModel>> GetAllUserForAdmin();
        Task<(bool isSuccess,string error)> BlockUser(string userId);
        Task<(bool isSuccess,string error)> UnBlockUser(string userId);
        Task<(bool isSuccess,string error)> GetUserAddress(string phoneNumber);



    }
}
