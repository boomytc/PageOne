using rui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class OrderController : baseController
    {
        // GET: Order
        public ActionResult Index(db.client.view.sv_bks_OrderInfo model)
        {
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }
        //确认付款
        public JsonResult Pay(string rowID)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_OrderInfo.Pay(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("付款成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }

        //确认收货
        public JsonResult Confirm(string rowID)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_OrderInfo.Confirm(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("签收成功", true);
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