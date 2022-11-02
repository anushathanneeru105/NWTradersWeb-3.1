using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using NWTradersWeb.Models;

namespace NWTradersWeb.Controllers
{
    public class CustomersController : Controller
    {
        private NorthwindEntities nwEntities = new NorthwindEntities();

        // GET: Customers
        public ActionResult Index(int? page,
            string itemsPerPage = "15",
            string sortOrder = "",
            string searchCompanyName = "",
            string searchContactName = "",
            string searchCountryName = "",
            string searchRegion = "",
            string searchTitle = "")
        {

            IEnumerable<Customer> theCustomers = nwEntities.Customers.
                OrderBy(c => c.CompanyName).
                Select(c => c).ToList();


            if (theCustomers.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchCompanyName) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    theCustomers = theCustomers.
                        Where(c => c.CompanyName.StartsWith(
                            searchCompanyName, ignoreCase: true, new System.Globalization.CultureInfo("en-US"))).
                        OrderBy(c => c.CompanyName).
                        Select(c => c);
                }
            ViewBag.searchCompanyName = searchCompanyName;


            if (theCustomers.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchContactName) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    theCustomers = theCustomers.
                        Where(c => c.ContactName.StartsWith(
                            searchContactName, ignoreCase: true, new System.Globalization.CultureInfo("en-US"))).
                        OrderBy(c => c.ContactName).
                        Select(c => c);
                }
            ViewBag.searchContactName = searchContactName;

            if (theCustomers.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchRegion) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    theCustomers = theCustomers.
                        Where(c => string.IsNullOrEmpty(c.Region) == false).
                        Where(c => c.Region.StartsWith(
                            searchRegion,
                            ignoreCase: true, new System.Globalization.CultureInfo("en-US"))).
                        OrderBy(c => c.Region).
                        Select(c => c);
                }
            ViewBag.searchContactName = searchContactName;

            if (theCustomers.Count() > 0)
                if (string.IsNullOrEmpty(searchCountryName) == false)
                {
                    theCustomers = theCustomers.
                        Where(c => c.Country.Equals(searchCountryName)).
                        OrderBy(c => c.CompanyName).
                        Select(c => c);
                }
            ViewBag.searchCountryName = searchCountryName;

            if (theCustomers.Count() > 0)
                if (string.IsNullOrEmpty(searchTitle) == false)
                {
                    theCustomers = theCustomers.
                        Where(c => c.ContactTitle.Contains(searchTitle)).
                        OrderBy(c => c.CompanyName).
                        Select(c => c);
                }
            ViewBag.searchTitle = searchTitle;


            switch (sortOrder)
            {
                case "CompName_desc":
                    theCustomers = theCustomers.
                        Where(c => c.CompanyName.StartsWith(searchCompanyName)).
                        OrderBy(c => c.CompanyName);
                    break;

                default: //All - do nothing
                    break;

            }

            // page size is supplied 
            int pageSize;
            if (itemsPerPage == "All")
                pageSize = theCustomers.Count();
            else
                pageSize = (int.Parse(itemsPerPage));

            int pageNumber = (page ?? 1);

            ViewBag.itemsPerPage = itemsPerPage;
            ViewBag.pageNumber = pageNumber;

            return View(theCustomers.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null || string.IsNullOrEmpty(id))
                return RedirectToAction("Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = nwEntities.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public ActionResult Analysis(string id)
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null || string.IsNullOrEmpty(id))
                return RedirectToAction("Login");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = nwEntities.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);

        }

        public ActionResult AnnualSales(string CustomerID)
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
           if (currentCustomer == null)
                return RedirectToAction("Login");

           Customer theCustomer = nwEntities.Customers.Include(c => c.Orders).Where(c => c.CustomerID.Equals(CustomerID)).FirstOrDefault();

            return PartialView("_AnnualSales", theCustomer.CustomerAnnualSales());
        }

        public ActionResult TopProducts(string CustomerID)
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login");

            Customer theCustomer = nwEntities.Customers.Include(c => c.Orders).Where(c => c.CustomerID.Equals(CustomerID)).FirstOrDefault();

            return PartialView("_TopProducts", theCustomer.CustomerTopProducts());
        }

        public ActionResult TopProductCategories(string CustomerID)
        {
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login");

            Customer theCustomer = nwEntities.Customers.Include(c => c.Orders).Where(c => c.CustomerID.Equals(CustomerID)).FirstOrDefault();

            return PartialView("_TopProductCategories", theCustomer.CustomerTopProducts());
        }

        public JsonResult GetAnnualSales(string CustomerID)
        {
            Customer theCustomer = nwEntities.Customers.Include(c => c.Orders).Where(c => c.CustomerID.Equals(CustomerID)).FirstOrDefault();

            IEnumerable<CustomerSales>  annualSales = theCustomer.CustomerAnnualSales();
            return Json(new {JsonList =  annualSales}, JsonRequestBehavior.AllowGet );
        }
        public ActionResult AverageSales()
        {
            List<OrderRevenues> AnnualAverageSales = new List<OrderRevenues>();

            var AvgOrder = nwEntities.
                Orders.
                Where(o => o.OrderDate.Value.Year < 2021).
                GroupBy(custOrder => new { custOrder.Customer, custOrder.OrderDate.Value.Year }).
                Select(avgOrder => new {
                    C = avgOrder.Key.Customer.CompanyName,
                    Year = avgOrder.Key.Year,
                    avg = avgOrder.Average(o => o.Order_Details.Sum(od => od.Quantity * od.UnitPrice)),
                    annualSpend = avgOrder.Sum(o => o.Order_Details.Sum(od => od.Quantity * od.UnitPrice)),
                    annualNumberOfOrders = avgOrder.Count(),
                    Orders = avgOrder.ToList()
                }).
                ToList();

            var avg = nwEntities.Orders.Where(o => o.OrderDate.Value.Year < 2021).
                Average(o => o.Order_Details.Sum(od => od.Quantity * od.UnitPrice));

            var avgAnnualOrder = nwEntities.Orders.
                                Where(o => o.OrderDate.Value.Year < 2021).
                                GroupBy(o => o.OrderDate.Value.Year).
                                Select(avgOrder => new {
                                    year = avgOrder.Key,
                                    avg = avgOrder.Average(o => o.Order_Details.Sum(od => od.Quantity * od.UnitPrice))
                                }).ToList();


            decimal AverageAnnualCustOrder = AvgOrder.Average(o => o.avg);
            ViewBag.AnnualOrder = AverageAnnualCustOrder;

            decimal AverageAnnualSpend = AvgOrder.Average(o => o.annualSpend);
            ViewBag.AnnualOrder = AverageAnnualCustOrder;

            decimal MaxSpend = AvgOrder.Max(o => o.annualSpend);
            decimal MinSpend = AvgOrder.Min(o => o.annualSpend);


            var AnnualSpend = AvgOrder.
                Select(o => o);

            return View();
        }














        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Customers.Add(customer);
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = nwEntities.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Entry(customer).State = EntityState.Modified;
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = nwEntities.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Customer customer = nwEntities.Customers.Find(id);
            nwEntities.Customers.Remove(customer);
            nwEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Logout(string CompanyName = "")
        {
            //Clear the current customer, if one is already logged in 
            Session["currentCustomer"] = null;
            Session.Clear(); // Clear the session
            Session.Abandon(); // And start a new one.

            ViewBag.LogoutMessage = "You have been successfully logged out.";
            return RedirectToAction("Login");
        }

            ///// <summary>
            ///// The Current Customer (synchronized with the Session "currentCustomer") is the customer currently logged in.
            ///// If the Current Customer is null, then no actions are allowed.
            ///// </summary>
            public ActionResult Login(string CompanyName = "")
        {
            //Clear the current customer, if one is already logged in 
            Session["currentCustomer"] = null;
            Session.Clear(); // Clear the session
            Session.Abandon(); // And start a new one.

            ViewBag.CompanyName = CompanyName;

            ViewBag.Message = "Welcome to NW Traders . Please Login with your Company Name and Customer ID .";
            return View();
        }

        /// <summary>
        /// Login the customer 
        /// </summary>
        /// <param name="CompanyName: We are using the company name of the customer."></param>
        /// <param name="CustomerID: We are using the CustomerID for the Customer."></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string CompanyName, string CustomerID)
        {

            if (string.IsNullOrEmpty(CompanyName))
            {
                ViewBag.CompanyNameMessage = "Please select your company";
                return View();
            }

            if (string.IsNullOrEmpty(CustomerID))
            {
                ViewBag.CustomerIDMessage = "Please Enter a Valid Customer ID as your password";
                return View();
            }

            // Here only if both Company Name and Customer ID are valid (neither null nor empty)

            // May need to convert case (toLower or toUpper)
            Customer currentCustomer = nwEntities.Customers. // From Customers.
                Where(c => c.CompanyName.Equals(CompanyName)). // Where the company name 
                Where(c => c.CustomerID.Equals(CustomerID)). // & Customer ID match,
                Select(c => c). // Select the customer
                FirstOrDefault(); // And get the first - there should be only one anyway.

            //Set the current user object for the session to TheCurrentUser (maybe null) 
            Session["currentCustomer"] = currentCustomer;

            if (currentCustomer == null)
            {
                ViewBag.CustomerIDMessage = "The customer ID you entered is not valid. Please enter a valid Customer ID as your password";
                return View();
            }
            else
                // the user is found, so load the selected customer informaiton.
                return RedirectToAction("Details", "Customers", new { @id = currentCustomer.CustomerID });
        }

        public ActionResult RefreshShoppingCart()
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                return PartialView("_ShoppingCart", currentCustomer);

            // We know that we have the current customer ... otherwise, we would have been re-directed.
            if (currentCustomer.myShoppingCart == null)
            {
                currentCustomer.CreateNewOrder();
            }

            return PartialView("_ShoppingCart", currentCustomer);
        }

        public ActionResult AddToCart(int? ProductID, short Quantity=1, 
            string searchCategoryName = "",string searchProductName = "")
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            // We know that we have the current customer ... otherwise, we would have been re-directed.
            if (currentCustomer.myShoppingCart == null)
            {currentCustomer.CreateNewOrder();}

            if (ProductID.HasValue)
            {
                Product productToAdd = nwEntities.Products.Find(ProductID.Value);
                currentCustomer.myShoppingCart.AddToCart(productToAdd, Quantity);
            }
            return RedirectToAction("Index", "Products", 
                new
                {searchCategoryName = searchCategoryName, searchProductName = searchProductName });
        }

        public ActionResult PlaceOrder()
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            if (currentCustomer.myShoppingCart == null)
                return RedirectToAction("Index", "Products");

            return RedirectToAction("Create", "Orders");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                nwEntities.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
