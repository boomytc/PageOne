using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class bks_BookType : rui.pagerBase
    {
        //定义需要的搜索条件
        public string rowID { get; set; }
        public string bookCode { get; set; }
        public string bookTypeCode { get; set; }
        public string bookTypeName { get; set; }

        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "bookTypeCode";
            this.ResourceCode = "bks_BookType";

            //拼接搜索语句
            string querySql = " select * from bks_BookType";
            querySql += rui.dbTools.searchDdl("rowID", this.rowID, this.cmdPara);
            querySql += rui.dbTools.searchDdl("bookTypeCode", this.bookTypeCode, this.cmdPara);
            querySql += rui.dbTools.searchTbx("bookTypeName", this.bookTypeName, this.cmdPara);

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
