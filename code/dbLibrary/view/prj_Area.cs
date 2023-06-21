using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class prj_Area : rui.pagerBase
    {
        //搜索字段
        public string areaShow { get; set; }
        public override void Search()
        {
            this.keyField = "rowID";
            this.ResourceCode = "prj_Area";

            //拼接搜索字段
            string querySql = "select * from prj_Area where 1=1 ";
            querySql += rui.dbTools.searchTbx("areaShow", this.areaShow, this.cmdPara);

            //搜索数据
            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowNum", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;

            //显示的列和列顺序
            this.showColumn = this.getShowColumn();
        }
    }
}
