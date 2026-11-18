using _25DH110096_MyStoreBE.Models;
using _25DH110096_MyStoreBE.Models.ViewModels;

using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _25DH110096_MyStoreBE.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private TH_BE_2026Entities db = new TH_BE_2026Entities();

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new SearchProductVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            { // Thanh tim kiem
                products = products.Where(p =>
                p.ProductName.Contains(searchTerm) ||
                p.ProductDecription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm)
                );
            }
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }
            switch (sortOrder)
            {
                case "name_asc": products = products.OrderBy(p => p.ProductName); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = sortOrder;

            int pageNumber = page ?? 1;
            int pageSize = 2;
            //model.Products = products.ToList();
            model.Products = products.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductID == id.Value);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")]Product product)
        {
            if (String.IsNullOrWhiteSpace(product.ProductImage))
            {
                product.ProductImage = "~/Content/images/default_img.png";
            }
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                product.CategoryID
            );
            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")]Product input)
        {
            Product product = db.Products.Find(input.ProductID);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Nếu không nhập link ảnh mới thì giữ ảnh cũ
            if (String.IsNullOrWhiteSpace(input.ProductImage))
            {
                input.ProductImage = product.ProductImage;
            }

            if (ModelState.IsValid)
            {
                product.CategoryID = input.CategoryID;
                product.ProductName = input.ProductName;
                product.ProductDecription = input.ProductDecription;
                product.ProductPrice = input.ProductPrice;
                product.ProductImage = input.ProductImage;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                input.CategoryID
            );

            return View(input);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
