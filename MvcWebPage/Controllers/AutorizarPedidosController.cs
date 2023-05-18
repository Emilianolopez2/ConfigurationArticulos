using Microsoft.AspNetCore.Mvc;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;

namespace MvcWebPage.Controllers
{
    public class AutorizarPedidosController : Controller
    {
        public IActionResult AutorizarPedidos()
        {

            ViewData["usuario"]    = Extensions.GetUserName();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");

            return View("AutorizarPedidos");
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
        public IActionResult AutorizarPedido([FromBody] Request req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var it = db.PEDCOMPRACAB.FirstOrDefault(f =>
                    f.NUMSERIE == req.pedido_aut.NUMSERIE && f.NUMPEDIDO == req.pedido_aut.NUMPEDIDO);

                MPedidosService.ProcesarPedidos(req, User);

                it.IDESTADO = 2;

                db.SaveChanges();

                return new { code = 0 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
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

        [HttpPost]
        public IActionResult GetPedidosAutorizados2([FromBody] Request req)
        {
            try
            {
                var rs = PedidosAutServices.GetPedidosAutorizados2(req);


                foreach (var it in rs)
                {
                    it.DESCRIPCION = it.DESCRIPCION + " - " + it.ID_ORDEN;
                }

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
        public IActionResult RechazarPedido([FromBody] Request req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var it = db.PEDCOMPRACAB.FirstOrDefault(f =>
                    f.NUMSERIE == req.pedido_aut.NUMSERIE && f.NUMPEDIDO == req.pedido_aut.NUMPEDIDO);

                it.IDESTADO = 1;

                db.SaveChanges();

                return new { code = 0 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
            }
        }
    }
}
