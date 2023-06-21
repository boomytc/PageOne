using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class sv_bks_Order : rui.pagerBase
    {
        public DateTime orderDate { get; set; }
        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string orderCode { get; set; }
        public string bookCode { get; set; }    
        public string customerCode { get; set; }
        public string orderDateStart { get; set; }
        public string orderDateEnd { get; set; }
        public decimal price { get; set; }
        public decimal costPrice { get; set; }
        public decimal subTotal { get; set; }
        public string status { get; set; }
        public decimal totalPrice { get; set; }
        public string addressCode { get; set; }
        public int quantity { get; set; }   


        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "orderCode";
            this.ResourceCode = "sv_bks_Order";


            //拼接搜索语句
            string querySql = " select * from sv_bks_Order where 1=1 ";
            querySql += rui.dbTools.searchIn("customerCode", this.customerCode, this.cmdPara);
            querySql += rui.dbTools.searchIn("bookCode", this.bookCode, this.cmdPara);
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