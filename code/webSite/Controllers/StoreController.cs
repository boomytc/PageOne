using db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Filters;

namespace web.Controllers
{
    public class StoreController : baseController
    {
        // GET: Store
        public ActionResult Index(db.client.view.bks_Books model, bool? successMessage)
        {
            if (successMessage.HasValue && successMessage.Value)
            {
                TempData["SuccessMessage"] = "加入购物车成功";
            }
            Session.Remove("bookCode");
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
            ViewData["bks_BookType"] = db.bll.bks_bookType.getTable(dc);
            return PartialView();
        }

        public ActionResult Detail(string customerCode, string bookCode, dbEntities dc)
        {
            // 保存 bookCode 到 Session 中
            Session["bookCode"] = bookCode;

            db.bks_Book entry = db.bll.bks_Book.getEntryByCode(bookCode, dc);

            return View(entry);
        }

        [HttpGet]
        [author]
        public ActionResult AddCart(dbEntities dc)
        {
            string customerCode = Session["username"].ToString();
            string bookCode = Session["bookCode"].ToString();

            db.bll.ShoppingTrolley.addCart(customerCode, bookCode, dc);

            return RedirectToAction("Index", new { successMessage = true });

        }
        [HttpGet]
        [author]
        public ActionResult Buy(dbEntities dc)
        {
            string customerCode = Session["username"].ToString();
            string bookCode = Session["bookCode"].ToString();

            db.bll.ShoppingTrolley.addCart(customerCode, bookCode, dc);

            return RedirectToAction("Index", "Shopping");
        }

        public ActionResult GetBooksByType(db.client.view.bks_Books model)
        {
            model.Search();
            if (Request.IsAjaxRequest())
                return PartialView("_showData", model);
            return View(model);
        }

        public ActionResult outload()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Store");
        }
    }
}