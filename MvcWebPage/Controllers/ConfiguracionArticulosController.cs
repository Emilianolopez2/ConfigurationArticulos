using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;
using MvcWebPage.Xlsx;
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
        public async Task<IActionResult> Excel(IFormFile xmlFile)
        {
            try
            {

                var ws = ArchivoExcel.GetExcel(xmlFile).ToList();



                MLAVIDContext db = new MLAVID_DB();


                string extension = Path.GetExtension(xmlFile.FileName).ToLower();

                int codArt = 0;
                bool flag = false;
                ARTICULOS artTemp = null;
                FORMATOSARTICULOS fArtTemp = null;

                if (extension != ".xlsx")
                {
                    return new { code = -1, msg = "El archivo debe ser xlsx" }.RSon();
                }


                var update = new List<IT_ARTICULOS_PROVEEDOR>();
                var insert = new List<IT_ARTICULOS_PROVEEDOR>();

                foreach (var it2 in ws)
                {

                    codArt = 0;
                    fArtTemp = null;
                    artTemp = null;
                    flag = false;

                    fArtTemp = db.FORMATOSARTICULOS.FirstOrDefault(x => x.CODBARRAS == it2.REFPROVEEDOR);
                    if (fArtTemp != null)
                    {
                        codArt = fArtTemp.CODARTICULO;
                    }

                    if (codArt > 0)
                    {
                        flag = true;
                    }

                    if (!flag)
                    {
                        artTemp = db.ARTICULOS.FirstOrDefault(x => x.REFPROVEEDOR == it2.REFPROVEEDOR);
                        if (artTemp != null)
                        {
                            codArt = artTemp.CODARTICULO;
                        }

                        if (codArt > 0)
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        var it = new IT_ARTICULOS_PROVEEDOR()
                        {
                            REFERENCIA = it2.REFPROVEEDOR,
                            CODARTICULO = codArt,
                            DESCRIPCION = it2.DESCRIPCION,
                            DOSIS = Double.Parse(it2.DOSIS),
                            UNIDADM = it2.UNIDADM,
                            UNIDADMD = it2.UNIDADMDES,
                            CODALMACEN = it2.SUCURSAL,
                            PROVEEDOR_A = it2.PROVEEDOR_A,
                            PORCENTAJE_A = Double.Parse(it2.PORCENTAJE_A),
                            PRECIO_A = Double.Parse(it2.PRECIO_A),
                            CODIGO_A = it2.CODIGO_A,
                            PROVEEDOR_B = it2.PROVEEDOR_B,
                            PORCENTAJE_B = Double.Parse(it2.PORCENTAJE_B),
                            PRECIO_B = Double.Parse(it2.PRECIO_B),
                            CODIGO_B = it2.CODIGO_B,
                            PROVEEDOR_C = it2.PROVEEDOR_C,
                            PORCENTAJE_C = Double.Parse(it2.PORCENTAJE_C),
                            PRECIO_C = Double.Parse(it2.PRECIO_C),
                            CODIGO_C = it2.CODIGO_C,
                            LEADTIME = Double.Parse(it2.LEADTIME),
                            STOCK_SEGURIDAD = Double.Parse(it2.STOCK_SEGURIDAD),
                            STOCK_MAXIMO = Double.Parse(it2.STOCK_MAXIMO),
                            FRECUENCIA = Double.Parse(it2.FRECUENCIA)
                        };

                        var it_articulo = db.IT_ARTICULOS_PROVEEDOR.FirstOrDefault(x => x.REFERENCIA == it.REFERENCIA && x.CODALMACEN == it.CODALMACEN);
                        if (it_articulo != null)
                        {//se va a actualizar
                            //db.Entry(it_articulo).CurrentValues.SetValues(it);
                            //db.Entry(it_articulo).State = EntityState.Modified;
                            update.Add(it);

                        }
                        else // se crea uno nuevo
                        {
                            //db.IT_ARTICULOS_PROVEEDOR2.Add(it);
                            insert.Add(it);
                        }
                        //db.SaveChanges();
                    }

                }


                db.BulkUpdate(update, options =>

                    options.ColumnPrimaryKeyExpression = op => new
                    {
                        op.REFERENCIA,
                        op.CODALMACEN,
                    }
                );

                db.BulkInsert(insert, options =>

                    options.ColumnPrimaryKeyExpression = op => new
                    {
                        op.REFERENCIA,
                        op.CODALMACEN,
                    });


            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
            }

            return new { code = 0, msg = "Archivo procesado correctamente." }.RSon();
        }






        //[HttpPost]
        //public async Task<IActionResult> Excel(IFormFile xmlFile)
        //{
        //    try
        //    {
        //        MLAVIDContext db = new MLAVID_DB();
        //        string extension = Path.GetExtension(xmlFile.FileName);
        //        int codArt = 0;
        //        bool flag = false;
        //        ARTICULOS artTemp = null;
        //        FORMATOSARTICULOS fArtTemp = null;
        //        if (extension != ".csv")
        //        return Json(new { status = "FAIl", message = "El archivo debe ser CSV" });
        //        using (var reader = new StreamReader(xmlFile.OpenReadStream()))
        //        {
        //            var a1 = reader.ReadLine();

        //            while (!reader.EndOfStream)
        //            {
        //                codArt = 0;
        //                fArtTemp = null;
        //                artTemp = null;
        //                flag = false;
        //                string[] row = reader.ReadLine().Split('|');
        //                fArtTemp = db.FORMATOSARTICULOS.FirstOrDefault(x => x.CODBARRAS == row[0]);
        //                if (fArtTemp != null)
        //                    codArt = fArtTemp.CODARTICULO;
        //                if (codArt > 0)
        //                    flag = true;
        //                if (!flag)
        //                {
        //                    artTemp = db.ARTICULOS.FirstOrDefault(x => x.REFPROVEEDOR == row[0]);
        //                    if (artTemp != null)
        //                        codArt = artTemp.CODARTICULO;
        //                    if (codArt > 0)
        //                        flag = true;
        //                }
        //                if (flag)
        //                {
        //                    IT_ARTICULOS_PROVEEDOR it = new IT_ARTICULOS_PROVEEDOR()
        //                    {
        //                        REFERENCIA = row[0],
        //                        CODARTICULO = codArt,
        //                        DESCRIPCION = row[1],
        //                        DOSIS = Double.Parse(row[2]),
        //                        UNIDADM = row[3],
        //                        UNIDADMD = row[4],
        //                        CODALMACEN = row[5],
        //                        PROVEEDOR_A = row[6],
        //                        PORCENTAJE_A = Double.Parse(row[7]),
        //                        PRECIO_A = Double.Parse(row[8]),
        //                        CODIGO_A = row[9],
        //                        PROVEEDOR_B = row[10],
        //                        PORCENTAJE_B = Double.Parse(row[11]),
        //                        PRECIO_B = Double.Parse(row[12]),
        //                        CODIGO_B = row[13],
        //                        PROVEEDOR_C = row[14],
        //                        PORCENTAJE_C = Double.Parse(row[15]),
        //                        PRECIO_C = Double.Parse(row[16]),
        //                        CODIGO_C = row[17],
        //                        LEADTIME = Double.Parse(row[18]),
        //                        STOCK_SEGURIDAD = Double.Parse(row[19]),
        //                        STOCK_MAXIMO = Double.Parse(row[20]),
        //                        FRECUENCIA = Double.Parse(row[21])
        //                    };

        //                    var it_articulo = db.IT_ARTICULOS_PROVEEDOR.FirstOrDefault(x => x.REFERENCIA == it.REFERENCIA && x.CODALMACEN == it.CODALMACEN);
        //                    if (it_articulo != null)
        //                    {//se va a actualizar
        //                        db.Entry(it_articulo).CurrentValues.SetValues(it);
        //                        db.Entry(it_articulo).State = EntityState.Modified;
        //                    }
        //                    else // se crea uno nuevo
        //                    {
        //                        db.IT_ARTICULOS_PROVEEDOR.Add(it);
        //                    }
        //                    db.SaveChanges();
        //                }
        //            }
        //        }

        //        //return Json(new { status = "OK", message = "Datos cargados con éxito" });
        //    }
        //    catch (Exception e)
        //    {
        //        return new { code = -1, msg = e.Message }.RSon();
        //    }

        //    return new { code = 0, msg = "Archivos procesados correctamente." }.RSon();
        //}


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

                if (req != null)
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
