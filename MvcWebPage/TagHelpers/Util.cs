using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MvcWebPage.TagHelpers
{
    public static class Util
    {

        public static string GetFecha(this IHtmlHelper html)
        {
            var dt = DateTime.Now.AddDays(-DateTime.Now.Day + 1);

            var yyyy = dt.Year;
            var mm   = dt.Month;
            var dd   = dt.Day;


            return string.Format("new Date('{0}/{1}/{2}')", yyyy, mm, dd);
        }

        public static string GetLogin(this IHtmlHelper helper) 
            {

            var urlHelperFactory = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(helper.ViewContext);
            

            
            string loginUrl = urlHelper.Action("Index", "Acceso", null, helper.ViewContext.HttpContext.Request.Scheme);

            
            return loginUrl;
            }



            public static HtmlString GetScript(this IHtmlHelper helper, IHtmlContent htmlContent)
            {
                return new HtmlString(htmlContent.Str());
            }


            public static void ToScript(this IHtmlHelper helper, Func<object, HelperResult> script)
            {

                var src = script.Str();
                src.Send(helper, "script");
        }

    }
}
