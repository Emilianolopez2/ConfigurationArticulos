using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;

namespace MvcWebPage.Controllers
{

    [Authorize(Roles = "PROVEEDOR,SUCURSAL")]
    public class PedidosController : Controller
    {
        public IActionResult Pedidos()
        {
            
            ViewData["usuario"]    = Extensions.GetNomProveedor();
            ViewData["sucursal"]   = Extensions.GetCodAlmacen();
            ViewData["lbSucursal"] = Extensions.GetlbSucursal();
            ViewData["fecha"]      = DateTime.Now.ToString("dd/MM/yyyy");

            //var ss = HttpContext.Session.GetString("UserName");

            return View("Pedidos");
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
                var rs = PedidosService.GetPedidosAutorizados(req);


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



        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile xmlFile, IFormFile pdfFile, int xid)
        {
            //long totalBytes    = xmlFile.Length + (pdfFile?.Length ?? 0);
            //long uploadedBytes = 0;


            var gui = Guid.NewGuid();

            string xmlFileName = gui + ".xml";
            string pdfFileName = gui + ".pdf";

            string xmlFilePath = "C:\\Archivos\\" + xmlFileName;
            string pdfFilePath = "C:\\Archivos\\" + pdfFileName;

            try
            {
                
                using (var xmlFileStream = new FileStream(xmlFilePath, FileMode.Create))
                {
                    xmlFile.CopyTo(xmlFileStream);
                    //uploadedBytes    += xmlFile.Length;
                    //ViewBag.Progress =  (int)(uploadedBytes * 100 / totalBytes);
                }

                if (pdfFile != null)
                {
                    using (var pdfFileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(pdfFileStream);
                       // uploadedBytes    += pdfFile.Length;
                       // ViewBag.Progress =  (int)(uploadedBytes * 100 / totalBytes);
                    }
                }
            }
            catch (Exception e)
            {
                return new { code = -1, msg = e.Message }.RSon();
            }

            return new { code = 0, msg = "Archivos procesados correctamente."}.RSon();
        }

        //[HttpPost]
        //public async Task<IActionResult> GuardarDocumento(Documento documento, IFormFile archivo)
        //{
        //    if (archivo != null && archivo.Length > 0)
        //    {
        //        documento.Nombre    = archivo.FileName;
        //        documento.Extension = Path.GetExtension(archivo.FileName);
        //        using (var ms = new MemoryStream())
        //        {
        //            await archivo.CopyToAsync(ms);
        //            documento.Contenido = ms.ToArray();
        //        }
        //    }
        //    db.Add(documento);
        //    db.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}

        public class Documento
        {
            public int    Id        { get; set; }
            public string Nombre    { get; set; }
            public string Extension { get; set; }
            public byte[] Contenido { get; set; }
        }
    }
}
