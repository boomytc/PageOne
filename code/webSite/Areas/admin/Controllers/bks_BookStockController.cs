using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.admin.Controllers
{
    public class bks_BookStockController : baseController
    {
        //
        // GET: /admin/bks_BookStock/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 主界面的查询Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Select(db.view.bks_BookStock model)
        {
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }

        //详情
        public ActionResult Detail(string rowID)
        {
            db.bks_BookStock model = db.bll.bks_BookStock.getEntryByRowID(rowID, dc);
            return View(model);
        }

        //展示新增
        [HttpGet]
        public ActionResult Insert()
        {
            db.bks_BookStock model = new db.bks_BookStock();
            return View(model);
        }

        //保存新增
        [HttpPost]
        public JsonResult Insert(db.bks_BookStock model)
        {
            JsonResult result = new JsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.bks_BookStock.insert(model, dc);
                    result.Data = rui.jsonResult.getAJAXResult("新增成功", true,
                        rui.jsonResult.getDicByRowID(rowID));
                }
                else
                {
                    result.Data = rui.jsonResult.getAJAXResult("输入不合法", false);
                }
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }

        //展示更新
        [HttpGet]
        public ActionResult Update(string rowID)
        {
            db.bks_BookStock model = db.bll.bks_BookStock.getEntryByRowID(rowID, dc);
            return View(model);
        }

        //保存更新
        [HttpPost]
        public ActionResult Update(db.bks_BookStock model, List<string> rowIDList, List<string> bookCodeList, List<string> quantityList, List<string> priceList)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    db.bll.bks_bookStockDetail.batchSave(rowIDList,bookCodeList,quantityList,priceList,dc);
                    db.bll.bks_BookStock.update(model, dc);
                    result.data = rui.jsonResult.getAJAXResult("更新成功", true);
                }
                else
                {
                    result.data = rui.jsonResult.getAJAXResult("输入不合法", false);
                }
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
        public JsonResult Delete(string rowID)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_BookStock.delete(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }

        public JsonResult publish(string rowID) 
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_BookStock.publish(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("发布成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }
        //入库信息excel导出
        public ActionResult SelectExport(db.view.bks_BookStock model)
        {
            try
            {
                //all代表要导出查询的所有数据,page代表导出本页的数据
                model.ExportRange = rui.dataRange.all.ToString();
                model.SheetName = "入库信息";
                model.Search();
                return File(model.ExportToXls(), rui.innerCode.mime(".xlsx"), "入库信息.xlsx");
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                Response.Write(string.Format("<script>alert('{0}');history.go(-1);</script>", ex.Message));
            }
            return null;
        }
    }
}