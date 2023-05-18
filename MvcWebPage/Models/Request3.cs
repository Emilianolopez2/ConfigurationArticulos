using MvcWebPage.MLAVID;

namespace MvcWebPage.Models
{
    public class Request3
    {
        public string referencia   { get; set; }
        public string dato   { get; set; }
        public string refart { get; set; }

        public string descripcion { get; set; }
        public string sucursal { get; set; }
        public List <IT_ART_PROVResult> datosg { get; set; }

    }
}
