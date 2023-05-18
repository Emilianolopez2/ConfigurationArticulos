using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;

namespace MvcWebPage.Services
{
    public class MPedidosService
    {

        public static void ProcesarPedidos(Request req, ClaimsPrincipal User)
        {

            MLAVIDContext db  = new MLAVID_DB();
            var tran = db.Database.BeginTransaction();

            try
            {


                var tmp1    = new Dictionary<PEDCOMPRACAB_KEY, PEDCOMPRACAB_KEY_LIN>();
                var tmp2    = new Dictionary<PEDCOMPRATOT_IMP_KEY, PEDCOMPRATOT>();



                //**********************************************************************

                db.PEDCOMPRATOT.Where(f =>
                    f.NUMERO == req.pedido_aut.NUMPEDIDO && f.SERIE == req.pedido_aut.NUMSERIE).ExecuteDelete();

                db.PEDCOMPRALIN.Where(f =>
                    f.NUMPEDIDO == req.pedido_aut.NUMPEDIDO && f.NUMSERIE == req.pedido_aut.NUMSERIE).ExecuteDelete();

                    //var npedido2 = kv.Value.npedido;
                    var lin      = 1;
                    var lin2     = 1;

                    double? totBruto = 0;

                    foreach (var it in req.pedido_det)
                    {
                        var codart     = it.CODARTICULO;
                        var refp       = it.REFERENCIA;
                        var cantidad   = it.CANTIDAD;
                        var porcentaje = it.PORCENTAJE;
                        var precio     = it.PRECIO;

                        

                        //SELECT IMPUESTOCOMPRA, CODARTICULO ,* FROM ARTICULOS WHERE REFPROVEEDOR = 'REF001-M'

                        //var art = db.ARTICULOS.FirstOrDefault(f => f.CODARTICULO == codart);
                        //var imp = db.IMPUESTOS.FirstOrDefault(f => f.TIPOIVA == art.TIPOIMPUESTO);

                        totBruto += cantidad * porcentaje * precio;

                        double UNID2 = 1;
                        double UNID4 = 1;

                        if (it.CODFORMATO == 0)
                        {
                            UNID2 = cantidad.Value * porcentaje.Value;
                        }

                        if (it.CODFORMATO != 0)
                        {
                            UNID4 = cantidad.Value * porcentaje.Value;
                        }


                        

                        var pcl = new PEDCOMPRALIN
                        {
                            NUMSERIE      = req.pedido_aut.NUMSERIE,
                            NUMPEDIDO     = req.pedido_aut.NUMPEDIDO,
                            N             = "B",
                            NUMLINEA      = lin,
                            CODARTICULO   = it.CODARTICULO,
                            REFERENCIA    = it.REFERENCIA,
                            TALLA         = ".", //FIJO
                            COLOR         = ".", //FIJO
                            DESCRIPCION   = it.DESCRIPCION,
                            UNID1         = 1, //FIJO
                            UNID2         = UNID2, //cantidad * porcentaje FIJO 1 cuando codformato = 0
                            UNID3         = 1, //FIJO
                            UNID4         = UNID4, //1 FIJO cuando venga codformato
                            UNIDADESTOTAL = cantidad * porcentaje,
                            UNIDADESREC   = 0, //FIJO
                            UNIDADESPEN   = cantidad * porcentaje,
                            PRECIO        = precio,
                            DTO           = 0, //FIJO
                            TIPOIMPUESTO  = it.TIPOIMPUESTO,
                            IVA           = it.IVA,
                            REQ           = it.REQ,
                            TOTALLINEA    = cantidad * porcentaje * precio,
                            CODALMACEN    = req.pedido_aut.CODALMACEN,
                            DEPOSITO      = "F", //FIJO
                            PRECIOVENTA   = 0, //FIJO
                            NUMKG         = 0, //FIJO
                            SUPEDIDO      = "-" + req.pedido_aut.CODALMACEN + "1P-" + req.pedido_aut.NUMPEDIDO, //FIJO
                            CODCLIENTE    = -1, //FIJO
                            CARGO1        = 0, //FIJO
                            CARGO2        = 0, //FIJO
                            DTOTEXTO      = "0%", //FIJO
                            ESOFERTA      = "F", //FIJO
                            FECHAENTREGA  = new DateTime(1899, 12, 30, 0, 0, 0), //FIJO
                            CODENVIO      = -1, //FIJO
                            UDMEDIDA2     = 0, //FIJO
                            LINEAOCULTA   = "F", //FIJO
                            CODFORMATO    = it.CODFORMATO, //FIJO
                        };
                        //30/12/1899 12:00:00.000 a. m.
                        db.PEDCOMPRALIN.Add(pcl);


                        lin++;

                        //**********************************************************************************************
                        /*
                        var sKey = new PEDCOMPRATOT_KEY()
                        {
                            NUMSERIE = pcl.NUMSERIE,
                            NUMERO = pcl.NUMPEDIDO
                        };
                        */

                        var sKeyIMP = new PEDCOMPRATOT_IMP_KEY()
                        {
                            NUMSERIE = pcl.NUMSERIE,
                            NUMERO = pcl.NUMPEDIDO,
                            TIPOIVA = pcl.TIPOIMPUESTO
                        };

                        if (!tmp2.ContainsKey(sKeyIMP))
                        {

                            tmp2.Add(sKeyIMP, new PEDCOMPRATOT()
                            {
                                SERIE         = req.pedido_aut.NUMSERIE,
                                NUMERO        = req.pedido_aut.NUMPEDIDO,
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
                            tmp2[sKeyIMP].BRUTO += pcl.TOTALLINEA;
                            tmp2[sKeyIMP].BASEIMPONIBLE += pcl.TOTALLINEA;
                        }



                        //**********************************************************************************************
                    }

                    /*
                    var pcc = tmp1[kv.Key].pcc;
                    

                    if (pcc.NUMPEDIDO== kv.Value.npedido &&
                        pcc.NUMSERIE == kv.Key.CODALMACEN + "1P")
                    {
                        pcc.TOTBRUTO = totBruto;
                    }
                    */

                    var pcc= db.PEDCOMPRACAB.FirstOrDefault(f =>
                        f.NUMPEDIDO == req.pedido_aut.NUMPEDIDO && f.NUMSERIE == req.pedido_aut.NUMSERIE);

                    pcc.TOTBRUTO = totBruto;
                    pcc.TOTIMPUESTOS = 0;


                foreach (var kv in tmp2)
                {
                    
                    kv.Value.TOTREQ = kv.Value.BASEIMPONIBLE * (kv.Value.REQ / 100);

                    kv.Value.TOTIVA = (kv.Value.BASEIMPONIBLE + kv.Value.TOTREQ) * (kv.Value.IVA / 100);

                    kv.Value.TOTAL = kv.Value.BASEIMPONIBLE + kv.Value.TOTIVA + kv.Value.TOTREQ;

                    /*
                    var tpcc=tmp1.Select(s => s.Value).ToList()
                        .Select(s => s.pcc).ToList().FirstOrDefault(f => f.NUMSERIE == kv.Value.SERIE && f.NUMPEDIDO== kv.Value.NUMERO);
                    */

                    if (pcc.TOTIMPUESTOS == null)
                    {
                        pcc.TOTIMPUESTOS = 0;
                    }

                    pcc.TOTIMPUESTOS += kv.Value.TOTIVA + kv.Value.TOTREQ;


                    db.PEDCOMPRATOT.Add(kv.Value);
                }


                pcc.TOTNETO = pcc.TOTBRUTO + pcc.TOTIMPUESTOS;

                /*
                foreach (var kv in tmp1)
                {
                    //kv.Value.pcc.TOTNETO = kv.Value.pcc.TOTBRUTO + kv.Value.pcc.TOTIMPUESTOS;

                    //db.PEDCOMPRACAB.Add(kv.Value.pcc);
                }*/

                //**********************************************************************

                /*
                foreach (var it in req.cab_pedidos)
                {
                    var cab=db.IT_PEDIDOS_CAB.FirstOrDefault(f => f.ID == it.ID);
                    cab.ESTATUS = 2;

                }*/
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


}
