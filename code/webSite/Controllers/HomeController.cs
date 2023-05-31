using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace web.Controllers
{
    public class HomeController : baseController
    {
        //
        // 图书首页
        public ActionResult Index(db.client.view.bks_Books model)
        { 
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }

        //栏目导航分布视图
        [OutputCache(Duration = 1000)]
        [ChildActionOnly]
        public ActionResult bookTypeNav()
        {
            ViewData["bks_BookTypeTable"] = db.bll.bks_bookType.getTable(dc);
            return PartialView();
        }

    }
}