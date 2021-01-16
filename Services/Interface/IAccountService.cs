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
        Task<ApplicationUser> GetWarehousingAdmin();
        Task<(bool isSuccess, string error)> ChangeWarehousingAdmin(string phoneNumber);
        Task<List<AllUserForAdminViewModel>> GetAllUserForAdmin(string startDate,string endDate);
        Task<(bool isSuccess,string error)> BlockUser(string userId);
        Task<(bool isSuccess,string error)> UnBlockUser(string userId);
        Task<(bool isSuccess,string error)> GetUserAddress(string phoneNumber);
        Task<(bool isSuccess,string error, UserInfoForReportViewModel model)> GetUserInfoForReport(string userId);
        Task<(bool isSuccess,string error, List<CustomerPurchaseReportViewModel> model)> GetCustomerPurchaseReport(string userId,string startDate,string endDate);

        Task<(bool isSuccess, string error, List<CustomersWhitProductReportViewModel> model)>
           CustomersWhitProductReport(string userId,string productCode, string startDate, string endDate);


        Task<(bool isSuccess, string error, List<CustomersPurchaseReportViewModel> model)>
       GetCustomersPurchaseReport(string userId, string productCode, string startDate, string endDate);


        Task<(bool isSuccess, string error, List<ProductWhitCustomersReportViewModel> model)>
       GetProductWhitCustomersReport(string userId, string productCode, string startDate, string endDate);


    }
}
