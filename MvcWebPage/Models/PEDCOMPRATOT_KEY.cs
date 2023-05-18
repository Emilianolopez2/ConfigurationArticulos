namespace MvcWebPage.Models
{
    struct PEDCOMPRATOT_KEY
    {
        public string NUMSERIE { get; set; }
        public int NUMERO { get; set; }

        public PEDCOMPRATOT_KEY()
        {

        }

        public PEDCOMPRATOT_KEY(string numserie, int numero)
        {
            NUMSERIE = numserie;
            NUMERO = numero;
        }
    }
}
