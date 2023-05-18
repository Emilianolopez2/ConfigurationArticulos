using Microsoft.AspNetCore.Mvc;
using MvcWebPage.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using MvcWebPage.Data;
using Newtonsoft.Json;
using NuGet.Protocol;
using MvcWebPage.Data;

namespace MvcWebPage.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Ventas()
        {
            return View();
        }

        [Authorize(Roles = "Supervisor")]
        public IActionResult Compras()
        {
            return View();
        }

        [Authorize(Roles ="Empleado,Administrador")]
        public IActionResult Clientes()
        {
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //[Authorize(Roles = "Administrador")]
        //[HttpPost]
        //public JsonResult GetCuentas()
        //{
        //    return new JsonResult(new { code = 0 , rs= BD.ListaUsuarios().ToJson() });
        //}
    }
}