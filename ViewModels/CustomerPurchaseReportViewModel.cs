using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class CustomerPurchaseReportViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BuyCount { get; set; }
        public string Discount { get; set; }
        public string Price { get; set; }
        public DateTime Date { get; set; }
    }

    public class CustomersWhitProductReportViewModel
    {
        public string FactorId { get; set; }
        public string PersianDate { get; set; }
        public string BuyCount { get; set; }
        public string Discount { get; set; }
        public string Price { get; set; }
        public string PriceWithDisCount { get; set; }
        public DateTime Date { get; set; }
        public List<ProductCodeWithId> ProductsCode { get; set; }
        public string UserId { get; set; }
    }
    public class ProductCodeWithId
    {
        public string Code { get; set; }
        public string Id { get; set; }
    }

    public class CustomersPurchaseReportViewModel
    {
        public string CustomerName { get; set; }
        public string BuyCount { get; set; }
        public string FactorCount { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Discount { get; set; }
        public string Price { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
    }
    public class ProductWithCustomerName
    {
        public string customerName { get; set; }
        public string customerId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }

    public class ProductWhitCustomersReportViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime Date { get; set; }
        public List<CustomerWithBuyCount> CustomerWithBuyCounts { get; set; }
    }

    public class ProductWhitCustomersReport2ViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime Date { get; set; }
        public List<List<CustomerWithBuyCount>> CustomerWithBuyCounts { get; set; }
    }
    public class CustomerWithBuyCount
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BuyCount { get; set; }
    }
}
