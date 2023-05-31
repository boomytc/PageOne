using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.client.view
{
    public class bks_Books : rui.pagerBase
    {
        //定义需要的搜索条件
        public string bookName { get; set; }
        public string press { get; set; }
        public string type { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "bookCode";
            this.ResourceCode = "bks_Book";

            //拼接搜索语句
            string querySql = " select * from sv_bks_Book where 1=1 ";
            querySql += rui.dbTools.searchDdl("bookName", this.bookName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("pressCode", this.bookName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("bookTypeCode", this.bookName, this.cmdPara);

            //利用搜索语句获取数据
            this.getPageConfig();
            this.PageSize = 5;
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowNum", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }

    }
}
