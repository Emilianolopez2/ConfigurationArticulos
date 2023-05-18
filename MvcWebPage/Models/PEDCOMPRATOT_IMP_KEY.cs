namespace MvcWebPage.Models
{
    struct PEDCOMPRATOT_IMP_KEY
    {
        public string NUMSERIE { get; set; }
        public int NUMERO { get; set; }
        public short? TIPOIVA { get; set; }

        public PEDCOMPRATOT_IMP_KEY()
        {

        }

        public PEDCOMPRATOT_IMP_KEY(string numserie, int numero, short? tipoiva)
        {
            NUMSERIE = numserie;
            NUMERO   = numero;
            TIPOIVA  = tipoiva;
        }
    }
}
