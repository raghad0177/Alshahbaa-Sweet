using alshahbaasweets.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dern_Support.Controllers.MVC_Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Accounts()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Inventory()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Scheduling()
        {
            return View();
        }

        public IActionResult AnalyticsManagement()
        {
            return View();
        }

        // Added Methods for Admin Views
        public IActionResult AddAdmin()
        {
            return View();
        }

        public IActionResult addCategory()
        {
            return View();
        }

        public IActionResult AddMenuProducts()
        {
            return View();
        }

        public IActionResult Addproduct()
        {
            return View();
        }

        public IActionResult adminproducts()
        {
            return View();
        }

        public IActionResult categories()
        {
            return View();
        }

        public IActionResult CouponManagement()
        {
            return View();
        }

        public IActionResult editcategory()
        {
            return View();
        }

        public IActionResult editmenuproducts()
        {
            return View();
        }

        public IActionResult editProduct()
        {
            return View();
        }

        public IActionResult MenuProducts()
        {
            return View();
        }

        public IActionResult order()
        {
            return View();
        }

        public IActionResult ordersingle()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}
