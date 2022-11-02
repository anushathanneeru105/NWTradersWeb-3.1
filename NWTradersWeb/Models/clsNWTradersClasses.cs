using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NWTradersWeb.Models
{
    public class OrderProducts
    {
        public string Date;
        public int Number;
        public string ProductName;
    }
    public class TopOrderProducts
    {
        public string Date;
        public string ProductName;
        public string sales;
    }
    public class OrderRevenues 
    { 
        public string Date; 
        public decimal Sales; 
    }

    public class EmployeeSales 
    { 
        public string theEmployee; 
        public decimal Sales;
    }

    public class ProductSales
    {
        public string ProductName;
        public string ProductCategory;
        public string Year;
        public decimal Sales;

        public IEnumerable<ProductSales> products;
    }

    public class CustomerSales
    {
        public string theCustomer;
        public string SalesPeriod;
        public decimal Sales;
        public int numberOfOrders;
        public int numberOfProducts;
        public Product Product;
    }

    public class AnnualOrders
    {
        string year;

    }

}