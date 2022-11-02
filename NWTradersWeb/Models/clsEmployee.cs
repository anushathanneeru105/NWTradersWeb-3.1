using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NWTradersWeb.Models
{


    public partial class Employee
    {

        private NorthwindEntities nwEntities = new NorthwindEntities();

        public string FullName
        { get { return FirstName + " " + LastName; } }


        public IEnumerable<OrderProducts>  AnnualOrders()
        {
            List<OrderProducts> annualOrders = this.Orders.
                GroupBy(od => od.OrderDate.Value.Year).
                    Select(annual => new OrderProducts
                    {
                        Date = annual.Key.ToString(),
                        Number= annual.Count()
                    }).ToList();

                return annualOrders;
        }

        public IEnumerable<OrderProducts> AnnualOrdersInYear(string Year = "")
        {
            DateTime YearBeginning = NWTradersUtilities.BeginningOfYear(Year);
            DateTime YearEnd = NWTradersUtilities.EndOfYear(Year);


            List < OrderProducts> annualOrders = this.Orders.
                Where(od => od.OrderDate >=YearBeginning && od.OrderDate <= YearEnd ).
                GroupBy(od => od.OrderDate.Value.Month).
                    Select(annual => new OrderProducts
                    {
                        Date = NWTradersUtilities.MonthName(annual.Key),
                        Number = annual.Count()
                    }).
                    ToList();

                return annualOrders;
        }


        public IEnumerable<OrderProducts> AnnualProducts()
        {
            List<OrderProducts> annualOrders = this.Orders.
                GroupBy(od => od.OrderDate.Value.Year).
                    Select(annual => new OrderProducts
                    {
                        Date = annual.Key.ToString(),
                        Number = annual.Sum(
                            o=> o.Order_Details.Sum(od=> od.Quantity))
                    }).ToList();

            return annualOrders;
        }


        public IEnumerable<OrderRevenues> AnnualRevenues()
        {
            List<OrderRevenues> annualRevenues = this.Orders.
                GroupBy(od => od.OrderDate.Value.Year).
                    Select(annual => new OrderRevenues
                    {
                        Date = annual.Key.ToString(),
                        Sales= annual.Sum(o => o.TotalSales)
                    }).ToList();

            return annualRevenues;
        }

        public decimal AnnualSales(string Year="")
        {
            decimal annualSales = 0;

            if (string.IsNullOrEmpty(Year))
                annualSales = Orders.Sum(od => od.TotalSales);
            else
                annualSales = Orders.
                    Where(od => 
                        od.OrderDate > NWTradersUtilities.BeginningOfYear(Year) && 
                        od.OrderDate < NWTradersUtilities.EndOfYear(Year)
                     ).
                    Sum(od => od.TotalSales);


            return annualSales;

        }

    }
}