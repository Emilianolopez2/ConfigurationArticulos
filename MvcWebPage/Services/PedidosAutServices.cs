using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;

namespace MvcWebPage.Services
{
    public static class PedidosAutServices
    {

        public static List<IT_PEDIDOS_AUTResult> GetPedidosAutorizados(Request req)
        {
            MLAVIDContext db = new MLAVID_DB();


            var f1 = DateTime.Parse(req.fecha1);
            var f2 = DateTime.Parse(req.fecha2);

            var rs = db.Procedures.IT_PEDIDOS_AUTAsync(
                req.sucursal,
                req.proveedor,
                f1.Year,
                f1.Month,
                f1.Day,
                f2.Year,
                f2.Month,
                f2.Day).Result.ToList();

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


        public static List<IT_PEDIDOS_AUTResult> GetPedidosAutorizados2(Request req)
        {
            MLAVIDContext db = new MLAVID_DB();


            var f1 = DateTime.Parse(req.fecha1);
            var f2 = DateTime.Parse(req.fecha2);

            var rs = db.Procedures.IT_PEDIDOS_AUTAsync(
                req.sucursal,
                req.proveedor,
                f1.Year,
                f1.Month,
                f1.Day,
                f2.Year,
                f2.Month,
                f2.Day).Result.Where(w => req.estatus == null || w.IDESTADO == req.estatus).ToList();

            rs.ForEach(f =>
            {


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


        public static (List<IT_PEDIDOS_AUT_DETResult> rs, string monedaDesc, string monedaCot, REM_FRONTS rs2, PROVEEDORES rs3)
            GetPedidosAutorizadosDet(Request req)
        {
            MLAVIDContext db = new MLAVID_DB();


            var rs = db.Procedures.IT_PEDIDOS_AUT_DETAsync(
                req.id,
                req.numserie,
                req.numpedido).Result.ToList();

            foreach (var it in rs)
            {
                if (it.SUGERIDO == null)
                {
                    it.SUGERIDO = 0;
                }

                if (it.CANTIDAD > it.SUGERIDO)
                {
                    it.COLOR = 2;
                }

                if (it.CANTIDAD < it.SUGERIDO)
                {
                    it.COLOR = 1;
                }


                if (it.TOTIVA == null)
                {
                    it.TOTIVA = 0;
                }

                if (it.TOTREQ == null)
                {
                    it.TOTREQ = 0;
                }

                it.TOTAL = it.TOTIVA + it.TOTREQ + it.TOTALLINEA;
            }

            string monedaDesc = "";
            string monedaCot = "";

            var prov = db.PROVEEDORES.FirstOrDefault(f => f.CODPROVEEDOR == req.codproveedor);

            if (prov != null && prov.CODMONEDA != null)
            {
                var moneda = db.MONEDAS.FirstOrDefault(f => f.CODMONEDA == prov.CODMONEDA);
                if (moneda != null)
                {
                    monedaDesc = moneda.DESCRIPCION;
                    monedaCot = moneda.COTDEF.Str();
                }
            }

            var rs2 = db.REM_FRONTS.Where(w => w.ALMACEN == req.sucursal).Select(s => new REM_FRONTS
            {
                NOMBRE     = s.NOMBRE.Str(),
                DIRECCION  = s.DIRECCION.Str(),
                DIRECCION2 = s.DIRECCION2.Str(),
                PROVINCIA  = s.PROVINCIA.Str(),
                CODPOSTAL  = s.CODPOSTAL.Str(),
                CIF        = s.CIF.Str(),
                PAIS       = s.PAIS.Str()
            }).FirstOrDefault();


            var rs3 = db.PROVEEDORES.Where(w => w.CODPROVEEDOR == req.codproveedor).Select(s => new PROVEEDORES
            {
                NOMPROVEEDOR = s.NOMPROVEEDOR.Str(),
                DIRECCION1   = s.DIRECCION1.Str(),
                DIRECCION2   = s.DIRECCION2.Str(),
                PROVINCIA    = s.PROVINCIA.Str(),
                CODPOSTAL    = s.CODPOSTAL.Str(),
                CIF          = s.CIF.Str(),
                PAIS         = s.PAIS.Str(),
                TELEFONO1    = s.TELEFONO1.Str(),
                TELEFONO2    = s.TELEFONO2.Str(),
            }).FirstOrDefault();

            return (rs, monedaDesc, monedaCot, rs2, rs3);
        }
    }
}
