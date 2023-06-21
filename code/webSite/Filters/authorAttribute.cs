using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace web.Filters
{
    public class authorAttribute : FilterAttribute, IAuthorizationFilter
    {
        //完成登录检测
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionAttr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true) as IEnumerable<AllowAnonymousAttribute>;
            var controllerAttr = filterContext.Controller.GetType().GetCustomAttributes(typeof(AllowAnonymousAttribute), true) as IEnumerable<AllowAnonymousAttribute>;

            if ((actionAttr != null && actionAttr.Any())
                || (controllerAttr != null && controllerAttr.Any()))
                return;
            //未登录
            if (filterContext.HttpContext.Session["isLogin"] == null)
            {
                string url = filterContext.HttpContext.Request.RawUrl;
                filterContext.HttpContext.Session["toUrl"] = url;
                filterContext.Result = new RedirectResult("/UserLogin/Login");
            }
        }
    }
}