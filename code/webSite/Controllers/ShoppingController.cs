using db;
using db.view;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Filters;

namespace web.Controllers
{
    public class ShoppingController : Controller
    {
        //
        // GET: /Shopping/
        [author]
        public ActionResult Index(db.client.view.bks_ShoppingTrolley model)
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
        public ActionResult orderTypeNav()
        {
            return PartialView();
        }

        //数量+
        public JsonResult add(string bookCode)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string customerCode = Convert.ToString(Session["username"]);
                if (customerCode == null)
                    rui.excptHelper.throwEx("客户端未登录");

                db.dbEntities dc = new db.dbEntities();
                db.bll.ShoppingTrolley.addNum(customerCode, bookCode, dc);
                result.data = rui.jsonResult.getAJAXResult("累加成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }

            return Json(result.data);
        }

        //数量-
        public JsonResult sub(string bookCode)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                string customerCode = Convert.ToString(Session["username"]);
                if (customerCode == null)
                    rui.excptHelper.throwEx("客户端未登录");

                db.dbEntities dc = new db.dbEntities();
                db.bll.ShoppingTrolley.subNum(customerCode, bookCode, dc);
                result.data = rui.jsonResult.getAJAXResult("累加成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }

            return Json(result.data);
        }

        //删除
        [HttpPost]
        public JsonResult Delete(string rowID, db.dbEntities dc)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.ShoppingTrolley.delete(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }

        //下单
        [HttpPost]
        public JsonResult Order(string keyFieldValues, string addressCode, db.dbEntities dc)
        {
            string customerCode = rui.sessionHelper.getValue("username");
            DateTime orderDate = DateTime.Now;
            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            decimal subPrice = db.bll.ShoppingTrolley.returnprice(KeyFieldList, dc);

            Session["orderCode"] = db.bll.bks_OrderInfo._createCode(dc);


            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.bks_OrderInfo.generate(keyFieldValues, addressCode, customerCode, orderDate, subPrice, dc);
                string msg = db.bll.bks_OrderInfo.generateDetail(keyFieldValues, customerCode, dc);
                result.data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return Json(result.data);
        }
        [HttpGet]
        public ActionResult personinfo()
        {
            string name = Session["username"].ToString();
            dbEntities dc = new dbEntities();
            db.bks_Customer model = db.bll.bks_Customer.getEntryByName(name, dc);
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateByPerson(string name, string email, string password, string phone, dbEntities dc)
        {
            name = Session["username"].ToString();
            db.bll.bks_Customer.UpdateByPerson(name, email, password, phone, dc);
            return RedirectToAction("personinfo");
        }

    }
}