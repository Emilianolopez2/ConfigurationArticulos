using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;

namespace MvcWebPage.Controllers
{
    public class GenerarPedidosController : Controller
    {
        public IActionResult GenerarPedidos()
        {
            ViewData["usuario"]    = Extensions.GetUserName();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");


            //MLAVIDContext db   = new DB();
            //var usuario = User.GetUserName();

            //var it=db.Procedures.IT_USUARIOSAsync().Result.FirstOrDefault(f=>f.NOMVENDEDOR==usuario);




            return View("GenerarPedidos");
        }

        [HttpPost]
        public IActionResult GetDptoData([FromBody] Request req)
        {
            try
            {
                //var rs = DepartamentoService.GetDptoData(req);

                return new { code = 0, rs="[]" }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }

        }

        public IActionResult AnularPedido([FromBody] Request req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var it = db.IT_PEDIDOS_CAB.Where(f => f.ID == req.id);
                it.ExecuteDelete();



                return new { code = 0 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
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
        public IActionResult GetDepartamentos([FromBody] Request req)
        {
            try
            {
                //var rs = DepartamentoService.GetDepartamentos(req);

                return new { code = 0, rs ="[]"}.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }

        }

        [HttpPost]
        public IActionResult GetPedidos([FromBody] Request req)
        {
            try
            {
                var rs = PedidosService.GetPedidos(req);


                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }


        [HttpPost]
        public IActionResult SavePedidos([FromBody] Request req)
        {

            if (req == null)
            {
                return new { code = -1 }.RSon();
            }


            try
            {
                PedidosService.SavePedidos(req, HttpContext);

                return new { code = 0 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }

        }

        public IActionResult ProcesarPedidos([FromBody] Request req)
        {
            try
            {
                APedidosService.ProcesarPedidos(req, HttpContext);

                return new { code = 0 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
            }
        }
    }
}
