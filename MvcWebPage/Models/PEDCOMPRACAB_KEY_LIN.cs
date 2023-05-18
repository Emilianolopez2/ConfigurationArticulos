using MvcWebPage.MLAVID;
using MvcWebPage.Services;

namespace MvcWebPage.Models
{
    public class PEDCOMPRACAB_KEY_LIN
    {

        public PEDCOMPRACAB pcc { get; set; }
        public List<PEDCOMPRACAB_PEDIDOS_LIN> lst1 { get; set; }
        public int npedido { get; set; }
        public int idCab { get; set; }
        //public int? numdpto { get; set; }
        public int CODPROVEEDOR { get; set; }
    }
}
