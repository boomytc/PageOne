using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class bks_OrderDetail:rui.pagerBase
    {
        public string  orderCode { get; set; }
        public string bookCode { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }     
        public decimal costPrice { get; set; }
        public decimal subTotal { get; set; }
        public char isComment { get; set; }
        public DateTime commentDate { get; set; }
        public string commentContent { get; set; }
        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "orderCode";
            this.ResourceCode = "bks_OrderDetail";

            //拼接搜索语句
            string querySql = " select * from bks_OrderDetail where 1=1 ";
            querySql += rui.dbTools.searchDdl("orderCode", this.orderCode, this.cmdPara);

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
