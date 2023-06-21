using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Controllers;

namespace web.Areas.admin.Controllers
{
    public class bks_BookStockDetailController : baseController
    {
        // GET: admin/bks_BookStockDetail
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 主界面的查询Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Select(db.view.bks_BookStockDetail model)
        {
            model.Search();
            //ajax请求是返回局部视图
            if (Request.IsAjaxRequest())
                return PartialView("_showData" + model.opMode, model);
            return PartialView(model);
        }

        //详情
        public ActionResult Detail(string rowID)
        {
            db.bks_BookStockDetail model = db.bll.bks_bookStockDetail.getEntryByRowID(rowID, dc);
            return View(model);
        }

        //展示新增
        [HttpGet]
        public ActionResult Insert(string stockCode)
        {
            db.bks_BookStockDetail model = new db.bks_BookStockDetail();
            model.stockCode= stockCode;
            return View(model);
        }

        //保存新增
        [HttpPost]
        public JsonResult Insert(db.bks_BookStockDetail model)
        {
            JsonResult result = new JsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.bks_bookStockDetail.insert(model, dc);
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
            db.bks_BookStockDetail model = db.bll.bks_bookStockDetail.getEntryByRowID(rowID, dc);
            return View(model);
        }

        //保存更新
        [HttpPost]
        public ActionResult Update(db.bks_BookStockDetail model)
        {
            JsonResult result = new JsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    db.bll.bks_bookStockDetail.update(model, dc);
                    result.Data = rui.jsonResult.getAJAXResult("更新成功", true);
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

        //删除
        [HttpPost]
        public JsonResult Delete(string rowID)
        {
            JsonResult result = new JsonResult();
            try
            {
                db.bll.bks_bookStockDetail.delete(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("删除成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(ex.Message, false);
            }
            return result;
        }

        //批量新增图书明细
        [HttpPost]
        public JsonResult batchInsert(string stockCode, string selectedBooks)
        {
            rui.jsonResult result = new rui.jsonResult();
            try
            {
                db.bll.bks_bookStockDetail.batchInsert(stockCode, selectedBooks, dc);
                result.data = rui.jsonResult.getAJAXResult("批量新增成功", true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }
            return Json(result.data);
        }

        //批量保存
        public JsonResult batchSave()
        {
            JsonResult result = new JsonResult();
            try
            {
                //获取界面数据
                List<string> stockCodeList = rui.requestHelper.getList("detail.stockCode");
                List<string> detailNoList = rui.requestHelper.getList("detail.detailNo");
                //将获取数据传给业务类
                string msg = db.bll.bks_press.batchSave(stockCodeList, detailNoList, dc);
                result.Data = rui.jsonResult.getAJAXResult(msg, true);
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                result.Data = rui.jsonResult.getAJAXResult(rui.excptHelper.getExMsg(ex), false);
            }
            return result;
        }


        public ActionResult SelectExport(db.view.bks_BookStockDetail model)
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