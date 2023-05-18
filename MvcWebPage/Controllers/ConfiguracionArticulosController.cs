using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcWebPage.Controllers
{


    public class ConfiguracionArticulosController : Controller
    {
          private readonly MLAVIDContext _context;
        public IActionResult ConfiguracionArticulos()
        {

            ViewData["usuario"]    = Extensions.GetUserName();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");


            MLAVIDContext db = new MLAVID_DB();



            var LCOMBO = db.ALMACEN.Select(s => new ALMACEN
            {
                NOMBREALMACEN = s.NOMBREALMACEN,
                CODALMACEN    = s.CODALMACEN
            }).ToList();

            ViewData["COMBO"] = LCOMBO;

            return View("ConfiguracionArticulos");
        }


        [HttpPost]
        public IActionResult GetArticulos()
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var rs = db.ARTICULOS.Select(s => s.DESCRIPCION).ToList();
                return new { code = 0, rs }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }

        [HttpPost]
        public IActionResult GetReferencia(IT_ARTICULOS_PROVEEDOR iT_ARTICULOS_PROVEEDOR)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                //var rs = db.ARTICULOS.Select(s => s.REFPROVEEDOR).ToList();

                var rs2 = db.IT_ARTICULOS_PROVEEDOR.Select(s => s.REFERENCIA).ToList();


                return new { code = 0, rs2 }.RSon();
            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }
        [HttpPost]
        public IActionResult GetReferencia2([FromBody] Request3 req)
        {
            try
            {
                if (req == null)
                {
                    return new { code = -1, rs = "[]" }.RSon();
                }


                MLAVIDContext db = new MLAVID_DB();
                var rs = db.IT_ARTICULOS_PROVEEDOR.Where(w => w.CODALMACEN == req.sucursal)
                    .Select(w => w.REFERENCIA).Distinct().ToList();

                
                return new { code = 0, rs }.RSon();

            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }


        [HttpPost]
        public IActionResult GetDescripcion2([FromBody] Request3 req)
        {
            try
            {
                if (req == null)
                {
                    return new { code = -1, rs = "[]" }.RSon();
                }

                MLAVIDContext db = new MLAVID_DB();

                var dato = req.refart;

                var art = db.IT_ARTICULOS_PROVEEDOR.Where(f => f.REFERENCIA == dato).ToList();

                var rs2 = art.Select(w => w.DESCRIPCION).ToArray();


                var rs = rs2[0];
                //if (art != null)
                //{
                //    //var rs = db.ARTICULOS.Where(w => w.REFPROVEEDOR == art.REFERENCIA).Select(w => w.DESCRIPCION).ToArray();


                //    //string[] rs = rs2.Distinct().ToArray();


                //    return new { code = 0, rs }.RSon();
                //}


                //var rs2 = db.IT_ARTICULOS_PROVEEDOR.Select(s => s.REFERENCIA).ToList();

                return new { code = -1, rs }.RSon();

            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }


        [HttpPost]
        public IActionResult GetDatosgenerales([FromBody] Request3 req)
        {
            try
            {
                if (req == null)
                {
                    return new { code = -1, rs = "[]" }.RSon();
                }




                //var dato = req.refart;

                //var art = db.IT_ARTICULOS_PROVEEDOR.Where(f => f.REFERENCIA == dato).ToList();

                //var rs2 = art.Select(w => w.DESCRIPCION).ToArray();


                //var rs = rs2[0];
                //var generales = req.datosg;

                MLAVIDContext db = new MLAVID_DB();

                var ref1       = req.referencia;
                var desc       = req.descripcion;


                //var gr3 = db.IT_ARTICULOS_PROVEEDOR.Where(s => s.REFERENCIA == ref1).Select(z => z.DESCRIPCION).ToArray();

                //gr[0]
                var rs = db.Procedures.IT_ART_PROVAsync(ref1, req.sucursal).Result
                    .Select(s => new
                    {
                        s.DOSIS,
                        s.UNIDADM,
                        s.UNIDADMD,
                        s.LEADTIME,
                        s.STOCK_SEGURIDAD,
                        s.STOCK_MAXIMO,
                        s.FRECUENCIA,

                        s.PROVEEDOR_A,
                        s.PORCENTAJE_A,
                        s.PRECIO_A,
                        s.CODIGO_A,
                        s.PROVEEDOR_B,
                        s.PORCENTAJE_B,
                        s.PRECIO_B,
                        s.CODIGO_B
                    }).FirstOrDefault();


                if (rs==null)
                {
                    return new { code = -1, rs="[]" }.RSon();
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
        public IActionResult GetReferencias([FromBody] Request3 req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var art = db.ARTICULOS.FirstOrDefault(f =>
                    f.DESCRIPCION.ToLower().Trim() == req.referencia.ToLower().Trim());

                if (art != null)
                {
                    var rs = db.IT_ARTICULOS_PROVEEDOR.Where(w => w.CODARTICULO == art.CODARTICULO && w.CODALMACEN == req.sucursal)
                        .GroupBy(g=>g.REFERENCIA)
                        .Select(w => new IT_ARTICULOS_PROVEEDOR
                        {
                            REFERENCIA = w.Key,
                            DESCRIPCION = w.First().DESCRIPCION
                        }).ToList();

                    if (rs.Count > 0)
                    {
                        var lng = rs.Select(a => a.REFERENCIA.Length).Max();
                        rs.ForEach(f =>
                        {
                            f.DESCRIPCION = f.REFERENCIA.PadRight(lng) + " **" + f.DESCRIPCION;

                        });

                    }




                    return new { code = 0, rs }.RSon();
                }


                //var rs2 = db.IT_ARTICULOS_PROVEEDOR.Select(s => s.REFERENCIA).ToList();

                return new { code = -1, rs = "[]" }.RSon();

            }
            catch (Exception e)
            {
                return new { code = -1, rs = "[]", msg = e.Message }.RSon();
            }
        }

        [HttpPost]
        public IActionResult updateArticulo([FromBody]IT_ARTICULOS_PROVEEDOR iT_ARTICULOS) //se debe de mandar en un nuevo modelo el nombre del almacen 
        {
            MLAVIDContext db = new MLAVID_DB();
           
            IT_ARTICULOS_PROVEEDOR it_articulo = db.IT_ARTICULOS_PROVEEDOR.FirstOrDefault(x => x.REFERENCIA == iT_ARTICULOS.REFERENCIA && x.CODALMACEN == iT_ARTICULOS.CODALMACEN);
            it_articulo.DOSIS = iT_ARTICULOS.DOSIS;
            it_articulo.UNIDADM = iT_ARTICULOS.UNIDADM;
            it_articulo.UNIDADMD= iT_ARTICULOS.UNIDADMD;
            it_articulo.LEADTIME = iT_ARTICULOS.LEADTIME;
            it_articulo.STOCK_MAXIMO = iT_ARTICULOS.STOCK_MAXIMO;
            it_articulo.STOCK_SEGURIDAD = iT_ARTICULOS.STOCK_SEGURIDAD;
            it_articulo.FRECUENCIA = iT_ARTICULOS.FRECUENCIA;

            it_articulo.PROVEEDOR_A = iT_ARTICULOS.PROVEEDOR_A;
            it_articulo.PORCENTAJE_A = iT_ARTICULOS.PORCENTAJE_A;
            it_articulo.PRECIO_A = iT_ARTICULOS.PRECIO_A;
            it_articulo.CODIGO_A = iT_ARTICULOS.CODIGO_A;

            it_articulo.PROVEEDOR_B = iT_ARTICULOS.PROVEEDOR_B;
            it_articulo.PORCENTAJE_B = iT_ARTICULOS.PORCENTAJE_B;
            it_articulo.PRECIO_B = iT_ARTICULOS.PRECIO_B;
            it_articulo.CODIGO_B = iT_ARTICULOS.CODIGO_B;

            db.Entry(it_articulo).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new {res="OK",message="El Articulo " + it_articulo.DESCRIPCION + " ha sido actualizado corectamente"});
        }



        ///EDITAAAR//
        //public async Task<IActionResult>Editar(int? id)
        //{
        //    if (id == null || _context.IT_ARTICULOS_PROVEEDOR == null)
        //    {
        //        return NotFound();
        //    }

        //    var IT_ARTICULOS_PROVEEDOR = await _context.IT_ARTICULOS_PROVEEDOR.FindAsync(id);
        //    if (IT_ARTICULOS_PROVEEDOR == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(IT_ARTICULOS_PROVEEDOR);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Editar(string id, [Bind("REFERENCIA,CODARTICULO,")] IT_ARTICULOS_PROVEEDOR iT_ARTICULOS_PROVEEDOR)
        //{
        //    if (id != iT_ARTICULOS_PROVEEDOR.REFERENCIA)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(iT_ARTICULOS_PROVEEDOR);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!iT_ARTICULOS_PROVEEDORExists(iT_ARTICULOS_PROVEEDOR.REFERENCIA)) 
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(iT_ARTICULOS_PROVEEDOR);
        //}


        [HttpPost]
        public IActionResult GetArtDescripcion([FromBody] Request3 req)
        {
            try
            {
                MLAVIDContext db = new MLAVID_DB();

                var it = db.IT_ARTICULOS_PROVEEDOR.FirstOrDefault(w => w.REFERENCIA == req.referencia && w.CODALMACEN == req.sucursal);

                if (it != null)
                {

                    return new { code = 0, rs= it.DESCRIPCION }.RSon();
                }


                

                return new { code = -1, rs = "" }.RSon();

            }
            catch (Exception e)
            {
                return new { code = -1, rs = "", msg = e.Message }.RSon();
            }
        }
    }
    
}
