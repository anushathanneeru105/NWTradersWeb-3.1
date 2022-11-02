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
    public class ProductsController : Controller
    {
        private NorthwindEntities nwEntities = new NorthwindEntities();

        // GET: Products
        public ActionResult Index(
            string searchCategoryName = "",
            string searchProductName = "",
            bool showDiscontinued = false
            )
        {
            // Gets all the products in the products table along with the products' categories and supplier objects.
            var products =
                nwEntities.Products.
                Include(p => p.Category).
                Include(p => p.Supplier);

            // If we are not showing the discontinued items, remove them from the list ...
            // otherwise, do nothing
            if (showDiscontinued == false)
                products = products.Where(p => p.Discontinued == false);
            ViewBag.showDiscontinued = showDiscontinued;

            if (products.Count() > 0) // if we did retrieve one or more products (>0) from the db.
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchCategoryName) == false) // the user supplied a Category name.
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    // Select those products where - product category name equals the supplied category name.
                    products = products.
                        Where(p => p.Category.CategoryName.
                        Equals(searchCategoryName)).
                        OrderBy(p => p.ProductName).
                        Select(p => p);
                }
            // remember the category name supplied by putting it the view bag.
            ViewBag.searchCategoryName = searchCategoryName;


            if (products.Count() > 0)
                // Here the ignore case allows for searches that are not case sensitive.
                // Use this to do case insensitive searches for any field.
                if (string.IsNullOrEmpty(searchProductName) == false)
                {
                    // Here the ignore case allows for searches that are not case sensitive.
                    // Use this to do case insensitive searches for any field.
                    products = products.
                        Where(p =>
                            p.ProductName.ToUpper().StartsWith(
                                searchProductName.ToUpper())).
                            OrderBy(p => p.ProductName).
                            Select(p => p);
                }
            ViewBag.searchCompanyName = searchProductName;


            // return the list of products - filtered by the search criteria 
            return View(products.ToList());
        }


        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = nwEntities.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(nwEntities.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(nwEntities.Suppliers, "SupplierID", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Products.Add(product);
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(nwEntities.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(nwEntities.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = nwEntities.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(nwEntities.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(nwEntities.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                nwEntities.Entry(product).State = EntityState.Modified;
                nwEntities.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(nwEntities.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(nwEntities.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = nwEntities.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = nwEntities.Products.Find(id);
            nwEntities.Products.Remove(product);
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
    }
}
