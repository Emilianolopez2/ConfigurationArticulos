using MvcWebPage.MLAVID;

namespace MvcWebPage.Models
{
    public class Request
    {
        public int?                           estatus      { get; set; }
        public int                            act          { get; set; }
        public bool                           todos        { get; set; }
        public bool                           excluir      { get; set; }
        public int                            id           { get; set; }
        public int                            codproveedor { get; set; }
        public string                         numserie     { get; set; }
        public int                            numpedido    { get; set; }
        public int                            tipoUsuario  { get; set; }
        public string                         usuario      { get; set; }
        public string                         proveedor    { get; set; }
        public string                         fecha        { get; set; }
        public string                         fecha1       { get; set; }
        public string                         fecha2       { get; set; }
        public short                          numdpto      { get; set; }
        public string                         sucursal     { get; set; }
        public List<IT_PEDIDOS_AUT_DETResult> pedido_det   { get; set; }
        public IT_PEDIDOS_AUTResult           pedido_aut   { get; set; }
        public List<IT_PEDIDOSResult>         pedidos      { get; set; }
        public List<IT_PEDIDOS_CAB2Result>    cab_pedidos  { get; set; }

        //public IEnumerable<IT_PEDIDOSResult> pedidos { get; set; }
    }
}
