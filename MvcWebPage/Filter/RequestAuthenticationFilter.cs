using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MvcWebPage.Filter
{
    public class RequestAuthenticationFilter : Attribute, IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RequestAuthenticationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {


            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var session = _httpContextAccessor.HttpContext.Session.GetString("UserName");
                //var path    = _httpContextAccessor.HttpContext.Request.Path;


                var urlHelperFactory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
                var urlHelper        = urlHelperFactory.GetUrlHelper(context);


                string loginUrl = urlHelper.Action("Index", "Acceso", null, context.HttpContext.Request.Scheme); //, null, context.HttpContext.Request.Scheme


                var head=context.HttpContext.Request.Headers["X-Requested-With"];


                if (_httpContextAccessor.HttpContext.Request.GetDisplayUrl() != loginUrl)
                {
                    if (session == null && head.Count == 0)
                    {
                        context.HttpContext.Response.Redirect(loginUrl);
                    }
                    else
                    {
                        //context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        context.HttpContext.Response.StatusCode = 401;
                        

                    }
                }



            }




            //if (session == null)
            //{
            //    //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Acceso" }));
            //}
        }

        /// <summary>
        /// OnActionExecuted
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
