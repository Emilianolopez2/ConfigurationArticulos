using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ClosedXML.Excel;
using MvcWebPage.Models;


namespace MvcWebPage.Xlsx
{
     static class ArchivoExcel
    {


        public static List<IT_LAYOUT<string>> GetExcel(IFormFile cfile)
        {

            MemoryStream file = new MemoryStream();
            cfile.CopyTo(file);


            XLWorkbook workbook = null;

            //using (var stream = new MemoryStream())
            //{
            //    Aspose.Cells.License license = new Aspose.Cells.License();
            //    license.SetLicense("Aspose.Cells.lic");

            //    var workbook2 = new Aspose.Cells.Workbook(file);

            //    //var lic=workbook2.IsLicensed;

            //    workbook2.Save(stream, SaveFormat.Xlsx);
            //    workbook = new XLWorkbook(stream);
            //}
            workbook = new XLWorkbook(file);

            var result = new List<IT_LAYOUT<string>>();

            var ws = workbook.Worksheets.Worksheet(1);

            if (ws!=null)
            {



                var header = ws.FirstRowUsed();

                var LR = ws.LastRowUsed();

                if (LR == null || header == null)
                {
                    //continue;

                    return result;
                }

                int rowLast = ws.LastRowUsed().RowNumber();

                IT_LAYOUT<int> columnMap=null;
                List<string> cols = null;

                for (int i = 0; i <= rowLast; i++)
                {
                    columnMap = header.GetColumnMappings<IT_LAYOUT<int>>();
                    cols = columnMap.IsValid();


                    if (cols.Count > 0)
                    {

                        header = header.RowBelow();

                        //throw new Exception("Archivo no valido");
                        //MessageBox.Show("No se encuentra(n) la(s) columna(s) :\n\n" + string.Join(", ", cols));

                        //continue;
                    }
                    else
                    {
                        break;
                    }
                }



                if (cols ==null || cols.Count > 0)
                {
                    //continue;
                    return result;
                }


                var dataRow = header.RowBelow();

                //var hoja=ws.Name.ToLower().Replace(" ", "").QuitaAcentos();


                

                while (dataRow.RowNumber() <= rowLast)
                {
                    var row = dataRow.ConvertRow<IT_LAYOUT<string>>(columnMap);


                    {
                        row.REFPROVEEDOR = row.REFPROVEEDOR.Trim();
                        
                        row.DOSIS        = row.DOSIS.Replace("'", "").Trim();
                        row.DOSIS        = Math.Round(row.DOSIS.Dbl(), 4).Str();

                        row.DESCRIPCION = row.DESCRIPCION.Trim();
                        row.UNIDADM     = row.UNIDADM.Trim();
                        row.UNIDADMDES  = row.UNIDADMDES.Trim();
                        row.SUCURSAL    = row.SUCURSAL.Trim();

                        /*********************************************************/
                        row.PROVEEDOR_A = row.PROVEEDOR_A.Trim();

                        row.PORCENTAJE_A = row.PORCENTAJE_A.Replace("'", "").Trim();
                        row.PORCENTAJE_A = Math.Round(row.PORCENTAJE_A.Dbl(), 2).Str();

                        row.PRECIO_A = row.PRECIO_A.Replace("'", "").Trim();
                        row.PRECIO_A = Math.Round(row.PRECIO_A.Dbl(), 9).Str();

                        row.CODIGO_A = row.CODIGO_A.Trim();


                        /*********************************************************/
                        row.PROVEEDOR_B  = row.PROVEEDOR_B.Trim();

                        row.PORCENTAJE_B = row.PORCENTAJE_B.Replace("'", "").Trim();
                        row.PORCENTAJE_B = Math.Round(row.PORCENTAJE_B.Dbl(), 2).Str();

                        row.PRECIO_B = row.PRECIO_B.Replace("'", "").Trim();
                        row.PRECIO_B = Math.Round(row.PRECIO_B.Dbl(), 9).Str();

                        row.CODIGO_B = row.CODIGO_B.Trim();

                        /*********************************************************/
                        row.PROVEEDOR_C  = row.PROVEEDOR_C.Trim();

                        row.PORCENTAJE_C = row.PORCENTAJE_C.Replace("'", "").Trim();
                        row.PORCENTAJE_C = Math.Round(row.PORCENTAJE_C.Dbl(), 2).Str();

                        row.PRECIO_C = row.PRECIO_C.Replace("'", "").Trim();
                        row.PRECIO_C = Math.Round(row.PRECIO_C.Dbl(), 9).Str();

                        row.CODIGO_C = row.CODIGO_C.Trim();
                        /*********************************************************/

                        row.LEADTIME = row.LEADTIME.Replace("'", "").Trim();

                        row.STOCK_MAXIMO = row.STOCK_MAXIMO.Replace("'", "").Trim();
                        row.STOCK_MAXIMO = Math.Round(row.STOCK_MAXIMO.Dbl(), 2).Str();
                        
                        row.STOCK_SEGURIDAD = row.STOCK_SEGURIDAD.Replace("'", "").Trim();
                        row.STOCK_SEGURIDAD = Math.Round(row.STOCK_SEGURIDAD.Dbl(), 2).Str();

                        row.FRECUENCIA = row.FRECUENCIA.Replace("'", "").Trim();
                        row.FRECUENCIA = Math.Round(row.FRECUENCIA.Dbl(), 2).Str();

                        result.Add(row);
                    }

                    dataRow = dataRow.RowBelow();
                }
            }

            return result;
        }



    }

}