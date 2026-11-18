using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25DH110096_MyStoreBE.Models;
using System.Data.Entity.Infrastructure;

namespace _25DH110096_MyStoreBE.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private TH_BE_2026Entities db = new TH_BE_2026Entities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            var categories = db.Categories
                .Include(c => c.Products)
                .ToList();

            return View(categories);
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // loi ko co gia tri truyen vao
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();    //404
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
            //return View(Details(id));
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories
                .Include(c => c.Products)
                .SingleOrDefault(c => c.CategoryID == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            // Đếm số sản phẩm thuộc danh mục
            ViewBag.ProductCount = category.Products.Count;

            // Lấy thông báo lỗi nếu có
            ViewBag.DeleteError = TempData["DeleteError"];

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories
                .Include(c => c.Products)
                .SingleOrDefault(c => c.CategoryID == id);

            if (category == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra sản phẩm liên quan trước khi xóa
            if (category.Products.Any())
            {
                TempData["DeleteError"] =
                    "Không thể xóa danh mục '" + category.CategoryName +
                    "' vì hiện đang có " + category.Products.Count +
                    " sản phẩm thuộc danh mục này. " +
                    "Vui lòng chuyển sản phẩm sang danh mục khác trước khi xóa.";

                return RedirectToAction("Delete", new { id = id });
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                // Phòng trường hợp phát sinh ràng buộc dữ liệu
                // trong lúc thực hiện thao tác xóa
                TempData["DeleteError"] =
                    "Không thể xóa danh mục vì vẫn còn dữ liệu liên quan.";

                return RedirectToAction("Delete", new { id = id });
            }
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
