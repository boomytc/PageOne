using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Controllers
{
    public class prj_AreaController : Controller
    {
        //
        // GET: /admin/prj_Area/
        public ActionResult Select(db.view.prj_Area model)
        {
            model.Search();
            if (rui.requestHelper.isAjax())
                return PartialView("_showData", model);
            return View(model);
        }
	}
}