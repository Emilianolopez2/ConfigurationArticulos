using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvcWebPage.Xlsx
{
    // XlsxRow<int> - represents mappings between column names and column numbers.
    // XlsxRow<string> - represents one parsed row from xlsx file
    /*public class XlsxRow1<T>
    {
        [XlsxColumn(XlsxColumnNames1.NUMDPTO)]
        public T NUMDPTO{ get; set; }

        [XlsxColumn(XlsxColumnNames1.DESCRIPCION)]
        public T DESCRIPCION { get; set; }   
    }*/

    public class IT_LAYOUT<T>
    {
        [XlsxColumn(Name = "REFPROVEEDOR")]
        public T REFPROVEEDOR { get; set; }

        [XlsxColumn(Name = "DOSIS")]
        public T DOSIS { get; set; }

        [XlsxColumn(Name = "DESCRIPCION")]
        public T DESCRIPCION { get; set; }

        [XlsxColumn(Name = "UNIDADM")]
        public T UNIDADM { get; set; }

        [XlsxColumn(Name = "UNIDADMDES")]
        public T UNIDADMDES { get; set; }

        [XlsxColumn(Name = "SUCURSAL")]
        public T SUCURSAL { get; set; }

        [XlsxColumn(Name = "PROVEEDOR_A")]
        public T PROVEEDOR_A { get; set; }

        [XlsxColumn(Name = "PORCENTAJE_A")]
        public T PORCENTAJE_A { get; set; }

        [XlsxColumn(Name = "PRECIO_A")]
        public T PRECIO_A { get; set; }

        [XlsxColumn(Name = "CODIGO_A")]
        public T CODIGO_A { get; set; }

        [XlsxColumn(Name = "PROVEEDOR_B")]
        public T PROVEEDOR_B { get; set; }

        [XlsxColumn(Name = "PORCENTAJE_B")]
        public T PORCENTAJE_B { get; set; }

        [XlsxColumn(Name = "PRECIO_B")]
        public T PRECIO_B { get; set; }

        [XlsxColumn(Name = "CODIGO_B")]
        public T CODIGO_B { get; set; }

        [XlsxColumn(Name = "PROVEEDOR_C")]
        public T PROVEEDOR_C { get; set; }

        [XlsxColumn(Name = "PORCENTAJE_C")]
        public T PORCENTAJE_C { get; set; }

        [XlsxColumn(Name = "PRECIO_C")]
        public T PRECIO_C { get; set; }

        [XlsxColumn(Name = "CODIGO_C")]
        public T CODIGO_C { get; set; }

        [XlsxColumn(Name = "LEADTIME")]
        public T LEADTIME { get; set; }

        [XlsxColumn(Name = "STOCK_SEGURIDAD")]
        public T STOCK_SEGURIDAD { get; set; }

        [XlsxColumn(Name = "STOCK_MAXIMO")]
        public T STOCK_MAXIMO { get; set; }

        [XlsxColumn(Name = "FRECUENCIA")]
        public T FRECUENCIA { get; set; }




    }

}