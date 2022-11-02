using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NWTradersWeb.Models;

namespace NWTradersWeb.Controllers
{
    public class EmployeesController : Controller
    {
        private NorthwindEntities nwEntities = new NorthwindEntities();

        #region Utilities

        public double CompanyAverageAnnualOrders()
        {
            double averageAnnualOrders = 0D;

            averageAnnualOrders = nwEntities.
                Orders.
                Where(o => o.OrderDate.Value.Year <= 2020).
                GroupBy(empOrder => new
                {
                    emp = empOrder.EmployeeID,
                    Year = empOrder.OrderDate.Value.Year
                }).
                    Select(avgOrder => new
                    {
                        nOrders = avgOrder.Count()
                    }).Average(o => o.nOrders);


            return averageAnnualOrders;
        }

        public double EmployeeAverageAnnualOrders(int? EmployeeID)
        {
            double averageAnnualOrders = 0D;

            if ((EmployeeID == null) || (nwEntities.Employees.Find(EmployeeID) == null))
                return averageAnnualOrders;

            averageAnnualOrders = nwEntities.
                Orders.
                Where(o => o.EmployeeID == EmployeeID).
                Where(o => o.OrderDate.Value.Year <= 2020).
                GroupBy(empOrder => new
                {
                    emp = empOrder.EmployeeID,
                    Year = empOrder.OrderDate.Value.Year
                }).
                    Select(avgOrder => new
                    {
                        nOrders = avgOrder.Count()
                    }).
                    Average(o => o.nOrders);


            return averageAnnualOrders;
        }

        public List<ProductSales> CompanyTop10Products()
        {
            return nwEntities.Order_Details.
                    Where(od => (od.Product.Discontinued == false)).
                    GroupBy(od => od.Product.ProductName).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(pSale => pSale.Sales).
                    Take(10).
                    ToList();
        }

        public List<CustomerSales> AllEmployeeTop10Customers()
        {

            return nwEntities.Order_Details.
                    Where(od => (od.Product.Discontinued == false)).
                    GroupBy(od => od.Order.Customer.CompanyName).
                    Select(custSales => new CustomerSales()
                    {
                        theCustomer = custSales.Key,
                        Sales = custSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(pSale => pSale.Sales).
                    Take(10).
                    ToList();
        }

        public decimal CompanyTotalSales()
        {
            return nwEntities.Order_Details.Sum(od => od.UnitPrice * od.Quantity);
        }

        public decimal EmployeeTotalSales(int? EmployeeID)
        {
            if (EmployeeID == null || nwEntities.Employees.Find(EmployeeID) == null)
                return 0;

            return nwEntities.Order_Details.
                Where(od => od.Order.EmployeeID == EmployeeID).
                Sum(od => od.UnitPrice * od.Quantity);
        }
        #endregion



        
        #region Annual Orders

        public ActionResult AnnualOrders()
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderProducts> annualOrders = theEmployee.AnnualOrders();

            ViewBag.CompanyAverageAnnualOrders = CompanyAverageAnnualOrders();
            ViewBag.EmployeeAverageAnnualOrders = EmployeeAverageAnnualOrders(theEmployee.EmployeeID);

            return PartialView("_AnnualOrders", annualOrders);
        }

        public JsonResult GetAnnualOrders()
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderProducts> annualOrders = theEmployee.AnnualOrders();

            return Json(new { JSONList = annualOrders }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnnualOrdersModal(string Year = "")
        {
            int year = DateTime.Now.Year;

            if (string.IsNullOrEmpty(Year))
                year = int.Parse(Year);
            else
                year = int.Parse(Year);

            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderProducts> annualOrders = theEmployee.AnnualOrdersInYear(Year);

            ViewBag.Year = year;
            return PartialView("_AnnualOrdersModal", annualOrders);
        }

        public JsonResult GetAnnualOrdersModal(string Year = "")
        {

            int year = DateTime.Now.Year;

            if (string.IsNullOrEmpty(Year))
                year = int.Parse(Year);

            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderProducts> annualOrders = theEmployee.AnnualOrdersInYear(Year);

            return Json(new { JSONList = annualOrders }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult TableAnnualOrders(string Year = "")
        {
            int year = DateTime.Now.Year;

            if (string.IsNullOrEmpty(Year))
                year = int.Parse(Year);
            else
                year = int.Parse(Year);

            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderProducts> annualOrders = theEmployee.AnnualOrdersInYear(Year);

            ViewBag.Year = year;
            return View(annualOrders);
        }

        #endregion

        #region Annual Revenues

        public ActionResult AnnualRevenues()
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderRevenues> annualRevenues = theEmployee.AnnualRevenues();

            return PartialView("_AnnualRevenues", annualRevenues);
        }

        public JsonResult GetAnnualRevenues()
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);
            IEnumerable<OrderRevenues> annualRevenues = theEmployee.AnnualRevenues();

            return Json(new { JSONList = annualRevenues }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EmployeeSalesByYear

        public ActionResult AllEmployeeSales(string Year = "All Years")
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);

            ViewBag.Year = Year;
            ViewBag.Years = NWTradersUtilities.BeginToEndOrderYears();

            return PartialView("_AllEmployeeSales", new List<EmployeeSales>());
        }

        public JsonResult GetAllEmployeeSales(string Year = "All Years")
        {

            IEnumerable<EmployeeSales> allEmployeeSales = new List<EmployeeSales>();

            if (string.IsNullOrEmpty(Year) || Year.Equals("All Years"))
            {
                allEmployeeSales = nwEntities.Orders.
                    GroupBy(order => new { order.Employee.FirstName, order.Employee.LastName }).
                    OrderBy(a => a.Key.LastName).
                    Select(empSales => new EmployeeSales
                    {
                        theEmployee = empSales.Key.FirstName + " " + empSales.Key.LastName,
                        Sales = empSales.Sum(o => o.Order_Details.Sum(
                            od => od.Quantity * od.UnitPrice))
                    });
            }
            else
            {
                DateTime begin = NWTradersUtilities.BeginningOfYear(Year);
                DateTime end = NWTradersUtilities.EndOfYear(Year);

                allEmployeeSales = nwEntities.Orders.
                    Where(order => (order.OrderDate >= begin) && (order.OrderDate <= end)).
                    GroupBy(order => new { order.Employee.FirstName, order.Employee.LastName }).
                    OrderBy(a => a.Key.LastName).
                    Select(empSales => new EmployeeSales
                    {
                        theEmployee = empSales.Key.FirstName + " " + empSales.Key.LastName,
                        Sales = empSales.Sum(o => o.Order_Details.Sum(
                            od => od.Quantity * od.UnitPrice))
                    });
            }

            return Json(new { JSONList = allEmployeeSales }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Top Products
        public ActionResult EmployeeTopProducts(string Year = "", string Category = "")
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);

            ViewBag.Year = Year;
            ViewBag.Years = NWTradersUtilities.BeginToEndOrderYears();

            IEnumerable<ProductSales> topProductSales = new List<ProductSales>();

            if (string.IsNullOrEmpty(Year))
            {
                topProductSales = nwEntities.Order_Details.
                    Where(od => (od.Product.Discontinued == false)).
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => od.Product).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key.ProductName,
                        ProductCategory = prodSales.Key.Category.CategoryName,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    ToList();
            }
            else
            {
                DateTime begin = NWTradersUtilities.BeginningOfYear(Year);
                DateTime end = NWTradersUtilities.EndOfYear(Year);

                topProductSales = nwEntities.Order_Details.
                    Where(od => (od.Order.OrderDate >= begin) && (od.Order.OrderDate <= end)).
                    Where(od => (od.Product.Discontinued == false)).
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => od.Product).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key.ProductName,
                        ProductCategory = prodSales.Key.Category.CategoryName,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    ToList();
            }

            if (!string.IsNullOrEmpty(Category))
            {
                topProductSales = topProductSales.Where(p => p.ProductCategory.Equals(Category));
            }

            topProductSales = topProductSales.OrderByDescending(p => p.Sales).Take(10);


            ViewBag.Category = Category;
            ViewBag.Year = Year;

            ViewBag.AllEmployeeSales = CompanyTop10Products();
            ViewBag.CompanyTotalSales = CompanyTotalSales();
            ViewBag.EmployeeTotalSales = EmployeeTotalSales(theEmployee.EmployeeID);
            return PartialView("_EmployeeTopProducts", topProductSales);
        }

        public JsonResult GetEmployeeTopProductsAllYears()
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);

            //IEnumerable<ProductSales> topProductSales = new List<ProductSales>();

            var topProductSales = nwEntities.Order_Details.
                   Where(od => (od.Product.Discontinued == false)).
                   Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                   GroupBy(od => new { od.Product, od.Order.OrderDate.Value.Year }).
                   Select(prodSales => new
                   {
                       ProductName = prodSales.Key.Product.ProductName,
                       Year = prodSales.Key.Year.ToString(),
                       Sales = prodSales.Sum(c => c.Quantity * c.UnitPrice)
                   }).
                   OrderByDescending(o => o.Year).
                   ThenByDescending(s => s.Sales).
                   ToList();



            List<string> years = topProductSales.
                Select(p => p.Year.ToString()).
                OrderByDescending(y => y).Distinct().ToList();

            var products =
                topProductSales.GroupBy(p => new { p.ProductName }).
                Select(
                    p => new {
                        ProductName = p.Key.ProductName,
                        sale = p.Sum(a => a.Sales)
                    }).
                OrderByDescending(y => y.sale).Take(10).
                Select(p => p.ProductName)
                .ToList();

            var sales = new decimal[products.Count, years.Count];

            int whichProduct = 0;
            int whichYear = 0;


            foreach (string prod in products)
            {
                foreach (string year in years)
                {
                    decimal sale = topProductSales.
                        Where(ps => ps.ProductName.Equals(prod)).
                        Where(ps => ps.Year.Equals(year)).
                        Select(ps => ps.Sales).
                        FirstOrDefault();


                    sales[whichProduct, whichYear] = sale;
                    whichYear++;
                }

                whichYear = 0;
                whichProduct++;

            }

            var AllSales = new
            {
                Years = years,
                Products = products,
                Sales = sales
            };

            return Json(new { JSONList = AllSales }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeTopProductsInYear(string Year = "")
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);

            IEnumerable<ProductSales> topProductSales = new List<ProductSales>();

            if (string.IsNullOrEmpty(Year))
            {
                topProductSales = nwEntities.Order_Details.
                    Where(od => (od.Product.Discontinued == false)).
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => od.Product.ProductName).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(pSale => pSale.Sales).
                    Take(10).
                    ToList();
            }
            else
            {
                DateTime begin = NWTradersUtilities.BeginningOfYear(Year);
                DateTime end = NWTradersUtilities.EndOfYear(Year);

                topProductSales = nwEntities.Order_Details.
                    Where(od => (od.Order.OrderDate >= begin) && (od.Order.OrderDate <= end)).
                    Where(od => (od.Product.Discontinued == false)).
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => od.Product.ProductName).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(pSale => pSale.Sales).
                    Take(10).
                    ToList();
            }

            return Json(new { JSONList = topProductSales }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Top Customers

        public ActionResult EmployeeTopCustomers(string Year = "")
        {
            Employee theEmployee = Session["currentEmployee"] as Employee;
            theEmployee = nwEntities.Employees.Find(theEmployee.EmployeeID);

            ViewBag.Year = Year;
            ViewBag.Years = NWTradersUtilities.BeginToEndOrderYears();

            IEnumerable<CustomerSales> allCustomerSales = new List<CustomerSales>();

            if (string.IsNullOrEmpty(Year) || Year.Equals("All"))
            {
                allCustomerSales = nwEntities.Order_Details.
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => new { od.Order.Customer.CompanyName }).
                    Select(custSales => new CustomerSales()
                    {
                        theCustomer = custSales.Key.CompanyName,
                        Sales = custSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(cSale => cSale.Sales).
                    Take(10).
                    ToList();
            }
            else
            {
                DateTime begin = NWTradersUtilities.BeginningOfYear(Year);
                DateTime end = NWTradersUtilities.EndOfYear(Year);

                allCustomerSales = nwEntities.Order_Details.
                    Where(od => (od.Order.OrderDate >= begin) && (od.Order.OrderDate <= end)).
                    Where(od => (od.Order.EmployeeID == theEmployee.EmployeeID)).
                    GroupBy(od => od.Order.Customer.CompanyName).
                    Select(custSales => new CustomerSales()
                    {
                        theCustomer = custSales.Key,
                        Sales = custSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderByDescending(cSale => cSale.Sales).
                    Take(10).
                    ToList();

            }

            ViewBag.AllEmployeeCustomers = AllEmployeeTop10Customers();
            ViewBag.CompanyTotalSales = CompanyTotalSales();
            ViewBag.EmployeeTotalSales = EmployeeTotalSales(theEmployee.EmployeeID);

            return PartialView("_EmployeeTopCustomers", allCustomerSales);
        }

        public JsonResult GetEmployeeCustomers(string Year = "")
        {

            IEnumerable<ProductSales> allProductSales = new List<ProductSales>();

            if (string.IsNullOrEmpty(Year))
            {
                allProductSales = nwEntities.Order_Details.
                    GroupBy(od => od.Product.ProductName).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderBy(pSale => pSale.Sales).
                    ToList();
            }
            else
            {
                DateTime begin = NWTradersUtilities.BeginningOfYear(Year);
                DateTime end = NWTradersUtilities.EndOfYear(Year);

                allProductSales = nwEntities.Order_Details.
                    Where(od => (od.Order.OrderDate >= begin) && (od.Order.OrderDate <= end)).
                    GroupBy(od => od.Product.ProductName).
                    Select(prodSales => new ProductSales()
                    {
                        ProductName = prodSales.Key,
                        Sales = prodSales.Sum(od => od.Quantity * od.UnitPrice)
                    }).
                    OrderBy(pSale => pSale.Sales).
                    ToList();
            }

            return Json(new { JSONList = allProductSales }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Login Functions

        ///// <summary>
        ///// The Current Customer (synchronized with the Session "currentCustomer") is the customer currently logged in.
        ///// If the Current Customer is null, then no actions are allowed.
        ///// </summary>
        public ActionResult Login(string CompanyName = "")
        {
            //Clear the current customer, if one is already logged in 
            Session["currentEmployee"] = null;
            Session.Clear(); // Clear the session
            Session.Abandon(); // And start a new one.

            ViewBag.LastNames = new SelectList(NWTradersUtilities.AllEmployeeLastNames());
            return View();
        }

        /// <summary>
        /// Login the customer 
        /// </summary>
        /// <param name="EmployeeLastName: We are using the company name of the customer."></param>
        /// <param name="Password: We are using the CustomerID for the Customer."></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string EmployeeLastName, string Password)
        {

            if (string.IsNullOrEmpty(EmployeeLastName))
            {
                ViewBag.LastNameMessage = "Please select your Last Name";
                return View();
            }

            if (string.IsNullOrEmpty(Password))
            {
                ViewBag.EmployeePasswordMessage = "Please Enter a password";
                return View();
            }

            // Here only if both Company Name and Customer ID are valid (neither null nor empty)

            // May need to convert case (toLower or toUpper)
            Employee currentEmployee = nwEntities.Employees. // From Customers.
                Where(e => e.LastName.Equals(EmployeeLastName)). // Where the Last Name matches
                Where(e => e.Password.Equals(Password)). // & the password Matches
                Select(e => e). // Select the Employee
                FirstOrDefault(); // And get the first - there should be only one anyway.

            //Set the current user object for the session to TheCurrentUser (maybe null) 
            Session["currentEmployee"] = currentEmployee;

            if (currentEmployee == null)
            {
                ViewBag.EmployeePasswordMessage = "The Employee Last Name and password you entered is not valid. Please enter a valid LastName and password";
                return View();
            }
            else
                // the user is found, so load the selected customer informaiton.
                return RedirectToAction("Details", "Employees", new { @ID = currentEmployee.EmployeeID });
        }

        #endregion

        #region ScaffoldedFunctions

        // GET: Employees
        public ActionResult Index()
        {
            var employees = nwEntities.Employees.Include(e => e.Employee1);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            Employee currentEmployee = Session["currentEmployee"] as Employee;
            if (currentEmployee == null)
                return RedirectToAction("Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = nwEntities.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }


        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.ReportsTo = new SelectList(nwEntities.Employees, "EmployeeID", "LastName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode,Country,HomePhone,Extension,Photo,Notes,ReportsTo,PhotoPath")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Employees.Add(employee);
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReportsTo = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = nwEntities.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportsTo = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode,Country,HomePhone,Extension,Photo,Notes,ReportsTo,PhotoPath")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Entry(employee).State = EntityState.Modified;
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReportsTo = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", employee.ReportsTo);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = nwEntities.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = nwEntities.Employees.Find(id);
            nwEntities.Employees.Remove(employee);
            nwEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                nwEntities.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}
