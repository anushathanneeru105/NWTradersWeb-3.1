using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NWTradersWeb.Models
{


    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
        #region shopping cart functions
        public Order myShoppingCart;

        public void CreateNewOrder()
        {
            myShoppingCart = new Order()
            {
                // Fill in the customer ID and the order date.
                CustomerID = CustomerID,
                OrderDate = DateTime.Today,

                // Add some defaults to the new order.
                ShippedDate = DateTime.Today.AddDays(7),
                RequiredDate = DateTime.Today.AddDays(14),

                // Shipping defaults
                ShipName = this.CompanyName,
                ShipAddress = this.Address,
                ShipCity = this.City,
                ShipRegion = this.Region,
                ShipPostalCode = this.PostalCode,
                ShipCountry = this.Country,

            };
        }

        public IEnumerable<Order> RecentOrders(int HowMany = 5)
        {
            return Orders.OrderByDescending(o => o.OrderDate).Take(HowMany);
        }

        public IEnumerable<Product> RecentProducts(int HowMany = 5)
        {

            var recentProducts = this.Orders.SelectMany(od => od.Order_Details).
                GroupBy(od => new { od.ProductID }).
                Select(g => new
                {
                    Product = g.FirstOrDefault().Product,
                    BoughtOn = g.Max(od => od.Order.OrderDate)
                }).
                OrderByDescending(pd => pd.BoughtOn);

            //OrderByDescending(p => p.BoughtOn).
            var recent = recentProducts.Where(p => p.Product.Discontinued == false).
            Select(p => p.Product).
            Take(HowMany);


            return recent.ToList();

        }

        public IEnumerable<Product> FavoriteProducts(int HowMany = 5)
        {

            var theFavorites = this.Orders.SelectMany(od => od.Order_Details).
                GroupBy(od => od.Product).
                Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(od => od.Quantity)
                }).
                OrderByDescending(p => p.Quantity);

            var Favorites = theFavorites.
                Where(p => p.Product.Discontinued == false).
                Select(p => p.Product).
                Take(HowMany).
                ToList();

            return Favorites;

        }

        #endregion

        #region Annual Sales Functions
        public IEnumerable<CustomerSales> CustomerAnnualSales()
        {
           List<CustomerSales> list = new List<CustomerSales>();

            list = this.Orders.
                GroupBy(o => o.OrderDate.Value.Year).
                Select(ordersInYears =>
                new CustomerSales
                {
                    theCustomer = this.CompanyName,
                    numberOfOrders = ordersInYears.Count(),
                    SalesPeriod = ordersInYears.Key.ToString(),
                    Sales = ordersInYears.Sum(od => od.orderTotal)
                    //  numberOfProducts = sales.SelectMany()

                }).ToList();

            return list;
        }

        #endregion

        public IEnumerable<CustomerSales> CustomerTopProducts(int HowMany = 10)
        {
            List<ProductSales> list = new List<ProductSales>();

            list = this.Orders.Select(od => new { od.OrderDate.Value.Year, od.Order_Details }).
                GroupBy(od => od.Year).
                Select(g => new ProductSales
                {
                    Year = g.Key.ToString(),
                                      
                    var temp = g.SelectMany(od => od.Order_Details).
                    GroupBy(od => od.Product).
                    Select(od => new ProductSales
                    {
                        ProductName = g.Key.ToString(),
                        Sales = g.Sum( pod => pod.Order_Details.Sum(temp => temp.Quantity))
                    }).
                    OrderByDescending(p => p.Sales).
                    Take(HowMany).ToList()
        }).ToList();
                

            return list;
        }
    }
}