using System.Data;
using System.Globalization;

namespace rui
{
    /// <summary>
    /// 数据分页的辅助类
    /// </summary>
    public class pagerHelper
    {
        private string querySql = "";       //传入的查询语句
        private string countSql = "";       //通过查询语句构造
        private string orderSql = "";       //传入的排序子语句
        private string pagerSql = "";       //存放分页获取子语句
        private pagerBase pagerPara;        //分页参数
        private DataTable pagerResult;      //分页数据

        /// <summary>
        /// 保存分页查询结果
        /// </summary>
        public DataTable Result { get { return pagerResult; } set { pagerResult = value; } }

        /// <summary>
        /// 默认不带参数的构造函数
        /// </summary>
        public pagerHelper() { }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="querySql">查询字符串</param>
        /// <param name="orderSql">查询总数量字符串</param>
        /// <param name="pagerPara">分页参数</param>
        public pagerHelper(string querySql, string orderSql, pagerBase pagerPara)
        {
            this.querySql = querySql;
            this.orderSql = orderSql;
            this.pagerPara = pagerPara;
            if (this.pagerPara.PageSize == 0)
                this.pagerPara.PageSize = rui.configHelper.pageSize;
            this.processSql();
        }

        /// <summary>
        /// 对传入的查询sql进行处理
        /// 处理完毕之后获取到getField，countSql语句
        /// </summary>
        private void processSql()
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            int selectEndPosition = compare.IndexOf(this.querySql, "select ", CompareOptions.IgnoreCase) + 6;
            int fromStartPosition = compare.IndexOf(this.querySql, " from ", CompareOptions.IgnoreCase);

            if (selectEndPosition < 0)
                rui.excptHelper.throwEx("select关键字后边需要有空格");
            if (fromStartPosition < 0)
                rui.excptHelper.throwEx("from关键字前后需要有空格");

            this.countSql = string.Format("select count(*) {0}", querySql.Substring(fromStartPosition));

            //处理分页语句
            int offset = (this.pagerPara.PageIndex - 1) * this.pagerPara.PageSize;
            int next = this.pagerPara.PageSize;
            this.pagerSql = string.Format(" offset {0}  ROWS FETCH NEXT {1} ROWS only ", offset, next);

            //拼接上排序语句
            //this.querySql += " " + this.orderSql;
        }

        /// <summary>
        /// 数据查询方法
        /// 查询完毕，设置pageCount,rowCount
        /// </summary>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pager">分页基类</param>
        /// <param name="exceptDateFieldName">需要排除的列</param>
        public void Execute(int pageSize, int pageIndex, rui.pagerBase pager, string exceptDateFieldName = "")
        {
            using (rui.dbHelper dbHelper = rui.dbHelper.createHelper())
            {
                pager.LongDateField = exceptDateFieldName;

                //不进行数据导出
                if(rui.typeHelper.isNullOrEmpty(pager.SheetName))
                {
                    //获取本页数据展示
                    this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql + this.pagerSql, pager.cmdPara);
                    rui.dbTools.insert序号(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                    //执行汇总语句时获取记录总数
                    if (pager.ExeCountSql)
                    {
                        this.pagerPara.RowCount = rui.typeHelper.toInt(dbHelper.ExecuteScalar(this.countSql, pager.cmdPara));
                        this.pagerPara.PageCount = rui.typeHelper.toInt(pager.RowCount / pager.PageSize) + (pager.RowCount % pager.PageSize == 0 ? 0 : 1);
                    }

                    //汇总本页的数据
                    if (pager.sumRange == rui.dataRange.page.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                    {
                        string sumSql = pager.sumSql + string.Format( " where {0} in ({1}) ",pager.keyField,rui.dbTools.getInExpression(this.pagerResult, pager.keyField));
                        DataRow sumRow = dbHelper.ExecuteDataRow(sumSql);
                        foreach (DataColumn col in sumRow.Table.Columns)
                        {
                            pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                        }
                    }
                    //汇总所有查询结果
                    if (pager.sumRange == rui.dataRange.all.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                    {
                        string sumSql = pager.sumSql + rui.dbTools.getWhereExpr(querySql);
                        DataRow sumRow = dbHelper.ExecuteDataRow(sumSql, pager.cmdPara);
                        foreach (DataColumn col in sumRow.Table.Columns)
                        {
                            pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                        }
                    }
                }
                //进行数据导出
                else
                {
                    if (rui.typeHelper.isNullOrEmpty(pager.ExportRange))
                        rui.excptHelper.throwEx("ExportRange属性必须赋值");

                    //导出勾选数据，返回勾选的数据
                    if (pager.ExportRange == rui.dataRange.selected.ToString())
                    {
                        if (rui.typeHelper.isNullOrEmpty(pager.CbxSelectedKeys))
                            rui.excptHelper.throwEx("CbxSelectedKeys属性必须赋值");

                        //查询要导出的数据
                        string whereExpr = string.Format(" and {0} in ({1}) ",pager.keyField,rui.dbTools.getInExpression(pager.CbxSelectedKeys));
                        //获取勾选行的数据
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + whereExpr + this.orderSql, pager.cmdPara);
                        rui.dbTools.insert序号(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //查询要导出的汇总行数据
                        if (rui.typeHelper.isNotNullOrEmpty(pager.sumRange))
                        {
                            string sumSql = pager.sumSql + " where 1=1 " + whereExpr;
                            DataRow sumRow = dbHelper.ExecuteDataRow(sumSql, pager.cmdPara);
                            foreach (DataColumn col in sumRow.Table.Columns)
                            {
                                pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                            }
                        }
                    }
                    //导出本页时，返回本页的数据
                    if (pager.ExportRange == rui.dataRange.page.ToString())
                    {
                        //查询要导出的数据
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql + this.pagerSql, pager.cmdPara);
                        rui.dbTools.insert序号(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //查询要导出的汇总行数据
                        if (rui.typeHelper.isNotNullOrEmpty(pager.sumRange))
                        {
                            string sumSql = pager.sumSql + string.Format(" where {0} in ({1}) ", pager.keyField, rui.dbTools.getInExpression(this.Result, pager.keyField));
                            DataRow sumRow = dbHelper.ExecuteDataRow(sumSql);
                            foreach (DataColumn col in sumRow.Table.Columns)
                            {
                                pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                            }
                        }
                    }
                    //导出查询结果时，返回查询的所有数据
                    if (pager.ExportRange == rui.dataRange.all.ToString())
                    {
                        //查询要导出的数据
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql, pager.cmdPara);
                        rui.dbTools.insert序号(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //查询要导出的汇总行数据
                        if (rui.typeHelper.isNotNullOrEmpty(pager.sumRange))
                        {
                            string sumSql = pager.sumSql + rui.dbTools.getWhereExpr(querySql);
                            DataRow sumRow = dbHelper.ExecuteDataRow(sumSql, pager.cmdPara);
                            foreach (DataColumn col in sumRow.Table.Columns)
                            {
                                pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 导出和汇总数据范围
    /// </summary>
    public enum dataRange
    {
        /// <summary>
        /// 所有查询结果
        /// </summary>
        all,
        /// <summary>
        /// 本页面数据
        /// </summary>
        page,
        /// <summary>
        /// 勾选的数据
        /// </summary>
        selected
    }
}

