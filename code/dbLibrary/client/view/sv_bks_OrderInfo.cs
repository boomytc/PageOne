using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.client.view
{
    public class sv_bks_OrderInfo : rui.pagerBase
    {
        public string orderCode { get; set; }
        public string customerCode { get; set; }
        public string addressCode { get; set; }
        public decimal totalPrice { get; set; }
        public string orderDateStart { get; set; }
        public string orderDateEnd { get; set; }
        public string status { get; set; }
        public string employeeCode { get; set; }
        public string remark { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "orderCode";
            this.ResourceCode = "sv_bks_OrderInfo";

            //获取登陆的用户名
            string customerCode = rui.sessionHelper.getValue("username");


            //拼接搜索语句
            string querySql = " SELECT * FROM sv_bks_OrderInfo WHERE 1=1 ";
            querySql += rui.dbTools.searchIn("customerCode", customerCode, this.cmdPara);
            querySql += rui.dbTools.searchDate("orderDate", this.orderDateStart, this.orderDateEnd, this.cmdPara);


            //利用搜索语句获取数据
            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowNum", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }
    }
}
