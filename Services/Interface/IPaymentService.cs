
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KaraYadak
{
    public interface IPaymentService
    {

        Task<List<OrderListForAdminViewModel>> GetLastOfOrdersForAdmin();
        Task<(bool isSuccess, string message)> ChangeOrderTypeByWarehousingAdmin(ChangeOrderTypeByWarehousingAdminViewModel model);
        Task<(bool isSuccess, string message,double? discount)> CheckGiftCode(string giftCode);
        Task<(bool isSuccess, string message,MoneyBackViewModel model)> GetUserInfoForMoneyBack(string phoneNumber);
        Task<(bool isSuccess, string message)> MoneyBack(MoneyBackViewModel model);


    }
}
