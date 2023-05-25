//using MvcWebPage.Data;
//using MvcWebPage.MLAVID;
//using MvcWebPage.Models;

//namespace MvcWebPage.Services
//{
//    public static class DepartamentoService
//    {
//        public static List<IT_DEPARTAMENTOResult> GetDepartamentos(Request req)
//        {
//            MLAVIDContext db = new MLAVID_DB();

//            List<IT_PEDIDOS_CAB> sc = null;


//            if (req.sucursal != "")
//            {
//                sc = db.IT_PEDIDOS_CAB.Where(w => w.CODALMACEN == req.sucursal).ToList();
//            }


//            var rs = db.Procedures.IT_DEPARTAMENTOAsync(req.sucursal).Result
//                .Select(s => new IT_DEPARTAMENTOResult()
//                {
//                    DESCRIPCION = s.DESCRIPCION,
//                    NUMDPTO     = s.NUMDPTO,
//                    MODIFICADO  = GetModificado(sc, s),
//                    ESTATUS     = GetEstatus(sc, s),


//                }).ToList();

//            //s[0].MODIFICADO = 1;
//            return rs;
//        }

//        private static int GetModificado(List<IT_PEDIDOS_CAB> sc, IT_DEPARTAMENTOResult s)
//        {
//            if (sc != null)
//            {
//                var it = sc.FirstOrDefault(f => f.NUMDPTO == s.NUMDPTO && f.FECHA != null && f.FECHA.Value.Date == DateTime.Now.Date && f.ESTATUS == 1);
//                if (it != null)
//                {
//                    return it.MODIFICADO.Int32();
//                }
//            }


//            return 0;
//        }

//        private static int GetEstatus(List<IT_PEDIDOS_CAB> sc, IT_DEPARTAMENTOResult s)
//        {
//            if (sc != null)
//            {
//                var it = sc.FirstOrDefault(f => f.NUMDPTO == s.NUMDPTO && f.FECHA != null && f.FECHA.Value.Date == DateTime.Now.Date && f.ESTATUS == 1);
//                if (it != null)
//                {
//                    return it.ESTATUS.Int32();
//                }
//            }


//            return 0;
//        }

//        public static List<IT_PEDIDOSResult> GetDptoData(Request req)
//        {

//            MLAVIDContext db = new MLAVID_DB();


//            int year = DateTime.Now.Year;
//            int month = DateTime.Now.Month;
//            int day = DateTime.Now.Day;

//            if (!string.IsNullOrEmpty(req.fecha))
//            {
//                year = DateTime.Parse(req.fecha).Year;
//                month = DateTime.Parse(req.fecha).Month;
//                day = DateTime.Parse(req.fecha).Day;
//            }

//            var rs = db.Procedures.IT_PEDIDOSAsync(
//                req.sucursal,
//                req.numdpto,
//                year,
//                month,
//                day).Result.ToList();



//            if (req.excluir)
//            {
//                rs.RemoveAll(r => r.CANTIDAD == null || r.CANTIDAD == 0);
//            }


//            foreach (var it in rs)
//            {

//                /*
//                if (it.REFPROVEEDOR == "VER046-M")
//                {
//                    it.MULTIPLOPEDIR = 3;
//                }

//                if (it.REFPROVEEDOR == "VER045-M")
//                {
//                    it.MULTIPLOPEDIR = 5;
//                }
//                if (it.REFPROVEEDOR == "VER067-M")
//                {
//                    it.MULTIPLOPEDIR = 2;
//                }
//                */

//                if (it.SUGERIDO == null)
//                {
//                    it.SUGERIDO = 0;
//                }


//                if (it.MULTIPLOPEDIR == null)
//                {
//                    it.MULTIPLOPEDIR = 0;
//                }



//                /*****************************************************/
//                var cal1 = (it.SUGERIDO.Value / it.MULTIPLOPEDIR.Value);
//                it.SUGERIDO = Math.Round(cal1) * it.MULTIPLOPEDIR;


//                /****************************************************/


//                it.SUGERIDO3 = it.SUGERIDO3.Str().Truncate(2).Dbl();
//                //it.SUGERIDO_COPIA = it.SUGERIDO_COPIA.Str().Truncate(2).Dbl();

//                if (it.CANTIDAD == null)
//                {

//                    if (it.SUGERIDO != 0)
//                    {
//                        if (it.CANTIDAD_PEDIDA > it.SUGERIDO)
//                        {
//                            it.COLOR = 2;
//                        }

//                        if (it.CANTIDAD_PEDIDA < it.SUGERIDO)
//                        {
//                            it.COLOR = 1;
//                        }
//                    }
//                }
//                else
//                {
//                    if (it.SUGERIDO != 0)
//                    {
//                        if (it.CANTIDAD > it.SUGERIDO)
//                        {
//                            it.COLOR = 2;
//                        }

//                        if (it.CANTIDAD < it.SUGERIDO)
//                        {
//                            it.COLOR = 1;
//                        }
//                    }
//                }



//                it.COLOR_TMP = it.COLOR;

//                it.CANTIDAD_PEDIDA ??= 0;

//                it.SUGERIDO2 = it.SUGERIDO - it.CANTIDAD_PEDIDA;
//                if (it.SUGERIDO2 < 0)
//                {
//                    it.SUGERIDO2 = 0;
//                }


//                if (it.CANTIDAD == null)
//                {
//                    it.CANTIDAD = 0;
//                }

//                if (it.CANTIDAD == 0)
//                {
//                    it.CANTIDAD = it.SUGERIDO2;

//                }
//            }

//            //if (req.act == 1)
//            //{
//            //    foreach (var it in rs)
//            //    {



//            //        //it.CANTIDAD_PEDIDA_TMP = it.CANTIDAD_PEDIDA;



//            //       // it.CANTIDAD = it.SUGERIDO2;



//            //    }
//            //}
//            return rs;
//        }
//    }
//}
