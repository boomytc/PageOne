using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class bks_BookStockDetail : rui.pagerBase
    {
        //定义需要的搜索条件
        public string rowID { get; set; }
        public string bookCode { get; set; }
        public string stockCode { get; set; }
        public string detailNo { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "stockCode";
            this.ResourceCode = "bks_BookStockDetail";

            //拼接搜索语句
            string querySql = " select * from bks_BookStockDetail where 1=1 ";
            querySql += rui.dbTools.searchDdl("stockCode", this.stockCode, this.cmdPara);
            querySql += rui.dbTools.searchDdl("detailNo", this.detailNo, this.cmdPara);

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
