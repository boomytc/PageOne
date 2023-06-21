using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class bks_Customer:rui.pagerBase
    {
        public string customerCode { get; set; }
        public string customerName { get; set; }
        public string password { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public override void Search()
        {
            //批量操作时，给后台传值的列
            this.keyField = "customerCode";
            this.ResourceCode = "bks_Customer";

            //拼接搜索语句
            string querySql = " select * from bks_Customer";
            querySql += rui.dbTools.searchDdl("customerCode", this.customerCode, this.cmdPara);
            querySql += rui.dbTools.searchDdl("customerName", this.customerName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("email", this.email, this.cmdPara);

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
