using Microsoft.AspNetCore.Mvc;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;

namespace MvcWebPage.Controllers
{
    public class PedidosAutorizadosController : Controller
    {
        public IActionResult PedidosAutorizados()
        {

            ViewData["usuario"]    = Extensions.GetUserName();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");

            return View("PedidosAutorizados");
        }

        [HttpPost]
        public IActionResult GetPedidosAutorizadosDet([FromBody] Request req)
        {
            try
            {
                var (rs, monedaDesc, monedaCot, rs2, rs3) = PedidosAutServices.GetPedidosAutorizadosDet(req);


                return new { code = 0, rs, rs2, rs3, monedaDesc, monedaCot }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }


        [HttpPost]
        public IActionResult GetPedidosAutorizados([FromBody] Request req)
        {
            try
            {
                var rs = PedidosAutServices.GetPedidosAutorizados(req);


                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }

        [HttpPost]
        public IActionResult GetSucursales([FromBody] Request req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var rs = db.ALMACEN.ToList();

                if (req.todos)
                {
                    rs.Insert(0, new ALMACEN
                    {
                        CODALMACEN    = "",
                        NOMBREALMACEN = "TODOS"
                    });
                }

                rs.RemoveAll(a => a.CODALMACEN == "00");

                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }


        [HttpPost]
        public IActionResult GetProveedores()
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var rs = db.PROVEEDORES.Select(s => s.NOMPROVEEDOR).ToList();


                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }
    }
}
