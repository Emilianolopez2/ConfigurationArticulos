namespace MvcWebPage.Models
{
    struct IT_PEDIDOS_NUMPEDIDOS_CAB_KEY
    {
        public int ID { get; set; }
        public string NUMSERIE { get; set; }
        public string CODALMACEN { get; set; }

        public IT_PEDIDOS_NUMPEDIDOS_CAB_KEY()
        {

        }

        //public IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(int id, string numserie, string codalmacen)
        //{
        //    ID         = id;
        //    NUMSERIE   = numserie;
        //    CODALMACEN = codalmacen;
        //}

        public IT_PEDIDOS_NUMPEDIDOS_CAB_KEY(int id)
        {
            ID         = id;
        }
    }
}
