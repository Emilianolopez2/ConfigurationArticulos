using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.Controllers;
using MvcWebPage.Data;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;
using MvcWebPage.Services;
using Newtonsoft.Json;
using NuGet.Protocol;
using Formatting = Newtonsoft.Json.Formatting;


namespace MvcWebPage
{
    public static class EntityExtensions
    {

        public static string Truncate(this DbContext context, string tableNameWithSchema)
        {
            string cmd = $"TRUNCATE TABLE {tableNameWithSchema}";
            context.Database.ExecuteSqlRaw(cmd);
            return cmd;
        }




        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }

        //public static TKey MaxID<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector) 
        //{

        //    /*
        //    if (collection != null && collection.Count > 0)
        //    {
        //        return collection
        //            .OrderBy(x => keySelector(x))
        //            .ToList();
        //    }*/



        //    var maxId = collection.Select(keySelector).DefaultIfEmpty().Max();

        //    Type t = typeof(TKey);
        //    t = Nullable.GetUnderlyingType(t) ?? t;

        //    if (maxId == null)
        //    {
        //        return (TKey)Convert.ChangeType(1, t);
        //    }


        //    var inx=(Int64)Convert.ChangeType(maxId, t);
        //    inx++;

        //    return (TKey)Convert.ChangeType(inx, t);
        //}


        //public static K MaxOrDefault<T, K>(this IEnumerable<T> enumeration, Func<T, K> selector)
        //{
        //    return enumeration.Any() ? enumeration.Max(selector) : default;
        //}
        public static int MaxID1<T, K>(this IEnumerable<T> enumeration, Func<T, K> selector, bool incremento = true)
        {
            var max = (enumeration.Select(selector).Cast<int?>().Max() ?? 0);

            if (incremento)
            {
                max++;
            }

            return max;
        }

        public static long MaxID2<T, K>(this IEnumerable<T> enumeration, Func<T, K> selector, bool incremento = true)
        {
            var max = (enumeration.Select(selector).Cast<long?>().Max() ?? 0);

            if (incremento)
            {
                max++;
            }

            return max;
        }

    }

    public static class Extensions
     {

         public static void Configure(IHttpContextAccessor httpContextAccessor)
         {
             
         }

        public static string Truncate(this object value, int precision)
         {
             if (precision < 0)
             {
                 throw new ArgumentOutOfRangeException("Precision cannot be less than zero");
             }

             string result = value.ToString();

             int dot = result.IndexOf('.');
             if (dot < 0)
             {
                 return result;
             }

             int newLength = dot + precision + 1;

             if (newLength == dot + 1)
             {
                 newLength--;
             }

             if (newLength > result.Length)
             {
                 newLength = result.Length;
             }

             return result.Substring(0, newLength);
         }

        public static IActionResult RSon(this object obj)
        {

             return new JsonResult(obj.ToJson()); //ToJson()
        }

        public static String GetMD5Hash(this String input)
         {
             MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
             byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
             bs = x.ComputeHash(bs);
             System.Text.StringBuilder s = new System.Text.StringBuilder();
             foreach (byte b in bs)
             {
                 s.Append(b.ToString("x2").ToLower());
             }
             String hash = s.ToString();
             return hash;
         }

        public static string SSubstring(this string value, int startIndex, int length)
          {
               return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
          }


          public static string Str(this Func<object, HelperResult> value)
          {

              TextWriter tx = new StringWriter();
              value.Invoke(null).WriteTo(tx, HtmlEncoder.Default);


            return tx.ToString()
                  .Trim()
                  .Trim("<script>", "</script>")
                  .Trim("\r\n", "\r\n");

          }

        public static string ToTitleCase(this string value)
        {
            TextInfo textInfo  = CultureInfo.CurrentCulture.TextInfo;
            string   titleCase = textInfo.ToTitleCase(value);


            return titleCase;

        }
        public static string Str(this IHtmlContent value)
        {
            string result = "";
            using (var writer = new StringWriter())
            {
                value.WriteTo(writer, HtmlEncoder.Default);
                result = writer.ToString();
            }


            return result
                .Trim()
                .Trim("<script>", "</script>")
                .Trim("\r\n", "\r\n");

        }

        public static string Str(this HtmlString value)
          {
              return value.ToString()
                  .Trim()
                  .Trim("<script>", "</script>")
                  .Trim("\r\n", "\r\n");

          }

          public static string Str(this HelperResult value)
          {
              return value.ToString()
                  .Trim()
                  .Trim("<script>", "</script>")
                  .Trim("\r\n", "\r\n");

          }
        public static void Send(this HtmlString value, IHtmlHelper helper, string type)
          {
              
              helper.Resource(o => new HelperResult(w => w.WriteAsync(value.ToString())), type);
          }

          public static void Send(this string value, IHtmlHelper helper, string type)
          {

              helper.Resource(o => new HelperResult(w => w.WriteAsync(value)), type);
          }

          public static void Send(this StringBuilder value, HtmlHelper helper, string type)
          {

              helper.Resource(o => new HelperResult(w => w.WriteAsync(value)), type);
          }

        public static string Quotes(this string str,bool quotes)
          {

               if (quotes)
               {
                    return "'" + str + "'";
               }
               else
               {
                    return str;
               }
          }

          public static Int32 Int32(this object obj)
          {
              return Convert.ToInt32(obj);
          }

          public static Double Dbl(this object obj)
          {
              return Convert.ToDouble(obj);
          }

        public static string Str(this object obj)
          {
              return Convert.ToString(obj);
          }

          public static string Quotes(this string str)
          {
               return str.Quotes(true);
          }

          public static string Trim(this string input, string str1, string str2)
          {

               return input.TrimStart(str1).TrimEnd(str2);
          }

          public static string TrimStart(this string input, string str)
          {
               //var start = input.Left(str.Length);

               //if (start.Equals(str, StringComparison.OrdinalIgnoreCase))

               if (input.StartsWith(str, StringComparison.OrdinalIgnoreCase))
               {
                    input = input.Substring(str.Length);
               }
               return input;
          }

          public static string TrimEnd(this string input, string str)
          {
               //var end = input.Right(str.Length);

               //if (end.Equals(str, StringComparison.OrdinalIgnoreCase))


               if (input.EndsWith(str, StringComparison.OrdinalIgnoreCase))
               {
                    input = input.Substring(0, input.Length - str.Length);
               }
               return input;
          }

          public static string Right(this string input, int count)
          {
               return String.Join("", (input + "").ToCharArray().Reverse().Take(count).Reverse());
          }

          public static string Left(this string input, int count)
          {
               return String.Join("", (input + "").ToCharArray().Take(count));
          }

          /*         public static string Left1(this string str, int length)
                   {
                        str = (str ?? string.Empty);
                        return str.Substring(0, Math.Min(length, str.Length));
                   }
  
                   public static string Right1(this string str, int length)
                   {
                        str = (str ?? string.Empty);
                        return (str.Length >= length)
                                  ? str.Substring(str.Length - length, length)
                                  : str;
                   }*/


          
          public static string Json(this object obj)
          {
               var serializer   = new JsonSerializer{NullValueHandling = NullValueHandling.Ignore};
               var stringWriter = new StringWriter();

               using( var writer = new JsonTextWriter(stringWriter) )
               {
                    writer.QuoteName = false;
                    writer.QuoteChar = '\'';

                    writer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, obj);

               }


               return stringWriter.ToString();
          }

          public static string GetUserName()
          {

              IHttpContextAccessor ctx = new HttpContextAccessor();
              var context = ctx.HttpContext;


              if (context != null)
              {
                  var identity = (ClaimsIdentity)context.User.Identity;

                  if (identity != null)
                  {
                      var claim = identity.FindFirst(ClaimTypes.Name);

                      if (claim == null)
                      {
                          return "";
                      }


                      return claim.Value;
                  }
              }

              return "";
          }

          public static string GetCodAlmacen()
          {
              var userName = GetUserName();

              if (userName != "")
              {
                  MLAVIDContext db = new MLAVID_DB();

                  var vnd = db.VENDEDORES.FirstOrDefault(w => w.NOMVENDEDOR.ToLower() == userName.ToLower());

                  if (vnd != null)
                  {
                      return vnd.CODALMACEN.Str();
                  }
              }

              return "";
          }

          public static int? GetCodVendedor()
          {
              var userName = GetUserName();

              if (userName != "")
              {
                  MLAVIDContext db = new MLAVID_DB();

                  var vnd = db.VENDEDORES.FirstOrDefault(w =>
                      w.NOMVENDEDOR.ToLower() == userName.ToLower());

                  if (vnd != null)
                  {
                      return vnd.CODVENDEDOR;
                  }
              }

              return null;
          }


          public static int? GetCodProveedor()
          {
              var userName = GetUserName();

              if (userName != "")
              {
                  MLAVIDContext db = new MLAVID_DB();

                  var vnd = db.VENDEDORES.FirstOrDefault(w => w.NOMVENDEDOR.ToLower() == userName.ToLower());

                  if (vnd != null && !string.IsNullOrEmpty(vnd.DNI))
                  {

                      var prv = db.PROVEEDORES.FirstOrDefault(f => f.NIF20 == vnd.DNI);

                      if (prv != null)
                      {
                          return prv.CODPROVEEDOR;
                      }

                      return null;
                  }
              }

              return null;
          }


          public static string GetNomProveedor()
          {
              var userName = GetUserName();
              if (userName != "")
              {
                  MLAVIDContext db = new MLAVID_DB();

                  var vnd = db.VENDEDORES.FirstOrDefault(w => w.NOMVENDEDOR.ToLower() == userName.ToLower());

                  if (vnd != null && !string.IsNullOrEmpty(vnd.DNI))
                  {

                      var prv = db.PROVEEDORES.FirstOrDefault(f => f.NIF20 == vnd.DNI);

                      if (prv != null)
                      {
                          return prv.NOMPROVEEDOR;
                      }

                      return null;
                  }
              }

              return null;
          }

          public static string GetlbSucursal()
          {
              var userName = GetUserName();
              if (userName != "")
              {
                  MLAVIDContext db = new MLAVID_DB();

                  var vnd = db.VENDEDORES.FirstOrDefault(w => w.NOMVENDEDOR.ToLower() == userName.ToLower());

                  if (vnd != null)
                  {
                      var lbSucursal = db.ALMACEN.FirstOrDefault(f => f.CODALMACEN == vnd.CODALMACEN);

                      if (lbSucursal != null)
                      {
                          return lbSucursal.NOMBREALMACEN;
                      }
                      else
                      {
                          return "";
                      }
                  }
              }

              return "";
          }



     }
}



/*


    public MLAVIDContext()
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }


//*********************************************************************************


            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var contextOptions = new DbContextOptionsBuilder<MLAVIDContext>()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .Options;

            var db = new MLAVIDContext(contextOptions);
 */