using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class bks_OrderInfo : rui.pagerBase
    {
        public string orderCode { get; set; }
        public string customerCode { get; set; }
        public string addressCode { get; set; }
        public decimal totalPrice { get; set; }
        public string orderDateStart { get; set; }
        public string orderDateEnd { get; set; }
        public string status { get; set; }
        public string employeeCode { get; set; }
        public string bookCode { get; set; }    
        public string remark { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "orderCode";
            this.ResourceCode = "bks_OrderInfo";
          

            //拼接搜索语句
            string querySql = " select * from bks_OrderInfo where 1=1 ";
            querySql += rui.dbTools.searchIn("customerCode", this.customerCode, this.cmdPara);
            querySql += rui.dbTools.searchIn("orderCode", this.orderCode, this.cmdPara);
            querySql += rui.dbTools.searchDate("orderDate", this.orderDateStart, this.orderDateEnd, this.cmdPara);
            querySql += rui.dbTools.searchIn("bookCode", this.bookCode, this.cmdPara);

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