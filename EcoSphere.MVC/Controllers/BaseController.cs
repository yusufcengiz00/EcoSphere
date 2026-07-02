using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EcoSphere.Core.Session;
using Serilog;

namespace EcoSphere.MVC.Controllers;

public abstract class BaseController : Controller
{
    protected bool RequireAdmin { get; set; } = false;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();

        // Check if user is logged in
        if (!CurrentSession.IsLoggedIn)
        {
            if (controllerName != "Account")
            {
                Log.Warning("Yetkisiz erişim denemesi: Controller={Controller}, Action={Action}. Login sayfasına yönlendiriliyor.", controllerName, actionName);
                context.Result = RedirectToAction("Login", "Account");
                return;
            }
        }
        else
        {
            // Set user in ViewBag for the layout to use
            ViewBag.CurrentUser = CurrentSession.CurrentUser;
            
            // Check admin authorization if required
            if (RequireAdmin && !CurrentSession.IsAdmin)
            {
                Log.Warning("Yetkisiz Admin paneli erişim denemesi! Kullanıcı: {User}, Sayfa: {Controller}/{Action}", CurrentSession.CurrentUser?.Username, controllerName, actionName);
                context.Result = RedirectToAction("AccessDenied", "Account");
                return;
            }
        }
    }
}
