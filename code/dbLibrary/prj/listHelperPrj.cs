using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace rui
{
    /// <summary>
    /// 存放每个系统需要增加的内容  - 参考sysLib的同名文件
    /// </summary>
    public partial class listHelper
    {
        //text和value一致时使用
        private static string _上下架 = "是,否";
        public static List<SelectListItem> bind上下架(bool has请选择 = false, string selectedValues = "")
        {
            return bindValues(_是否, has请选择, selectedValues);
        }
    }
}
