using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using MvcWebPage.MLAVID;
using MvcWebPage.Data;
using NuGet.Protocol;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MvcWebPage.Services;


namespace MvcWebPage.Controllers
{
    [Authorize(Roles = "SUCURSAL")]
    public class ProveedoresController : Controller
    {






        [Authorize(Policy = "PedLibre")]
        public IActionResult Pagina1()
        {
            return View();
        }

        [Authorize(Policy = "PedModifica")]
        public IActionResult Pagina2()
        {
            return View();
        }

        [Authorize(Policy = "PedElimina")]
        public IActionResult Pagina3()
        {
            return View();
        }


    }
}
