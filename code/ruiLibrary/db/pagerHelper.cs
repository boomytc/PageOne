using System.Data;
using System.Globalization;

namespace rui
{
    /// <summary>
    /// ���ݷ�ҳ�ĸ�����
    /// </summary>
    public class pagerHelper
    {
        private string querySql = "";       //����Ĳ�ѯ���
        private string countSql = "";       //ͨ����ѯ��乹��
        private string orderSql = "";       //��������������
        private string pagerSql = "";       //��ŷ�ҳ��ȡ�����
        private pagerBase pagerPara;        //��ҳ����
        private DataTable pagerResult;      //��ҳ����

        /// <summary>
        /// �����ҳ��ѯ���
        /// </summary>
        public DataTable Result { get { return pagerResult; } set { pagerResult = value; } }

        /// <summary>
        /// Ĭ�ϲ��������Ĺ��캯��
        /// </summary>
        public pagerHelper() { }

        /// <summary>
        /// �������Ĺ��캯��
        /// </summary>
        /// <param name="querySql">��ѯ�ַ���</param>
        /// <param name="orderSql">��ѯ�������ַ���</param>
        /// <param name="pagerPara">��ҳ����</param>
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
        /// �Դ���Ĳ�ѯsql���д���
        /// �������֮���ȡ��getField��countSql���
        /// </summary>
        private void processSql()
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            int selectEndPosition = compare.IndexOf(this.querySql, "select ", CompareOptions.IgnoreCase) + 6;
            int fromStartPosition = compare.IndexOf(this.querySql, " from ", CompareOptions.IgnoreCase);

            if (selectEndPosition < 0)
                rui.excptHelper.throwEx("select�ؼ��ֺ����Ҫ�пո�");
            if (fromStartPosition < 0)
                rui.excptHelper.throwEx("from�ؼ���ǰ����Ҫ�пո�");

            this.countSql = string.Format("select count(*) {0}", querySql.Substring(fromStartPosition));

            //�����ҳ���
            int offset = (this.pagerPara.PageIndex - 1) * this.pagerPara.PageSize;
            int next = this.pagerPara.PageSize;
            this.pagerSql = string.Format(" offset {0}  ROWS FETCH NEXT {1} ROWS only ", offset, next);

            //ƴ�����������
            //this.querySql += " " + this.orderSql;
        }

        /// <summary>
        /// ���ݲ�ѯ����
        /// ��ѯ��ϣ�����pageCount,rowCount
        /// </summary>
        /// <param name="pageSize">ҳ���С</param>
        /// <param name="pageIndex">��ǰҳ</param>
        /// <param name="pager">��ҳ����</param>
        /// <param name="exceptDateFieldName">��Ҫ�ų�����</param>
        public void Execute(int pageSize, int pageIndex, rui.pagerBase pager, string exceptDateFieldName = "")
        {
            using (rui.dbHelper dbHelper = rui.dbHelper.createHelper())
            {
                pager.LongDateField = exceptDateFieldName;

                //���������ݵ���
                if(rui.typeHelper.isNullOrEmpty(pager.SheetName))
                {
                    //��ȡ��ҳ����չʾ
                    this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql + this.pagerSql, pager.cmdPara);
                    rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                    this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                    //ִ�л������ʱ��ȡ��¼����
                    if (pager.ExeCountSql)
                    {
                        this.pagerPara.RowCount = rui.typeHelper.toInt(dbHelper.ExecuteScalar(this.countSql, pager.cmdPara));
                        this.pagerPara.PageCount = rui.typeHelper.toInt(pager.RowCount / pager.PageSize) + (pager.RowCount % pager.PageSize == 0 ? 0 : 1);
                    }

                    //���ܱ�ҳ������
                    if (pager.sumRange == rui.dataRange.page.ToString() && rui.typeHelper.isNotNullOrEmpty(pager.sumSql))
                    {
                        string sumSql = pager.sumSql + string.Format( " where {0} in ({1}) ",pager.keyField,rui.dbTools.getInExpression(this.pagerResult, pager.keyField));
                        DataRow sumRow = dbHelper.ExecuteDataRow(sumSql);
                        foreach (DataColumn col in sumRow.Table.Columns)
                        {
                            pager.sumRow.Add(col.ColumnName, rui.typeHelper.toDecimal(sumRow[col.ColumnName]));
                        }
                    }
                    //�������в�ѯ���
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
                //�������ݵ���
                else
                {
                    if (rui.typeHelper.isNullOrEmpty(pager.ExportRange))
                        rui.excptHelper.throwEx("ExportRange���Ա��븳ֵ");

                    //������ѡ���ݣ����ع�ѡ������
                    if (pager.ExportRange == rui.dataRange.selected.ToString())
                    {
                        if (rui.typeHelper.isNullOrEmpty(pager.CbxSelectedKeys))
                            rui.excptHelper.throwEx("CbxSelectedKeys���Ա��븳ֵ");

                        //��ѯҪ����������
                        string whereExpr = string.Format(" and {0} in ({1}) ",pager.keyField,rui.dbTools.getInExpression(pager.CbxSelectedKeys));
                        //��ȡ��ѡ�е�����
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + whereExpr + this.orderSql, pager.cmdPara);
                        rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //��ѯҪ�����Ļ���������
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
                    //������ҳʱ�����ر�ҳ������
                    if (pager.ExportRange == rui.dataRange.page.ToString())
                    {
                        //��ѯҪ����������
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql + this.pagerSql, pager.cmdPara);
                        rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //��ѯҪ�����Ļ���������
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
                    //������ѯ���ʱ�����ز�ѯ����������
                    if (pager.ExportRange == rui.dataRange.all.ToString())
                    {
                        //��ѯҪ����������
                        this.pagerResult = dbHelper.ExecuteDataTable(this.querySql + this.orderSql, pager.cmdPara);
                        rui.dbTools.insert���(this.pagerResult, pager.PageSize * (pager.PageIndex - 1));
                        this.Result = rui.dbTools.dateFormat(this.pagerResult, exceptDateFieldName);

                        //��ѯҪ�����Ļ���������
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
    /// �����ͻ������ݷ�Χ
    /// </summary>
    public enum dataRange
    {
        /// <summary>
        /// ���в�ѯ���
        /// </summary>
        all,
        /// <summary>
        /// ��ҳ������
        /// </summary>
        page,
        /// <summary>
        /// ��ѡ������
        /// </summary>
        selected
    }
}

