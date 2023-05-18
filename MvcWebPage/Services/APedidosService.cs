using System.Security.Claims;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;

namespace MvcWebPage.Services
{
    public class APedidosService
    {



        public static void ProcesarPedidos(Request req, HttpContext context)
        {

            MLAVIDContext db  = new MLAVID_DB();
            var tran = db.Database.BeginTransaction();

            try
            {

                /*
            DataTable dt = new DataTable();
            dt.Columns.Add("REFERENCIA", typeof(string));
            dt.Columns.Add("CODALMACEN", typeof(string));
            */
                var tmp1    = new Dictionary<PEDCOMPRACAB_KEY, PEDCOMPRACAB_KEY_LIN>();
                var tmp2    = new Dictionary<PEDCOMPRATOT_IMP_KEY, PEDCOMPRATOT>();

                //var npedido = db.PEDCOMPRACAB.Select(s => s.NUMPEDIDO).DefaultIfEmpty().Max();

                //var npedido = db.PEDCOMPRACAB.GroupBy(
                //    g => g.NUMSERIE
                //).ToDictionary(k => k.Key, k => k.Select(s => s.NUMPEDIDO).ToList().DefaultIfEmpty().Max());

                var npedido = db.PEDCOMPRACAB.GroupBy(
                    g => g.NUMSERIE
                ).ToDictionary(k => k.Key, k => k.MaxID1(s => s.NUMPEDIDO));


                /*
                var nnumero= db.PEDCOMPRATOT.GroupBy(
                    g => new PEDCOMPRATOT_KEY(g.SERIE,g.NUMERO)
                ).ToDictionary(k => k.Key, k => k.Select(s => s.NUMLINEA).ToList().DefaultIfEmpty().Max());
                */

                //var nid = db.IT_PEDIDOS_NUMPEDIDOS_CAB.GroupBy(
                //    g => new IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(g.ID) //, pcc.NUMSERIE,it.CODALMACEN
                //).ToDictionary(k => k.Key, k => k.Select(s => s.ID).ToList().DefaultIfEmpty().Max());


                var nid = db.IT_PEDIDOS_NUMPEDIDOS_CAB.MaxID1(s => s.ID);
                
                //var nid = db.IT_PEDIDOS_NUMPEDIDOS_CAB.Select(s => s.ID).DefaultIfEmpty().Max();
                //var idOrden = db.IT_PEDIDOS_NUMPEDIDOS_CAB.Select(s => s.ID_ORDEN).DefaultIfEmpty((int?)0).Max() + 1;

                //var idOrden = db.IT_PEDIDOS_NUMPEDIDOS_CAB.DefaultIfEmpty().Max(r => r == null ? 0 : r.ID_ORDEN) + 1;
                //var idOrden = db.IT_PEDIDOS_NUMPEDIDOS_CAB.MaxID(x => x == null ? 0 : x.ID_ORDEN) + 1;


                var f1 = DateTime.Now;


                //var nslin = new Dictionary<int,int>();
                //var nslin = db.IT_PEDIDOS_NUMPEDIDOS_CAB.Select(s => s.ID).DefaultIfEmpty().Max();

                var idOrden = db.IT_PEDIDOS_NUMPEDIDOS_CAB.MaxID1(s => s.ID_ORDEN, false);

                foreach (var it in req.cab_pedidos)
                {
                    idOrden++;

                    var rs = db.IT_PEDIDOS_LIN.Where(f => f.ID_CAB == it.ID).ToList();

                    foreach (var it2 in rs)
                    {

                        var iap = db.IT_ARTICULOS_PROVEEDOR.FirstOrDefault(
                            f => f.REFERENCIA.ToUpper() == it2.REFERENCIA.ToUpper() &&
                                 f.CODALMACEN == it.CODALMACEN);

                        if (iap != null)
                        {

                            if (iap.PORCENTAJE_A != null)
                            {
                                //******************************************************************//

                                if (string.IsNullOrEmpty(iap.PROVEEDOR_A) || iap.PROVEEDOR_A.ToUpper() == "NA")
                                {


                                    continue;
                                }


                                if (iap.PORCENTAJE_A == 1)
                                {

                                    var cprooveedorA = db.PROVEEDORES.FirstOrDefault(f => f.NIF20 == iap.PROVEEDOR_A);

                                    if (cprooveedorA == null)
                                    {

                                        continue;
                                    }

                                    var sKeyA = new PEDCOMPRACAB_KEY
                                    {
                                        CODALMACEN = it.CODALMACEN,
                                        PROVEEDOR  = iap.PROVEEDOR_A
                                    };

                                    it2.CODALMACEN = it.CODALMACEN;

                                    if (!tmp1.ContainsKey(sKeyA))
                                    {
                                        
                                        if (npedido.ContainsKey(it.CODALMACEN + "1P"))
                                        {
                                            npedido[it.CODALMACEN + "1P"]++;
                                        }
                                        else
                                        {
                                            npedido.Add(it.CODALMACEN + "1P",1);
                                        }

                                        var id = npedido[it.CODALMACEN + "1P"];


                                        var pcc = GetPedCompraCab(it, id, cprooveedorA.CODPROVEEDOR, f1);


                                        //---------------------------------------------------------------


                                        //var sKey=new IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(it.ID); //, pcc.NUMSERIE,it.CODALMACEN

                                        //if (nid.ContainsKey(sKey))
                                        //{
                                        //    nid[sKey]++;
                                        //}
                                        //else
                                        //{
                                        //    nid.Add(sKey, 1);
                                        //}

                                        //var sid = nid[sKey];

                                        nid++;

                                        db.IT_PEDIDOS_NUMPEDIDOS_CAB.Add(new IT_PEDIDOS_NUMPEDIDOS_CAB
                                        {
                                            ID          = nid,
                                            NUMPEDIDO   = pcc.NUMPEDIDO,
                                            CODALMACEN  = it.CODALMACEN,
                                            NUMSERIE    = pcc.NUMSERIE,
                                            FECHAPEDIDO = pcc.FECHAPEDIDO,
                                            NUMDPTO     = it.NUMDPTO,
                                            CODVENDEDOR = Extensions.GetCodVendedor(),
                                            ID_ORDEN    = idOrden
                                        });

                                        //---------------------------------------------------------------
                                        
                                        
                                        tmp1.Add(sKeyA, new PEDCOMPRACAB_KEY_LIN
                                        {
                                            CODPROVEEDOR = cprooveedorA.CODPROVEEDOR,
                                            idCab        = nid,
                                            pcc          = pcc,
                                            npedido      = id,
                                            lst1 = new List<PEDCOMPRACAB_PEDIDOS_LIN>
                                            {
                                                new()
                                                {
                                                    CODIGO      = iap.CODIGO_A,
                                                    PORCENTAJE  = iap.PORCENTAJE_A,
                                                    PRECIO      = iap.PRECIO_A,
                                                    PEDIDOS_LIN = it2,
                                                    CODALMACEN  = it.CODALMACEN,
                                                }
                                            }
                                        });
                                    }
                                    else
                                    {
                                        tmp1[sKeyA].lst1.Add(new()
                                        {
                                            CODIGO      = iap.CODIGO_A,
                                            PORCENTAJE  = iap.PORCENTAJE_A,
                                            PRECIO      = iap.PRECIO_A,
                                            PEDIDOS_LIN = it2,
                                            CODALMACEN  = it.CODALMACEN
                                        });
                                    }

                                    continue;
                                }

                                if (iap.PORCENTAJE_A == 0)
                                {

                                    if (iap.PORCENTAJE_B != null)
                                    {
                                        //******************************************************************//
                                        if (string.IsNullOrEmpty(iap.PROVEEDOR_B) || iap.PROVEEDOR_B.ToUpper() == "NA")
                                        {


                                            continue;
                                        }

                                        if (iap.PORCENTAJE_B == 0)
                                        {

                                            continue;
                                        }


                                        continue;
                                        //******************************************************************//
                                    }

                                    if (iap.PORCENTAJE_B == null)
                                    {
                                        //******************************************************************//



                                        continue;
                                        //******************************************************************//
                                    }


                                    continue;
                                }

                                if (iap.PORCENTAJE_A > 0 && iap.PORCENTAJE_A < 1)
                                {

                                    if (iap.PORCENTAJE_B != null)
                                    {
                                        //******************************************************************//
                                        if (string.IsNullOrEmpty(iap.PROVEEDOR_B) || iap.PROVEEDOR_B.ToUpper() == "NA")
                                        {


                                            continue;
                                        }

                                        if (iap.PORCENTAJE_B == 0)
                                        {

                                            continue;
                                        }

                                        //---------------------------------------------------------------------------------//
                                        var cprooveedorA = db.PROVEEDORES.FirstOrDefault(f => f.NIF20 == iap.PROVEEDOR_A);

                                        if (cprooveedorA == null)
                                        {

                                            continue;
                                        }
                                        //---------------------------------------------------------------------------------//
                                        var cprooveedorB = db.PROVEEDORES.FirstOrDefault(f => f.NIF20 == iap.PROVEEDOR_B);

                                        if (cprooveedorB == null)
                                        {

                                            continue;
                                        }
                                        //---------------------------------------------------------------------------------//

                                        var sKeyA = new PEDCOMPRACAB_KEY
                                        {
                                            CODALMACEN = it.CODALMACEN,
                                            PROVEEDOR  = iap.PROVEEDOR_A
                                        };

                                        it2.CODALMACEN = it.CODALMACEN;

                                        if (!tmp1.ContainsKey(sKeyA))
                                        {

                                            if (npedido.ContainsKey(it.CODALMACEN + "1P"))
                                            {
                                                npedido[it.CODALMACEN + "1P"]++;
                                            }
                                            else
                                            {
                                                npedido.Add(it.CODALMACEN + "1P", 1);
                                            }

                                            var id = npedido[it.CODALMACEN + "1P"];

                                            var pcc = GetPedCompraCab(it, id, cprooveedorA.CODPROVEEDOR, f1);

                                            //---------------------------------------------------------------
                                            //var ipc = db.IT_PEDIDOS_CAB.FirstOrDefault(f => f.ID == it.ID);

                                            //var sKey = new IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(it.ID); //, pcc.NUMSERIE,it.CODALMACEN

                                            //if (nid.ContainsKey(sKey))
                                            //{
                                            //    nid[sKey]++;
                                            //}
                                            //else
                                            //{
                                            //    nid.Add(sKey, 1);
                                            //}

                                            //var sid = nid[sKey];

                                            nid++;


                                            db.IT_PEDIDOS_NUMPEDIDOS_CAB.Add(new IT_PEDIDOS_NUMPEDIDOS_CAB
                                            {
                                                ID          = nid,
                                                NUMPEDIDO   = pcc.NUMPEDIDO,
                                                CODALMACEN  = it.CODALMACEN,
                                                NUMSERIE    = pcc.NUMSERIE,
                                                FECHAPEDIDO = pcc.FECHAPEDIDO,
                                                NUMDPTO     = it.NUMDPTO,
                                                CODVENDEDOR = Extensions.GetCodVendedor(),
                                                ID_ORDEN    = idOrden
                                            });
                                            //---------------------------------------------------------------


                                            tmp1.Add(sKeyA, new PEDCOMPRACAB_KEY_LIN
                                            {
                                                CODPROVEEDOR = cprooveedorA.CODPROVEEDOR,
                                                idCab        = nid,
                                                pcc          = pcc,
                                                npedido      = id,
                                                lst1 = new List<PEDCOMPRACAB_PEDIDOS_LIN>
                                                {
                                                    new()
                                                    {
                                                        CODIGO      = iap.CODIGO_A,
                                                        PORCENTAJE  = iap.PORCENTAJE_A,
                                                        PRECIO      = iap.PRECIO_A,
                                                        PEDIDOS_LIN = it2,
                                                        CODALMACEN  = it.CODALMACEN
                                                    }
                                                }

                                            });
                                        }
                                        else
                                        {
                                            tmp1[sKeyA].lst1.Add(new()
                                            {
                                                CODIGO      = iap.CODIGO_A,
                                                PORCENTAJE  = iap.PORCENTAJE_A,
                                                PRECIO      = iap.PRECIO_A,
                                                PEDIDOS_LIN = it2,
                                                CODALMACEN  = it.CODALMACEN
                                            });
                                        }

                                        //---------------------------------------------------------------------------------//

                                        var sKeyB = new PEDCOMPRACAB_KEY
                                        {
                                            CODALMACEN = it.CODALMACEN,
                                            PROVEEDOR  = iap.PROVEEDOR_B
                                        };

                                        if (!tmp1.ContainsKey(sKeyB))
                                        {
                                            if (npedido.ContainsKey(it.CODALMACEN + "1P"))
                                            {
                                                npedido[it.CODALMACEN + "1P"]++;
                                            }
                                            else
                                            {
                                                npedido.Add(it.CODALMACEN + "1P", 1);
                                            }

                                            var id  = npedido[it.CODALMACEN + "1P"];

                                            var pcc = GetPedCompraCab(it, id, cprooveedorB.CODPROVEEDOR, f1);

                                            //---------------------------------------------------------------
                                            //var ipc = db.IT_PEDIDOS_CAB.FirstOrDefault(f => f.ID == it.ID);

                                            //var sKey = new IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(it.ID); //, pcc.NUMSERIE,it.CODALMACEN

                                            //if (nid.ContainsKey(sKey))
                                            //{
                                            //    nid[sKey]++;
                                            //}
                                            //else
                                            //{
                                            //    nid.Add(sKey, 1);
                                            //}

                                            //var sid = nid[sKey];

                                            nid++;

                                            db.IT_PEDIDOS_NUMPEDIDOS_CAB.Add(new IT_PEDIDOS_NUMPEDIDOS_CAB
                                            {
                                                ID          = nid,
                                                NUMPEDIDO   = pcc.NUMPEDIDO,
                                                CODALMACEN  = it.CODALMACEN,
                                                NUMSERIE    = pcc.NUMSERIE,
                                                FECHAPEDIDO = pcc.FECHAPEDIDO,
                                                NUMDPTO     = it.NUMDPTO,
                                                CODVENDEDOR = Extensions.GetCodVendedor(),
                                                ID_ORDEN    = idOrden
                                            });
                                            
                                            //---------------------------------------------------------------

                                            tmp1.Add(sKeyB, new PEDCOMPRACAB_KEY_LIN
                                            {
                                                CODPROVEEDOR = cprooveedorB.CODPROVEEDOR,
                                                idCab        = nid,
                                                pcc          = pcc,
                                                npedido      = id,
                                                lst1 = new List<PEDCOMPRACAB_PEDIDOS_LIN>
                                                {
                                                    new()
                                                    {
                                                        CODIGO      = iap.CODIGO_B,
                                                        PORCENTAJE  = iap.PORCENTAJE_B,
                                                        PRECIO      = iap.PRECIO_B,
                                                        PEDIDOS_LIN = it2,
                                                        CODALMACEN  = it.CODALMACEN
                                                    }
                                                }
                                            });
                                        }
                                        else
                                        {
                                            tmp1[sKeyB].lst1.Add(new()
                                            {
                                                CODIGO      = iap.CODIGO_B,
                                                PORCENTAJE  = iap.PORCENTAJE_B,
                                                PRECIO      = iap.PRECIO_B,
                                                PEDIDOS_LIN = it2,
                                                CODALMACEN  = it.CODALMACEN
                                            });
                                        }
                                        //---------------------------------------------------------------------------------//

                                        continue;
                                        //******************************************************************//
                                    }

                                    if (iap.PORCENTAJE_B == null)
                                    {
                                        //******************************************************************//



                                        continue;
                                        //******************************************************************//
                                    }


                                    continue;
                                }


                                continue;
                                //******************************************************************//
                            }



                            if (iap.PORCENTAJE_A == null)
                            {
                                //******************************************************************//



                                continue;
                                //******************************************************************//
                            }


                        }


                    }

                }




                //**********************************************************************
                foreach (var kv in tmp1)
                {
                    var npedido2 = kv.Value.npedido;
                    var lin      = 1;
                    var lin2     = 1;

                    double? totBruto = 0;

                    

                    foreach (var ppl in kv.Value.lst1)
                    {

                            var codart     = ppl.PEDIDOS_LIN.CODARTICULO;
                            var refp       = ppl.PEDIDOS_LIN.REFERENCIA;
                            var cantidad   = ppl.PEDIDOS_LIN.CANTIDAD;
                            var porcentaje = ppl.PORCENTAJE;
                            var precio     = ppl.PRECIO;


                        //SELECT IMPUESTOCOMPRA, CODARTICULO ,* FROM ARTICULOS WHERE REFPROVEEDOR = 'REF001-M'

                        var art = db.ARTICULOS.FirstOrDefault(f => f.CODARTICULO == codart);
                        var imp = db.IMPUESTOS.FirstOrDefault(f => f.TIPOIVA == art.TIPOIMPUESTO);

                        totBruto += cantidad * porcentaje * precio;

                        double UNID2 = 1;
                        double UNID4 = 1;

                        if (ppl.PEDIDOS_LIN.CODFORMATO == 0)
                        {
                            UNID2 = cantidad.Value * porcentaje.Value;
                        }

                        if (ppl.PEDIDOS_LIN.CODFORMATO != 0)
                        {
                            UNID4 = cantidad.Value * porcentaje.Value;
                        }


                        

                        var pcl = new PEDCOMPRALIN
                        {
                            NUMSERIE      = ppl.CODALMACEN + "1P",
                            NUMPEDIDO     = npedido2,
                            N             = "B",
                            NUMLINEA      = lin,
                            CODARTICULO   = art.CODARTICULO,
                            REFERENCIA    = art.REFPROVEEDOR,
                            TALLA         = ".", //FIJO
                            COLOR         = ".", //FIJO
                            DESCRIPCION   = art.DESCRIPCION,
                            UNID1         = 1, //FIJO
                            UNID2         = UNID2, //cantidad * porcentaje FIJO 1 cuando codformato = 0
                            UNID3         = 1, //FIJO
                            UNID4         = UNID4, //1 FIJO cuando venga codformato
                            UNIDADESTOTAL = cantidad * porcentaje,
                            UNIDADESREC   = 0, //FIJO
                            UNIDADESPEN   = cantidad * porcentaje,
                            PRECIO        = precio,
                            DTO           = 0, //FIJO
                            TIPOIMPUESTO  = (short?)art.TIPOIMPUESTO,
                            IVA           = imp.IVA,
                            REQ           = imp.REQ,
                            TOTALLINEA    = cantidad * porcentaje * precio,
                            CODALMACEN    = ppl.CODALMACEN,
                            DEPOSITO      = "F", //FIJO
                            PRECIOVENTA   = 0, //FIJO
                            NUMKG         = 0, //FIJO
                            SUPEDIDO      = "-" + ppl.CODALMACEN + "1P-" + npedido2, //FIJO
                            CODCLIENTE    = -1, //FIJO
                            CARGO1        = 0, //FIJO
                            CARGO2        = 0, //FIJO
                            DTOTEXTO      = "0%", //FIJO
                            ESOFERTA      = "F", //FIJO
                            FECHAENTREGA  = new DateTime(1899,12,30,0,0,0), //FIJO
                            CODENVIO      = -1, //FIJO
                            UDMEDIDA2     = 0, //FIJO
                            LINEAOCULTA   = "F", //FIJO
                            CODFORMATO    = ppl.PEDIDOS_LIN.CODFORMATO, //FIJO
                        };
                        //30/12/1899 12:00:00.000 a. m.
                        db.PEDCOMPRALIN.Add(pcl);



                        db.IT_PEDIDOS_NUMPEDIDOS_LIN.Add(new IT_PEDIDOS_NUMPEDIDOS_LIN
                        {
                            ID_CAB       = kv.Value.idCab,
                            NUMPEDIDO    = pcl.NUMPEDIDO,
                            NUMLINEA     = pcl.NUMLINEA,
                            CODALMACEN   = pcl.CODALMACEN,
                            CODARTICULO  = pcl.CODARTICULO,
                            REFERENCIA   = pcl.REFERENCIA,
                            REFERENCIA2  = refp,
                            CODPROVEEDOR = kv.Value.CODPROVEEDOR,
                            PORCENTAJE   = ppl.PORCENTAJE,
                            CANTIDAD     = ppl.PEDIDOS_LIN.CANTIDAD,
                            SUGERIDO     = ppl.PEDIDOS_LIN.SUGERIDO,
                            SEMAFORO     = ppl.PEDIDOS_LIN.SEMAFORO,
                            NUMSERIE     = pcl.NUMSERIE,
                            CODIGO       = ppl.CODIGO,
                            CODFORMATO   = ppl.PEDIDOS_LIN.CODFORMATO,
                            TIPOIMPUESTO = pcl.TIPOIMPUESTO,
                            IVA          = pcl.IVA,
                            REQ          = pcl.REQ,
                            PRECIO       = pcl.PRECIO
                        });
                        
                        lin++;

                        //**********************************************************************************************

                        /*
                        var sKey = new PEDCOMPRATOT_KEY()
                        {
                            NUMSERIE = pcl.NUMSERIE,
                            NUMERO   = pcl.NUMPEDIDO
                        };*/

                        var sKeyIMP = new PEDCOMPRATOT_IMP_KEY()
                        {
                            NUMSERIE = pcl.NUMSERIE,
                            NUMERO   = pcl.NUMPEDIDO,
                            TIPOIVA  = pcl.TIPOIMPUESTO
                        };

                        if (!tmp2.ContainsKey(sKeyIMP))
                        {
                            /*
                            if (nnumero.ContainsKey(sKey))
                            {
                                nnumero[sKey]++;
                            }
                            else
                            {
                                nnumero.Add(sKey, 1);
                            }

                            var id = nnumero[sKey];
                            */


                            tmp2.Add(sKeyIMP, new PEDCOMPRATOT()
                            {
                                SERIE         = ppl.CODALMACEN + "1P",
                                NUMERO        = npedido2,
                                N             = "B", //FIJO
                                NUMLINEA      = lin2, //id
                                BRUTO         = pcl.TOTALLINEA,
                                DTOCOMERC     = 0, //FIJO
                                TOTDTOCOMERC  = 0, //FIJO
                                DTOPP         = 0, //FIJO
                                TOTDTOPP      = 0, //FIJO
                                BASEIMPONIBLE = pcl.TOTALLINEA,
                                IVA           = pcl.IVA,
                                TOTIVA        = null,
                                REQ           = pcl.REQ,
                                TOTREQ        = null,
                                TOTAL         = null,
                                ESGASTO       = "F", //FIJO
                                CODDTO        = -1, //FIJO
                                DESCRIPCION   = "", //FIJO
                                TIPOIVA       = pcl.TIPOIMPUESTO,
                            });

                            lin2++;
                        }
                        else
                        {
                            tmp2[sKeyIMP].BRUTO         += pcl.TOTALLINEA;
                            tmp2[sKeyIMP].BASEIMPONIBLE += pcl.TOTALLINEA;
                        }



                        //**********************************************************************************************

                    }

                    var pcc = tmp1[kv.Key].pcc;
                    

                    if (pcc.NUMPEDIDO== kv.Value.npedido &&
                        pcc.NUMSERIE == kv.Key.CODALMACEN + "1P")
                    {
                        pcc.TOTBRUTO = totBruto;
                    }


                }


                foreach (var kv in tmp2)
                {
                    
                    kv.Value.TOTREQ = kv.Value.BASEIMPONIBLE * (kv.Value.REQ / 100);

                    kv.Value.TOTIVA = (kv.Value.BASEIMPONIBLE + kv.Value.TOTREQ) * (kv.Value.IVA / 100);

                    kv.Value.TOTAL = kv.Value.BASEIMPONIBLE + kv.Value.TOTIVA + kv.Value.TOTREQ;


                    var tpcc=tmp1.Select(s => s.Value).ToList()
                        .Select(s => s.pcc).ToList().FirstOrDefault(f => f.NUMSERIE == kv.Value.SERIE && f.NUMPEDIDO== kv.Value.NUMERO);

                    if (tpcc.TOTIMPUESTOS == null)
                    {
                        tpcc.TOTIMPUESTOS = 0;
                    }

                    tpcc.TOTIMPUESTOS += kv.Value.TOTIVA + kv.Value.TOTREQ;


                    db.PEDCOMPRATOT.Add(kv.Value);
                }


                foreach (var kv in tmp1)
                {
                    kv.Value.pcc.TOTNETO = kv.Value.pcc.TOTBRUTO + kv.Value.pcc.TOTIMPUESTOS;

                    db.PEDCOMPRACAB.Add(kv.Value.pcc);
                }

                //**********************************************************************
                foreach (var it in req.cab_pedidos)
                {
                    var cab=db.IT_PEDIDOS_CAB.FirstOrDefault(f => f.ID == it.ID);
                    cab.ESTATUS = 2;

                }
                //**********************************************************************




                    db.SaveChanges();
                tran.Commit();
                //tran.Rollback();
            }
            catch (Exception e)
            {

                tran.Rollback();
                throw;
            }
        }

        private static PEDCOMPRACAB GetPedCompraCab(IT_PEDIDOS_CAB2Result it, int npedido, int codproveedor, DateTime f1)
        {
            var pcc = new PEDCOMPRACAB
            {
                NUMSERIE            = it.CODALMACEN + "1P",
                NUMPEDIDO           = npedido,
                N                   = "B", //FIJO
                CODPROVEEDOR        = codproveedor,
                SERIEALBARAN        = "",  //FIJO
                NUMEROALBARAN       = -1,  //FIJO
                NALBARAN            = "B", //FIJO
                FECHAPEDIDO         = f1,
                FECHAENTREGA        = f1,
                ENVIOPOR            = "", //FIJO
                TOTBRUTO            = 0,
                DTOPP               = 0, //FIJO
                TOTDTOPP            = 0, //FIJO
                DTOCOMERCIAL        = 0, //FIJO
                TOTDTOCOMERCIAL     = 0, //FIJO
                TOTIMPUESTOS        = null,
                TOTNETO             = null,
                CODMONEDA           = 1, //FIJO
                FACTORMONEDA        = 1, //FIJO
                PORTESPAG           = "F", //FIJO
                SUPEDIDO            = "-" + it.CODALMACEN + "1P-" + npedido,
                IVAINCLUIDO         = "F", //FIJO
                TODORECIBIDO        = "F", //FIJO
                TIPODOC             = 2, //FIJO
                IDESTADO            = 0, //Default - Por Autorizar
                FECHAMODIFICADO     = f1,
                HORA                = f1,
                TRANSPORTE          = 0, //FIJO
                NBULTOS             = 0, //FIJO
                TOTALCARGOSDTOS     = 0, //FIJO
                NORECIBIDO          = "T", //FIJO
                CODEMPLEADO         = -1, //FIJO
                CONTACTO            = -1, //FIJO
                FROMPEDVENTACENTRAL = "F", //FIJO
                FECHACREACION       = f1,
                NUMIMPRESIONES      = null, //FIJO
                REGIMFACT           = "G" //FIJO
            };
            return pcc;
        }
    }

    public class PEDCOMPRACAB_PEDIDOS_LIN
    {
        public double?        PORCENTAJE  { get; set; }
        public IT_PEDIDOS_LIN PEDIDOS_LIN { get; set; }
        public string         CODALMACEN  { get; set; }
        public string         CODIGO      { get; set; }
        public double?        PRECIO      { get; set; }
    }
}
