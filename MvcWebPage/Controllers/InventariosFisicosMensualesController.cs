using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Data;
using MvcWebPage.Data;

namespace MvcWebPage.Controllers
{

    [Authorize(Roles = "SUCURSAL")]

    public class InventariosFisicosMensualesController : Controller
    {




        public IActionResult InventariosFisicosMensuales()
        {
            //var exs = _context.MOVIMENTS.Where(w => w.CODALMACENDESTINO == "01").ToList().OrderBy(w => w.ID);
            ViewData["usuario"]    = Extensions.GetUserName();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");


            MLAVIDContext db = new MLAVID_DB();

            //var es = db.MOVIMENTS.Where(w => w.CODALMACENDESTINO == User.GetCodAlmacen()).ToList()/*.OrderBy(w => w.ID)*/;

            //var ls = db.Procedures.IT_SELECCIONAR_ARTICULOSAsync(User.GetCodAlmacen()).Result.ToList();

            //List<SELECCIONAR_ARTICULOSResult> lst = (List<SELECCIONAR_ARTICULOSResult>)ViewData["ls"];

            //List<IT_PERFILES> lst = null;

            //ViewData["lista"] = ls;
            //ViewData["MOVS"] = es;


            return View("InventariosFisicosMensuales");
        }



        [HttpPost]
        public IActionResult GetDptoData([FromBody] Request2 req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();


                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;

                if (!string.IsNullOrEmpty(req.fecha))
                {
                    year = DateTime.Parse(req.fecha).Year;
                    month = DateTime.Parse(req.fecha).Month;
                    day = DateTime.Parse(req.fecha).Day;
                }


                var rs = db.Procedures.IT_PEDIDOSAsync(
                    req.sucursal,
                    req.numdpto,
                    year,
                    month,
                    day).Result.ToList();



                //throw new Exception("err 11");
                /*
                foreach (var it in rs)
                {
                    var it2=db.IT_PEDIDOS_TMP.FirstOrDefault(w => w.CODALMACEN == req.sucursal && 
                                                 w.NUMDPTO == req.numdpto &&
                                                 w.REFPROVEEDOR == it.REFPROVEEDOR);
                    if (it2 != null)
                    {
                        it.CANTIDAD = it2.CANTIDAD;
                    }
                }*/

                if (req.excluir)
                {
                    rs.RemoveAll(r => r.CANTIDAD == null || r.CANTIDAD == 0);
                }

                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }



        }


        [HttpPost]
        public IActionResult GetSeleccionarArticulos([FromBody] Request2 req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();


                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;

                //if (!string.IsNullOrEmpty(req.fecha))

                if (req.GridCab != null && req.GridCab.FECHA != null)
                {
                    var fecha = req.GridCab.FECHA.Value;

                    year  = fecha.Year;
                    month = fecha.Month;
                    day   = fecha.Day;

                    req.sucursal = req.GridCab.CODALMACEN;
                    req.numdpto = (short)req.GridCab.NUMDPTO.Value;

                }

                var rs = db.Procedures.IT_SELECT_MODAsync(
                    req.sucursal,
                    req.numdpto,
                    year,
                    month,
                    day).Result.ToList();


                if (req.excluir)
                {
                    rs.RemoveAll(r => r.CANTIDAD == null || r.CANTIDAD == 0);
                }
                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }





            [HttpPost]
        public IActionResult EnviarDatos([FromBody] Request2 req)
        {
            

            //if(req == null)
            //{
            //    return new { code = -1 }.RSon();
            //}


                              
            try
            {
                MLAVIDContext db = new MLAVID_DB();


                var fecha = DateTime.Now;
                var hora = new TimeSpan(0, fecha.Hour, fecha.Minute, fecha.Second);

                if (req.GridCab != null && req.GridCab.FECHA != null)
                {
                    fecha        = req.GridCab.FECHA.Value;
                    req.sucursal = req.GridCab.CODALMACEN;
                    req.numdpto = (short)req.GridCab.NUMDPTO;
                }


                var cab = db.IT_INV_MENSUALES_CAB.FirstOrDefault(f =>
                    f.FECHA == fecha &&
                    f.CODALMACEN == req.sucursal &&
                    f.NUMDPTO == req.numdpto &&
                    f.ESTATUS == 1);


                if (cab == null) // Registro nuevo
                {

                    //-----------------------------------
                    //ID DESDE 1
                    //-----------------------------------

                    var id = db.IT_INV_MENSUALES_CAB.MaxID1(x => x.ID);

                    var nw = new IT_INV_MENSUALES_CAB
                    {
                        ID         = id,
                        FECHA      = fecha,
                        HORA       = hora,
                        CODALMACEN = req.sucursal,
                        ESTATUS    = 1,
                        NUMDPTO    = req.numdpto,
                    };

                    db.Add(nw);

                    req.data2.ForEach(x =>
                    {
                        var lu = new IT_INV_MENSUALES_LIN
                        {
                            ID                = id,
                            REFERENCIA        = x.REFERENCIA,
                            CODARTICULO       = x.CODARTICULO,
                            DESCRIPCION       = x.DESCRIPCION,
                            EXISTENCIA_ACTUAL = x.EXISTENCIA_ACTUAL,
                            UNIDADMEDIDA      = x.UNIDADMEDIDA
                        };

                        db.Add(lu);

                    });


                    db.SaveChanges();

                }
                else
                { // Actualiza


                    db.IT_INV_MENSUALES_LIN.Where(w=>w.ID == cab.ID).ExecuteDelete();

                    req.data2.ForEach(x =>
                    {
                        var lu = new IT_INV_MENSUALES_LIN
                        {
                            ID                = cab.ID,
                            REFERENCIA        = x.REFERENCIA,
                            CODARTICULO       = x.CODARTICULO,
                            DESCRIPCION       = x.DESCRIPCION,
                            EXISTENCIA_ACTUAL = x.EXISTENCIA_ACTUAL,
                            UNIDADMEDIDA      = x.UNIDADMEDIDA
                        };

                        db.Add(lu);

                    });

                    db.SaveChanges();

                }



                return new { code = 0 }.RSon();
            }
         
            catch (Exception e)
            {

                return new { code = -1, msg = e.Message }.RSon();

            }
        }


        [HttpPost]
        public IActionResult GetDepartamentos([FromBody] Request2 req)
        {

            try
            {
                MLAVIDContext db = new MLAVID_DB();

                //var rs = db.DEPARTAMENTO.Where(w => w.VISIBLEWEB == "1" ).ToList().OrderBy(w => w.DESCRIPCION);

                var rs = db.Procedures.IT_DEPARTAMENTOAsync(req.sucursal).Result.ToList();


                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }


        }

        [HttpPost]
        public IActionResult EDIT([FromBody] Request2 req)
        {


            try
            {
            
                MLAVIDContext db = new MLAVID_DB();


                var rs = db.IT_INV_MENSUALES_LIN.Where(w => w.ID == req.id).ToList();
                

                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }


        }


        [HttpPost]
        public IActionResult DELETE([FromBody] Request2 req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                db.IT_INV_MENSUALES_CAB.Where(w => w.ID == req.id).ExecuteDelete();
                db.SaveChanges();

                return new { code = 0 }.RSon();
            }

            catch (Exception e)
            {

                return new { code = -1, msg = e.Message }.RSon();

            }


        }



        [HttpPost]
        public IActionResult GetInventarios([FromBody] Request2 req)
        {

            try
            {
                MLAVIDContext db = new MLAVID_DB();

                //var rs = db.DEPARTAMENTO.Where(w => w.VISIBLEWEB == "1" ).ToList().OrderBy(w => w.DESCRIPCION);

                var dep = db.DEPARTAMENTO.ToList();
                var suc = db.ALMACEN.ToList();

                var rs = db.IT_INV_MENSUALES_CAB.OrderByDescending(o=>o.ID).ToList();

                foreach (var it in rs)
                {
                    var f = it.FECHA.Value;
                    var h = it.HORA.Value;

                    var fecha = new DateTime(f.Year, f.Month, f.Day, h.Hours, h.Minutes, h.Seconds);
                    var _fecha = fecha.ToString("MMM").TrimEnd(".").ToTitleCase() + fecha.ToString(" yyyy d hh:mm tt");


                    it._ALMACEN = suc.FirstOrDefault(f => f.CODALMACEN == it.CODALMACEN)?.NOMBREALMACEN;
                    it._DEPARTAMENTO = dep.FirstOrDefault(f => f.NUMDPTO == it.NUMDPTO)?.DESCRIPCION;

                    it._DETALLE    = "Inventario Físico " + it.ID;
                    it._REFERENCIA = it._DEPARTAMENTO + " " + it._ALMACEN + " " + _fecha;

                    if (it.ESTATUS == 1)
                    {
                        it._ESTATUS = "POR ENVIAR";
                    }

                    if (it.ESTATUS == 2)
                    {
                        it._ESTATUS = "ENVIADO";
                    }
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
    }
}


