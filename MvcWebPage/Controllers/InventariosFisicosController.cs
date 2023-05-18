using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MvcWebPage.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class InventariosFisicosController : Controller
    {


        public IActionResult InventariosFisicosMensuales()
        {
            return View();
        }

        public IActionResult InventariosFisicosSemanales()
        {
            return View();
        }
    }
}
