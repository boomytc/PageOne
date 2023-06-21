using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.admin.Controllers
{
    public class sv_bks_OrderController : baseController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 主界面的查询Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Select(db.view.sv_bks_Order model)
        {
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }
        public ActionResult Detail(string rowID)
        {
            db.view.sv_bks_Order model = db.bll.sv_bks_Order.getEntryByRowID(rowID, dc);
            return View(model);
        }
    }
}