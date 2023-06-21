using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.admin.Controllers
{
    public class bks_OrderInfoController : baseController
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
        public ActionResult Select(db.view.bks_OrderInfo model)
        {
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }
        public ActionResult Detail(string rowID)
        {
            db.bks_OrderInfo model = db.bll.bks_OrderInfo.getEntryByRowID(rowID, dc);
            return View(model);
        }

        public JsonResult Send(string rowID)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_OrderInfo.Send(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("发货成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }
  
    }
}