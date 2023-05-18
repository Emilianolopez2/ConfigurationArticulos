using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcWebPage;
namespace System.Web.Mvc
{

    public static class renderResources
    {

        public static void Script(this IHtmlHelper HtmlHelper, Func<object, HelperResult> Template)
        {

            Resource(HtmlHelper, o => new HelperResult(w => w.WriteLineAsync(Template.ToString())), "script");
        }

        public static HtmlString Resource(this IHtmlHelper HtmlHelper, Func<object, HelperResult> Template, string Type)
        {
            if (HtmlHelper.ViewContext.HttpContext.Items[Type] != null)
            {
                ((List<Func<object, HelperResult>>)HtmlHelper.ViewContext.HttpContext.Items[Type]).Add(Template);
            }
            else
            {
                HtmlHelper.ViewContext.HttpContext.Items[Type] = new List<Func<object, HelperResult>> { Template };
            }

            return new HtmlString(String.Empty);
        }

        public static HtmlString RenderResources(this IHtmlHelper HtmlHelper, string Type)
        {
            if (HtmlHelper.ViewContext.HttpContext.Items[Type] == null) return new HtmlString(String.Empty);

            var Resources = (List<Func<object, HelperResult>>)HtmlHelper.ViewContext.HttpContext.Items[Type];

            foreach (var Resource in Resources.Where(Resource => Resource != null))
            {
                HtmlHelper.ViewContext.Writer.Write(Resource(null));
            }

            return new HtmlString(String.Empty);
        }
    }
}