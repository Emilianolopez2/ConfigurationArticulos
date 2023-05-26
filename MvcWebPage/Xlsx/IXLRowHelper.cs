using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using ClosedXML.Excel;

namespace MvcWebPage.Xlsx
{
    public static class IXLRowHelper
    {
        private static Dictionary<string, PropertyInfo[]> _props = new Dictionary<string, PropertyInfo[]>();

        /// <summary>
        /// Get properties of type T and cache them
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties<T>()
        {
            var t = (typeof(T)).FullName;

            if (!_props.ContainsKey(t))
            {
                _props.Add(t, (typeof(T)).GetProperties());
            }

            return _props[t];
        }

        /// <summary>
        /// Validate excel table header row.
        /// Row is valid when it contains all column names defined in field attributes in XlsxRow map file.
        /// </summary>
        /// <param name="row">IXLRow - represents xlsx table header row</param>
        /// <returns></returns>
        public static bool IsValidHeaderRow<T>(this IXLRow row, Hashtable cols)
        {
            bool isValid = true;
            //bool isValid = false;
            cols.Clear();
            var properties = GetProperties<T>();
            var cells = row.Cells();

            foreach (var property in properties)
            {
                var attr =
                    Attribute.GetCustomAttribute(property, typeof(XlsxColumnAttribute)) as XlsxColumnAttribute;

                if (attr == null) continue;

                var columnName = attr.Name;

                var headerColumn =
                    cells.FirstOrDefault(
                        x =>
                            !x.HasFormula &&
                            string.Equals(x.GetString().Replace(" ", ""), columnName.Replace(" ", ""),
                                StringComparison.InvariantCultureIgnoreCase));

                if (headerColumn == null)
                {
                    isValid = false;
                    //isValid = true;
                    //break;
                    cols.Add(attr.Name,"");
                }
            }
            
            return isValid;
        }


        /// <summary>
        /// Resolve column indexes by column names and return name/index mapping
        /// </summary>
        /// <typeparam name="T">type of class which contains mappings</typeparam>
        /// <param name="headerRow">excel table header row</param>
        /// <returns></returns>
        public static T GetColumnMappings<T>(this IXLRow headerRow) where T : new()
        {
            var columnMap = new T();

            var properties = GetProperties<T>();

            foreach (var prop in properties)
            {
                var attr =
                    Attribute.GetCustomAttribute(prop, typeof(XlsxColumnAttribute)) as XlsxColumnAttribute;

                if (attr == null) continue;

                var cells = headerRow.Cells();
                var columnNumbers = cells.Where(
                    x =>
                        !x.HasFormula &&
                        string.Equals(x.GetString().Replace(" ", "").QuitaAcentos(), attr.Name.Replace(" ", "").QuitaAcentos(),
                            StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.WorksheetColumn().ColumnNumber())
                    .ToList();
                if (attr.IsMultiple)
                {
                    var list = new List<int>();
                    foreach (var columnNumber in columnNumbers)
                    {
                        list.Add(columnNumber);
                    }
                    prop.SetValue(columnMap, list, null);
                }
                else
                {
                    prop.SetValue(columnMap, columnNumbers.FirstOrDefault(), null);
                }

            }
            return columnMap;
        }


        /// <summary>
        /// Parse row based on mappings and convert into result of type T
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="row">table row</param>
        /// <param name="map">column mappings</param>
        /// <returns></returns>
        public static T ConvertRow<T>(this IXLRow row, object map) where T : new()
        {
            var rowMap = new T();

            var properties = GetProperties<T>();
            foreach (var prop in properties)
            {
                var attr =
                        Attribute.GetCustomAttribute(prop, typeof(XlsxColumnAttribute)) as XlsxColumnAttribute;

                if (attr == null) continue;

                // since mapping and data are instances of the same class MrpColumns<T> property names will be the same
                var columnIndex = map.GetType().GetProperty(prop.Name).GetValue(map, null);

                if (columnIndex == null || (int)columnIndex == 0)
                    continue;

                if (attr.IsMultiple)
                {
                    //var list = new List<string>();
                    //foreach (var index in (List<int>)columnIndex)
                    //{
                    //    var cell = row.Cell((int)index);
                    //    list.Add(cell.GetString());
                    //}
                    //prop.SetValue(rowMap, list, null);
                }
                else
                {
                    object cell = null;

                    try
                    {
                        if (row.Cell((int) columnIndex).HasFormula)
                        {
                            cell = row.Cell((int)columnIndex).CachedValue.ToString();
                        }
                        else
                        {
                            cell = row.Cell((int)columnIndex).Value.ToString();
                        }
                        
                    }
                    catch(Exception e)
                    {
                    }

                    Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    object safeValue = (cell == null) ? null : Convert.ChangeType(cell, t);



                    //var    converter = TypeDescriptor.GetConverter(t);
                    //object safeValue = (cell == null) ? null : converter.ConvertFrom(cell);


                    prop.SetValue(rowMap, safeValue, null);
                }

            }
            return rowMap;
        }

        public static List<string> IsValid(this object columnMap)
        {
            List<string>it=new List<string>();

            //typeof(xlsx_STOCK<int>).GetProperties()
            foreach (PropertyInfo pi in columnMap.GetType().GetProperties())
            {
                var attr =
                        Attribute.GetCustomAttribute(pi, typeof(XlsxColumnAttribute)) as XlsxColumnAttribute;

                if (attr == null) continue;

                object value = pi.GetValue(columnMap, null);
                if (value != null && value.Equals(0)) //!string.IsNullOrEmpty(value.ToString())
                {
                    it.Add(attr.Name);
                }
            }

            return it;
        }
    }
}