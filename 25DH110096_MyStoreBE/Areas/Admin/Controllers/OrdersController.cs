using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using _25DH110096_MyStoreBE.Models;

namespace _25DH110096_MyStoreBE.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private TH_BE_2026Entities db = new TH_BE_2026Entities();

        // GET: Admin/Orders
        // Danh sách đơn hàng, tìm kiếm và lọc
        public ActionResult Index(
            string customerName,
            string paymentStatus,
            DateTime? orderDate,
            int? page)
        {
            var orders = db.Orders
                .Include(o => o.Customer)
                .AsNoTracking()
                .AsQueryable();

            // Tìm kiếm theo tên khách hàng
            if (!String.IsNullOrWhiteSpace(customerName))
            {
                customerName = customerName.Trim();

                orders = orders.Where(o =>
                    o.Customer != null &&
                    o.Customer.CustomerName.Contains(customerName));
            }

            // Lọc trạng thái thanh toán
            if (!String.IsNullOrWhiteSpace(paymentStatus))
            {
                orders = orders.Where(o =>
                    o.PaymentStatus == paymentStatus);
            }

            // Lọc theo ngày đặt hàng
            if (orderDate.HasValue)
            {
                DateTime startDate = orderDate.Value.Date;
                DateTime endDate = startDate.AddDays(1);

                orders = orders.Where(o =>
                    o.OrderDate >= startDate &&
                    o.OrderDate < endDate);
            }

            // Lấy danh sách trạng thái đang có trong Database
            var paymentStatuses = db.Orders
                .Select(o => o.PaymentStatus)
                .Distinct()
                .ToList()
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .OrderBy(s => s)
                .ToList();

            ViewBag.PaymentStatuses =
                new SelectList(paymentStatuses, paymentStatus);

            ViewBag.CustomerName = customerName;
            ViewBag.SelectedPaymentStatus = paymentStatus;

            ViewBag.OrderDate = orderDate.HasValue
                ? orderDate.Value.ToString("yyyy-MM-dd")
                : "";

            // Sắp xếp đơn hàng mới nhất lên trước
            orders = orders
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderID);

            // Phân trang
            int pageNumber = page ?? 1;

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            int pageSize = 5;

            return View(orders.ToPagedList(pageNumber, pageSize));
        }


        // GET: Admin/Orders/Details/5
        // Xem hóa đơn chi tiết
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            Order order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .AsNoTracking()
                .SingleOrDefault(o => o.OrderID == id.Value);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
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