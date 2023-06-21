using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.client.view
{
    public class bks_CustomerAddress : rui.pagerBase
    {
        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "rowID";
            this.ResourceCode = "bks_CustomerAddress";

            //获取登陆的用户名
            string customerCode = rui.sessionHelper.getValue("username");

            //拼接搜索语句
            string querySql = "SELECT * FROM dbo.bks_CustomerAddress WHERE 1=1 ";
            querySql += rui.dbTools.searchIn("customerCode", customerCode, this.cmdPara);


            //利用搜索语句获取数据
            this.getPageConfig();
            this.PageSize = 6;
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowNum", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }
    }
}
