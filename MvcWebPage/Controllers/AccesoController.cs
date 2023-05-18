using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using MvcWebPage.Models;
using MvcWebPage.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.MLAVID;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MvcWebPage.Data;

namespace MvcWebPage.Controllers
{
    [Authorize(Roles = "CUENTAS DE USUARIO")]
    public class AccesoController : Controller
    {
        //[AllowAnonymous]
        //public async Task<IActionResult> Index()
        //{

        //    //MLAVIDContext db = new DB();

        //    //int id = db.VENDEDORES.Max(m => m.CODVENDEDOR) + 1;



        //    //LoginService.DefaultCuentas("123");




        //    //var rs=await db.Procedures.IT_PEDIDOSAsync("01",0);
        //    //var j = rs;




        //    //var usuario = db.VENDEDORES.FirstOrDefault(w => w.NOMVENDEDOR.ToLower() == "supervisor");




        //    //var ss=db.VENDEDORES.ToList();

        //    //int CODVENDEDOR = 1;

        //    //var ss2 = db.VENDEDORES.FromSqlInterpolated($"IT_VENDEDORES {CODVENDEDOR}").ToList();


        //    return View();
        //}


        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(Usuario usuario)
        {


            /************************************************************/
            //usuario.User     = "proveedor1";
            //usuario.User     = "sucursal";
            usuario.Password = "123";
            /************************************************************/


            if (string.IsNullOrEmpty(usuario.User))
            {
                return View();
            }

            MLAVIDContext db = new MLAVID_DB(); // MLAVIDContext();

            var it = db.Procedures.IT_LOGINAsync(usuario.User, usuario.Password.GetMD5Hash()).Result.FirstOrDefault();
            


            if (it != null && it.PerfilDescripcion != "")
            {
                HttpContext.Session.SetString("UserName", usuario.User);


                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Name, usuario.User));
                claims.Add(new Claim(ClaimTypes.Role, it.PerfilDescripcion));


                if (it.PerfilDescripcion == "SUCURSAL")
                {
                    if (it.PedLibre)
                    {
                        claims.Add(new Claim("PedLibre", "1"));
                    }

                    if (it.PedModifica)
                    {
                        claims.Add(new Claim("PedModifica", "1"));
                    }

                    if (it.PedElimina)
                    {
                        claims.Add(new Claim("PedElimina", "1"));
                    }
                }

                if (it.PerfilDescripcion == "SUPERVISOR")
                {
                    if (it.PedLibre)
                    {
                        claims.Add(new Claim("PedLibre", "1"));
                    }

                    if (it.PedModifica)
                    {
                        claims.Add(new Claim("PedModifica", "1"));
                    }

                    if (it.PedElimina)
                    {
                        claims.Add(new Claim("PedElimina", "1"));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (it.PerfilDescripcion.ToUpper()== "CUENTAS DE USUARIO")
                {
                    return RedirectToAction("CuentasUsuario", "Acceso");
                }


                if (it.PerfilDescripcion.ToUpper() == "SUCURSAL")
                {
                    return RedirectToAction("GenerarPedidos", "GenerarPedidos");
                }


                if (it.PerfilDescripcion.ToUpper() == "PROVEEDOR")
                {
                    return RedirectToAction("Pedidos", "Pedidos");
                }
            }


            usuario.MsgErr = "Usuario o contraseña no valido.";
            return View(usuario);


            /*
            var rs = LoginService.Login(usuario);

            if (rs != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuario.User),
                };

                foreach (var rol in rs.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));


                if (rs.Roles.Contains("Administrador"))
                {
                    return RedirectToAction("InventariosFisicosSemanales", "InventariosFisicos");
                }


                if (rs.Roles.Contains("Proveedor"))
                {
                    return RedirectToAction("GenerarPedidos", "Proveedores");
                }

                return RedirectToAction("Index", "Home");
            }*/


        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSession()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Acceso");
        }

        
        public IActionResult CuentasUsuario()
        {
            return View();
        }

        public IActionResult GetPerfiles()
        {
            MLAVIDContext db = new MLAVID_DB();
            var rs = db.IT_PERFILES.ToList();
            rs.Insert(0,new IT_PERFILES()
            {
                descripcion = "TODOS",
                tipoUsuario = 0
            });

            return new { code = 0, rs }.RSon();

        }

        public IActionResult GetUsuarios([FromBody] Request req)
        {
            MLAVIDContext db = new MLAVID_DB();


            List<IT_USUARIOSResult> rs = null;

            if (req.tipoUsuario == 0)
            {
                rs=db.Procedures.IT_USUARIOSAsync().Result
                    .Where(w=>w.NOMVENDEDOR.ToLower().Contains(req.usuario.ToLower()))
                    .ToList();
            }
            else
            {
                rs=db.Procedures.IT_USUARIOSAsync().Result
                    .Where(w=>w.TIPOUSUARIO==req.tipoUsuario &&
                              w.NOMVENDEDOR.ToLower().Contains(req.usuario.ToLower())).ToList();
                
            }


            return new { code = 0, rs }.RSon();
        }
    }
}
