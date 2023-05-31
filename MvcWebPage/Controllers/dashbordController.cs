using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebPage.Controllers
{
    public class dashbordController : Controller
    {
        // GET: dashbordController
        public ActionResult Index()
        {
            return View();
        }

        // GET: dashbordController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: dashbordController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: dashbordController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: dashbordController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: dashbordController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: dashbordController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: dashbordController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
