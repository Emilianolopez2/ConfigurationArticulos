using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebPage.Attributes
{
    public class GridColumnAttribute : Attribute
    {
         public string TEXT { get; set; }

         public GridColumnAttribute(string text)
         {
              TEXT = text;
         }

         public GridColumnAttribute()
         {
         }
    }
}