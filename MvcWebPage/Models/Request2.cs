using MvcWebPage.MLAVID;
using System.ComponentModel.DataAnnotations;

namespace MvcWebPage.Models
{
    public class Request2
    {
        public bool editar { get; set; }
        public bool excluir { get; set; }
        public int id { get; set; }
        public int tipoUsuario { get; set; }
        public string usuario { get; set; }
        public string fecha { get; set; }
        public short numdpto { get; set; }
        public string sucursal { get; set; }

        //public List<IT_DETALLESINVResult> details { get; set; }
        //public List<IT_SELECCIONAR_ARTICULOSResult> ART_RES { get; set; }

        public List<IT_SELECCIONAR_ARTICULOSResult> data { get; set; }
        public List<IT_SELECT_MODResult> data2 { get; set; }

        //public List<IT_PEDIDOS_CAB2Result> cab_pedidos { get; set; }


        public MOVIMENTS mOVIMENTS { get; set; }

        public ALMACEN a { get; set; }
        public IT_INV_MENSUALES_CAB GridCab { get; set; }
    }
}
