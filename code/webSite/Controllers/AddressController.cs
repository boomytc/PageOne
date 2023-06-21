﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class AddressController : baseController
    {
        // GET: Address
        public ActionResult Index(db.client.view.bks_CustomerAddress model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }
        //展示新增
        [HttpGet]
        public ActionResult Insert()
        {
            db.bks_CustomerAddress model = new db.bks_CustomerAddress();
            return View(model);
        }

        //保存新增
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Insert(db.bks_CustomerAddress model)
        {
            JsonResult result = new JsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    string rowID = db.bll.bks_CustomerAddress.insert(model, dc);
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
            db.bks_CustomerAddress model = db.bll.bks_CustomerAddress.getEntryByRowID(rowID, dc);
            return View(model);
        }

        //保存更新
        [HttpPost]
        public ActionResult Update(db.bks_CustomerAddress model)
        {
            JsonResult result = new JsonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    db.bll.bks_CustomerAddress.update(model, dc);
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
                db.bll.bks_CustomerAddress.delete(rowID, dc);
                result.Data = rui.jsonResult.getAJAXResult("删除成功", true);
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