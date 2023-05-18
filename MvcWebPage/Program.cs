using System.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using MvcWebPage;
using MvcWebPage.Filter;
using MvcWebPage.MLAVID;
using MvcWebPage.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

/*
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PedLibre", policy => policy.RequireRole("SUCURSAL"));
    options.AddPolicy("PedModifica", policy => policy.RequireRole("SUCURSAL"));
});*/



builder.Services.AddAuthorization(options =>
{

    //------------------ SUCURSAL --------------------//

    options.AddPolicy("PedLibre", policy =>
    {
        policy.RequireClaim("PedLibre");
        policy.RequireRole("SUCURSAL");
    });

    options.AddPolicy("PedModifica", policy =>
    {
        policy.RequireClaim("PedModifica");
        policy.RequireRole("SUCURSAL");
    });

    options.AddPolicy("PedElimina", policy =>
    {
        policy.RequireClaim("PedElimina");
        policy.RequireRole("SUCURSAL");
    });

});




// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(typeof(RequestAuthenticationFilter));
//}).AddNewtonsoftJson();

/*
var contextOptions = new DbContextOptionsBuilder<MLAVIDContext>()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .Options;

using var context = new MLAVIDContext(contextOptions);

var mn=context.ALMACEN.ToList();
*/


builder.Services.AddDbContext<MLAVIDContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddControllersWithViews().AddNewtonsoftJson();

/*
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath        = "/Acceso/Index";
        option.ExpireTimeSpan   = TimeSpan.FromMinutes(20);
        option.AccessDeniedPath = "/Home/Privacy";
    });
*/

/*
options => {
    options.DefaultScheme = "Cookies";
}
 */

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie( options => {

    options.LoginPath            = "/Acceso/Index";
    options.ExpireTimeSpan       = TimeSpan.FromMinutes(20); //TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/Home/Privacy";

        /*
    options.Cookie.Name = "auth_cookie";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Events = new CookieAuthenticationEvents
    {
        
        OnRedirectToLogin = redirectContext =>
        {
            redirectContext.HttpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
        */

});


builder.Services.AddSession(options =>
{
    //options.Cookie.Name        = "Prueba123";
    options.IdleTimeout        = TimeSpan.FromMinutes(20);//TimeSpan.FromMinutes(20); //TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


builder.Services.Configure<FormOptions>(options =>
{

    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.BufferBodyLengthLimit = int.MaxValue;
    options.BufferBody = true;
    options.ValueCountLimit = int.MaxValue;
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
//});


//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.MaxRequestBodySize       = 524288000; // 500 MB
//});



/*
builder.Services.AddMvc().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
    o.JsonSerializerOptions.DictionaryKeyPolicy = null;
});
*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();




app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


//app.Use(async (context, next) =>
//{
//    var urlHelperFactory = context.RequestServices.GetRequiredService<IUrlHelperFactory>();

//    var urlHelper        = urlHelperFactory.GetUrlHelper(context.RequestServices);


//    if (!context.User.Identity.IsAuthenticated && context.Request.Path != "/")
//    {

//        var session = context.Session;
//        if (session != null && session.GetString("UserName") == null)
//        {
//            context.Response.Redirect("/");
//            return;
//        }
//    }


//    await next.Invoke();
//});


/*
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}");



app.Run();
