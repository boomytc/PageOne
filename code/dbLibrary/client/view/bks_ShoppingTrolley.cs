using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.client.view
{
    public class bks_ShoppingTrolley : rui.pagerBase
    {
        //定义需要的搜索条件
        public string customerCode { get; set; }
        public string bookName { get; set; }
        public string customerName { get; set; }
        public string addressCode { get; set; }
        public string addressName { get; set; }
        public string price { get; set; }
        public string surfacePic { get; set; }
        public string quantity { get; set; }
        public string subPrice { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "bookCode";
            this.ResourceCode = "bks_ShoppingTrolley";

            //获取登录的用户名
            string customerCode = rui.sessionHelper.getValue("username");

            //拼接搜索语句
            string querySql = "SELECT * FROM dbo.sv_bks_ShoppingTrolley WHERE 1=1 ";
            querySql += rui.dbTools.searchIn("customerCode", customerCode, this.cmdPara);

            //汇总语句
            this.sumSql = @"SELECT SUM(subPrice) AS subPrice FROM dbo.sv_bks_ShoppingTrolley ";
            this.sumRange = rui.dataRange.all.ToString();

            //利用搜索语句获取数据
            this.getPageConfig();
            this.PageSize = 8;
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowNum", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }

    }
}
