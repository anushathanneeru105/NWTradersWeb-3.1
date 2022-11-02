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
    public class OrdersController : Controller
    {
        private NorthwindEntities nwEntities = new NorthwindEntities();

        // GET: Orders
        public ActionResult Index()
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            // Will only execute this line if there is a "current Customer"
            var orders = nwEntities.Orders.
                Include(o => o.Customer).
                Include(o => o.Employee).
                Include(o => o.Shipper).
                Where( o=> o.CustomerID.Equals(currentCustomer.CustomerID)).
                OrderByDescending(o => o.OrderDate);
            
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = nwEntities.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            Order theCurrentOrder = currentCustomer.myShoppingCart;
            if (theCurrentOrder == null)
                return RedirectToAction("Index", "Products");

            ViewBag.CustomerID = new SelectList
                (nwEntities.Customers, "CustomerID", "CompanyName");

            ViewBag.EmployeeID = new SelectList
                (nwEntities.Employees, "EmployeeID", "LastName");
            
            ViewBag.ShipVia = new SelectList
                (nwEntities.Shippers, "ShipperID", "CompanyName");

            ViewBag.OrderDate = System.DateTime.Today;
            
            return View(currentCustomer.myShoppingCart);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            //Read the current customer object from the session into the currentCustomer (maybe null) 
            Customer currentCustomer = Session["currentCustomer"] as Customer;

            currentCustomer.myShoppingCart.OrderDate = System.DateTime.Today;
            currentCustomer.myShoppingCart.RequiredDate = System.DateTime.Today.AddDays(7);
            currentCustomer.myShoppingCart.CustomerID = currentCustomer.CustomerID;

            Customer cust = 
                nwEntities.Customers.
                Where(x => x.CustomerID.Equals(currentCustomer.CustomerID)).
                Include(c => c.Orders).
                FirstOrDefault();



            if (ModelState.IsValid)
            {
                nwEntities.Orders.Add(currentCustomer.myShoppingCart);
                nwEntities.SaveChanges();

                // Now I can clear the current shopping cart.
                currentCustomer.myShoppingCart = null;

                return RedirectToAction("Details", "Customers", new { @CustomerID = currentCustomer.CustomerID});
            }

            ViewBag.CustomerID = new SelectList(nwEntities.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(nwEntities.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = nwEntities.Orders.Find(id);
            if (order == null)
            {
                if (order==null)
                    return HttpNotFound();
            }

            ViewBag.CustomerID = new SelectList(nwEntities.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(nwEntities.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Entry(order).State = EntityState.Modified;
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(nwEntities.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(nwEntities.Shippers, "ShipperID", "CompanyName", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = nwEntities.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = nwEntities.Orders.Find(id);
            nwEntities.Orders.Remove(order);
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

        public ActionResult Confirm()
        {
            // I need to make sure the customer is still in the session - cant add a product to an order without a customer.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            //The customer should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.myShoppingCart == null)
                return RedirectToAction("Index", "Products");

            // There is an order and customer, bu there are no products in the order.
            if (currentCustomer.myShoppingCart.Order_Details.Count <= 0)
                return RedirectToAction("Index", "Products");

            ViewBag.CustomerID = currentCustomer.CustomerID;
            ViewBag.EmployeeID = new SelectList(nwEntities.Employees, "EmployeeID", "LastName");
            ViewBag.ShipVia = new SelectList(nwEntities.Shippers, "ShipperID", "CompanyName");

            ViewBag.OrderDate = currentCustomer.myShoppingCart.OrderDate;
            ViewBag.RequiredDate= currentCustomer.myShoppingCart.RequiredDate;
            ViewBag.ShippedDate= currentCustomer.myShoppingCart.ShippedDate;

            ViewBag.Freight = currentCustomer.myShoppingCart.Freight;
            ViewBag.ShipTo = currentCustomer.ContactName;



            // Otherwise, the customer and order exist and are ready to confirm
            return View(currentCustomer.myShoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")]
        Order order)
        {
            // I need to make sure the customer is still in the session - cant add a product to an order without a customer.
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            //The customer should have a current order, if not then this is the first product in the cart.
            if (currentCustomer.myShoppingCart == null)
                return RedirectToAction("Index", "Products");

            // There is an order and customer, bu there are no products in the order.
            if (currentCustomer.myShoppingCart.Order_Details.Count <= 0)
                return RedirectToAction("Index", "Products");

            if (ModelState.IsValid)
            {
                /// The object that comes back from the page is called order.

                // We associate the CustomerID to the order.
                order.CustomerID = currentCustomer.CustomerID;

                // And add each of the order details - Dont add  (double add) the product.
                foreach (Order_Detail od  in currentCustomer.myShoppingCart.Order_Details)
                {
                    od.Product = null;
                    order.Order_Details.Add(od);
                }

                // Then ... just add order to the Orders table in the database
                nwEntities.Orders.Add(order);
                // And save changes - EF does the rest .
                nwEntities.SaveChanges();

                // Now the order has an Order ID:
                int OrderID = order.OrderID;

                // Since the order has been placed, theCurrent Order is cleared out.
                currentCustomer.myShoppingCart = null;

                // Show the customer details about the last order placed.
                return RedirectToAction("Details", new { @id = OrderID });
            }

            // If the new order is placed (saved) successfully, the following code will not be reached since we have returned a re-direct.
            ViewBag.CustomerID = new SelectList(nwEntities.Customers, "CustomerID", "CompanyName", order.CustomerID);
            ViewBag.EmployeeID = new SelectList(nwEntities.Employees, "EmployeeID", "LastName", order.EmployeeID);
            ViewBag.ShipVia = new SelectList(nwEntities.Shippers, "ShipperID", "CompanyName", order.ShipVia);

            return View(order);
        }

        public ActionResult Cancel()
        {
            // Cancelling an order implies clearing the current order's information 
            Customer currentCustomer = Session["currentCustomer"] as Customer;
            if (currentCustomer == null)
                return RedirectToAction("Login", "Customers");

            // Clear the current order by setting it to null
            currentCustomer.myShoppingCart = null;

            // Take the user back to the Index page for products.
            return RedirectToAction("Index", "Products", new { });

        }

    }
}
