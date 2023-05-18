using Microsoft.EntityFrameworkCore;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace MvcWebPage.Services
{
    public class PedidosService
    {

        public static List<IT_PEDIDOS_AUTPResult> GetPedidosAutorizados(Request req)
        {
            MLAVIDContext db = new MLAVID_DB();


            var f1 = DateTime.Parse(req.fecha1);
            var f2 = DateTime.Parse(req.fecha2);

            var rs = db.Procedures.IT_PEDIDOS_AUTPAsync(
                req.sucursal,
                2,
                f1.Year,
                f1.Month,
                f1.Day,
                f2.Year,
                f2.Month,
                f2.Day).Result.Where(w => w.CODPROVEEDOR == Extensions.GetCodProveedor() && w.IDESTADO == 2).ToList();

            rs.ForEach(f =>
            {
                if (f.FECHAPEDIDO != null)
                {
                    f.ENTREGA_ESTIMADA = f.FECHAPEDIDO.Value.AddDays(1);
                }

                f.ORDEN_DE_COMPRA = "Orden <br>" + f.ID_ORDEN;

                //f.NOMPROVEEDOR = "DISTRIBUIDORA ALIMENTICIA PARA HOTELES Y RESTAURANTES, S. DE R.L.";

                switch (f.IDESTADO)
                {
                    case 0:
                        f.ESTATUS = "Por Autorizar";
                        break;
                    case 1:
                        f.ESTATUS = "Rechazado";
                        break;
                    case 2:
                        f.ESTATUS = "Entrega Pendiente";
                        break;
                    case 3:
                        f.ESTATUS = "En Transito";
                        break;
                }
            });
            return rs;
        }
        public static List<IT_PEDIDOS_CAB2Result> GetPedidos(Request req)
        {
            if (string.IsNullOrEmpty(req.sucursal))
            {
                req.sucursal = "";
            }


            MLAVIDContext db = new MLAVID_DB();

            var rs = db.Procedures.IT_PEDIDOS_CAB2Async(req.sucursal).Result.Select(s =>
                new IT_PEDIDOS_CAB2Result()
                {
                    ID           = s.ID,
                    CODALMACEN   = s.CODALMACEN,
                    SUCURSAL     = s.SUCURSAL,
                    NUMDPTO      = s.NUMDPTO,
                    DEPARTAMENTO = s.DEPARTAMENTO,
                    FECHA        = s.FECHA,
                    DETALLE      = "Pedido " + s.ID,
                    ESTATUS2     = s.ESTATUS2,
                    ESTATUS      = s.ESTATUS,
                    REFERENCIA   = s.REFERENCIA + " " + GetFecha1(s)
                }).ToList();

            return rs;
        }

        private static string GetFecha1(IT_PEDIDOS_CAB2Result it)
        {
            var f = it.FECHA.Value;
            var h = it.HORA.Value;

            var fecha = new DateTime(f.Year, f.Month, f.Day, h.Hours, h.Minutes, h.Seconds);

            return fecha.ToString("MMM").TrimEnd(".").ToTitleCase() + fecha.ToString(" yyyy d hh:mm tt");

        }


        public static void SavePedidos(Request req, HttpContext context)
        {
            req.pedidos.RemoveAll(r => r.CANTIDAD == null || r.CANTIDAD == 0);


            MLAVIDContext db = new MLAVID_DB();
            MLAVIDContext db2 = new MLAVID_DB();


            var id = db.IT_PEDIDOS_CAB.Select(s => s.ID).DefaultIfEmpty().Max() + 1;

            var fecha = DateTime.Now;
            var hora = new TimeSpan(0, fecha.Hour, fecha.Minute, fecha.Second);


            if (!string.IsNullOrEmpty(req.fecha))
            {
                fecha = DateTime.Parse(req.fecha);
            }

            var cab = db.IT_PEDIDOS_CAB.FirstOrDefault(f =>
                f.FECHA == fecha &&
                f.CODALMACEN == req.sucursal &&
                f.NUMDPTO == req.numdpto &&
                f.ESTATUS == 1);

            if (cab != null)
            {
                var itp = db2.IT_PEDIDOS_LIN.Where(w => w.ID_CAB == cab.ID);

                var lin = itp.ToList();


                var result = lin.IntersectBy(req.pedidos.Select(x => new
                {
                    x.CANTIDAD,
                    x.REFERENCIA
                }), x => new
                {
                    x.CANTIDAD,
                    x.REFERENCIA
                }).ToList();


                if (result.Count != req.pedidos.Count)
                {
                    cab.MODIFICADO = 2;
                }

                itp.ExecuteDelete();
                foreach (var it in req.pedidos)
                {
                    db.IT_PEDIDOS_LIN.Add(new IT_PEDIDOS_LIN
                    {
                        ID_CAB = cab.ID,
                        CODALMACEN = req.sucursal,
                        CODARTICULO = it.CODARTICULO,
                        REFERENCIA = it.REFERENCIA,
                        CANTIDAD = it.CANTIDAD,
                        SUGERIDO = it.SUGERIDO,
                        SEMAFORO = it.COLOR,
                        CODFORMATO = it.CODFORMATO
                    });
                }

                db.UpdateRange(db.IT_PEDIDOS_LIN);

                db.SaveChanges();
            }
            else
            {
                db.IT_PEDIDOS_CAB.Add(new IT_PEDIDOS_CAB
                {
                    ID = id,
                    CODALMACEN = req.sucursal,
                    NUMDPTO = req.numdpto,
                    ESTATUS = 1,
                    FECHA = fecha,
                    HORA = hora,
                    MODIFICADO = 1,
                    CODVENDEDOR = Extensions.GetCodVendedor()
                });

                foreach (var it in req.pedidos)
                {
                    db.IT_PEDIDOS_LIN.Add(new IT_PEDIDOS_LIN
                    {
                        ID_CAB = id,
                        CODALMACEN = req.sucursal,
                        CODARTICULO = it.CODARTICULO,
                        REFERENCIA = it.REFERENCIA,
                        CANTIDAD = it.CANTIDAD,
                        SUGERIDO = it.SUGERIDO,
                        SEMAFORO = it.COLOR,
                        CODFORMATO = it.CODFORMATO
                    });
                }

                db.SaveChanges();
            }
        }
    }
}
